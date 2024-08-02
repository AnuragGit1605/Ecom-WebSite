using EcomWeb1.DataAccess.Repo.IRepo;
using EcomWeb1.Models;
using EcomWeb1.Models.ViewModels;
using EcomWeb1.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Stripe;
using Stripe.Checkout;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace EcomWeb1.Areas.Customers.Controllers
{
    [Area("Customers")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitofWork;
        private readonly ISMSSender _smsSender;
        private static bool isEmailConfirm = false;
        private readonly IEmailSender _emailsender;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CartController> _logger;
        private readonly IPaypal _paypal;
        public CartController(IUnitOfWork unitofwork, IEmailSender emailSender, UserManager<IdentityUser> userManager, IConfiguration configuration,
           ISMSSender smsSender, ILogger<CartController> logger, IPaypal paypal)
        {
            _unitofWork = unitofwork;
            _emailsender = emailSender;
            _userManager = userManager;
            _configuration = configuration;
            _smsSender = smsSender;
            _logger = logger;
            _paypal = paypal;
        }
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public IActionResult Index()
        {
            var claimIdentiy = (ClaimsIdentity)(User.Identity);
            var claim = claimIdentiy.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                ShoppingCartVM = new ShoppingCartVM()
                {
                    ListCart = new List<ShoppingKart>()
                };
                return View(ShoppingCartVM);
            }
            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitofWork.ShoppingKart.GetAll(sc => sc.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };
            ShoppingCartVM.OrderHeader.OrderTotal = 0;
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitofWork.ApplicationUser.FirstOrDefault(au => au.Id == claim.Value);
            foreach (var list in ShoppingCartVM.ListCart)
            {
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Product.Price * list.Count);
                if (list.Product.Description.Length > 100)
                {
                    list.Product.Description = list.Product.Description.Substring(0, 99) + "...";
                }
            }
            if (!isEmailConfirm)
            {
                ViewBag.EmailMessage = "Email has been sent kindly verify your email !!";
                ViewBag.EmailCSS = "text-success";
                isEmailConfirm = false;
            }
            else
            {
                ViewBag.EmailMessage = "Email Must be confirm for authorize customer !!";
                ViewBag.EmailCSS = "text-danger";
            }
            return View(ShoppingCartVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var user = _unitofWork.ApplicationUser.FirstOrDefault(au => au.Id == claims.Value);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email is Empty");
            }
            else
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code },
                    protocol: Request.Scheme);

                await _emailsender.SendEmailAsync(user.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
            }

            return RedirectToAction(nameof(Index));
        }
        public IActionResult plus(int id)
        {
            var cart = _unitofWork.ShoppingKart.FirstOrDefault(c => c.Id == id);
            cart.Count += 1;
            _unitofWork.save();
            return RedirectToAction("Index");
        }
        public IActionResult minus(int id)
        {
            var cart = _unitofWork.ShoppingKart.FirstOrDefault(c => c.Id == id);
            if (cart.Count == 1)
                cart.Count = 1;
            else
                cart.Count -= 1;
            _unitofWork.save();
            return RedirectToAction("Index");

        }
        public IActionResult delete(int id)
        {
            var cart = _unitofWork.ShoppingKart.FirstOrDefault(c => c.Id == id);
            _unitofWork.ShoppingKart.Remove(cart);
            _unitofWork.save();
            var claimIdentiy = (ClaimsIdentity)(User.Identity);
            var claim = claimIdentiy.FindFirst(ClaimTypes.NameIdentifier);
            var count = _unitofWork.ShoppingKart.GetAll(sc => sc.ApplicationUserId == claim.Value).ToList().Count;
            HttpContext.Session.SetInt32(SD.SS_SessionCartCount, count);
            return RedirectToAction("Index");
        }
        public IActionResult UpdateSelectedItem(int itemId, bool isChecked)
        {
            var cartItem = _unitofWork.ShoppingKart.FirstOrDefault(c => c.Id == itemId);
            if (cartItem != null)
            {
                cartItem.selected = isChecked;
                _unitofWork.save(); // Assuming you have a method to save changes
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }
        public IActionResult Summary()
        {
            var claimIdentiy = (ClaimsIdentity)(User.Identity);
            var claim = claimIdentiy.FindFirst(ClaimTypes.NameIdentifier);
            var uniqueAddresses = _unitofWork.OrderHeader.GetAll(o => o.ApplicationUserId == claim.Value).
                Select(o => o.StreetAddress + ", " + o.City + ", " + o.State + ", " + o.PostalCode)
                .Distinct().ToList();

            // Retrieve the list of cart items for the current user
            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitofWork.ShoppingKart.GetAll(sc => sc.ApplicationUserId == claim.Value && sc.selected, includeProperties: "Product"),
                OrderHeader = new OrderHeader(),
                UniqueAddresses = uniqueAddresses,
            };


            ShoppingCartVM.OrderHeader.ApplicationUser = _unitofWork.ApplicationUser.FirstOrDefault(au => au.Id == claim.Value);

            // Calculate order total and update item descriptions
            foreach (var list in ShoppingCartVM.ListCart)
            {
                // Calculate
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Product.Price * list.Count);

                // Shorten product description if necessary
                if (list.Product.Description.Length > 100)
                    list.Product.Description = list.Product.Description.Substring(0, 99) + "...";
            }

            // Set order header information from user profile
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;

            return View(ShoppingCartVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(string PaymentGateway)
        {
            var claimIdentiy = (ClaimsIdentity)(User.Identity);
            var claim = claimIdentiy.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitofWork.ApplicationUser.FirstOrDefault(au => au.Id == claim.Value);
            ShoppingCartVM.ListCart = _unitofWork.ShoppingKart.GetAll(sc => sc.ApplicationUserId == claim.Value && sc.selected, includeProperties: "Product");

            ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusPending;
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
            _unitofWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitofWork.save();

            foreach (var list in ShoppingCartVM.ListCart)
            {
                OrderDetail orderdetail = new OrderDetail()
                {
                    ProductId = list.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = list.Product.Price,
                    Count = list.Count
                };
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
                _unitofWork.OrderDetails.Add(orderdetail);
                _unitofWork.save();
            }

            if (PaymentGateway == "Stripe")
            {
                #region Stripe
                var returnUrl = Url.Action("OrderConfirmation", "Cart", new { id = ShoppingCartVM.OrderHeader.Id }, Request.Scheme);
                var cancelUrl = Url.Action("Summary", "Cart", null, Request.Scheme);
                var options = new SessionCreateOptions
                {
                    SuccessUrl = returnUrl,
                    CancelUrl = cancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };
                foreach (var list in ShoppingCartVM.ListCart)
                {
                    var sessionlistitem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)((list.Price * list.Count) * 100),
                            Currency = "inr",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = list.Product.Title
                            }
                        },
                        Quantity = list.Count
                    };

                    options.LineItems.Add(sessionlistitem);
                }

                var service = new SessionService();
                Session session = service.Create(options);

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
                #endregion
            }
            else if (PaymentGateway == "PayPal")
            {
                #region PayPal
                var returnUrl = Url.Action("OrderConfirmation", "Cart", new { id = ShoppingCartVM.OrderHeader.Id }, Request.Scheme);
                var cancelUrl = Url.Action("Summary", "Cart", null, Request.Scheme);
                var payment = await _paypal.CreateOrderAsync(ShoppingCartVM.OrderHeader.OrderTotal, returnUrl, cancelUrl);

                if (payment.state == "created")
                {
                    // Payment creation successful, redirect to PayPal approval URL
                    return Redirect(payment.links.First(link => link.rel.Equals("approval_url", StringComparison.OrdinalIgnoreCase)).href);
                }
                else
                {
                    return RedirectToAction("Summary", "Cart");
                }
                #endregion
            }
            else
            {
                // Payment gateway not selected, handle accordingly
                return RedirectToAction("Summary", "Cart");
            }
        }        
        public IActionResult OrderConfirmation(int id)
        {
            var order = _unitofWork.OrderHeader.FirstOrDefault(o => o.Id == id);

            SendOrderConfirmationSms(order);
            SendOrderConfirmationEmail(order);

            order.OrderStatus = SD.OrderStatusApproved;
            order.PaymentStatus = SD.PymentstatusApproved;
            order.PaymentDate = DateTime.Now;

            var claimIdentiy = (ClaimsIdentity)(User.Identity);
            var claim = claimIdentiy.FindFirst(ClaimTypes.NameIdentifier);         
            var cartItems = _unitofWork.ShoppingKart.GetAll(sc => sc.ApplicationUserId == claim.Value && sc.selected).ToList();
            _unitofWork.ShoppingKart.RemoveRange(cartItems);
            _unitofWork.save();

            if (claim != null)
            {
                var count = _unitofWork.ShoppingKart.GetAll(sc => sc.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.SS_SessionCartCount, count);
            }
            return View(id);
        }
        public void SendOrderConfirmationEmail(OrderHeader order)
        {
            var claimIdentiy = (ClaimsIdentity)(User.Identity);
            var claim = claimIdentiy.FindFirst(ClaimTypes.NameIdentifier);
            order.ApplicationUser= _unitofWork.ApplicationUser.FirstOrDefault(au => au.Id == claim.Value);
            var emailSettings = _configuration.GetSection("EmailSettings");

            var fromAddress = new MailAddress(emailSettings["FromEmail"], "Your Name");
            var toAddress = new MailAddress(order.ApplicationUser.Email, order.Name);
            var ccAddress = new MailAddress(emailSettings["CcEmail"]);
            var bccAddress = new MailAddress(emailSettings["BccEmail"]);
            string subject = "Order Confirmation - Order #" + order.Id;
            string body = "Dear " + order.ApplicationUser.Name + ",\n\n"
                        + "Thank you for your order. Your order with Order ID #" + order.Id + " has been successfully placed.\n"
                        + "Total Amount: ₹" + order.OrderTotal + "\n\n"
                        + "For any queries or concerns, please contact our customer support.\n\n"
                        + "Regards,\nShoppingKart";

            var smtp = new SmtpClient
            {
                Host = emailSettings["PrimaryDomain"],
                Port = Convert.ToInt32(emailSettings["PrimaryPort"]),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(emailSettings["UsernameEmail"], emailSettings["UsernamePassword"])
            };
            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            };
            message.CC.Add(ccAddress);
            message.Bcc.Add(bccAddress);

            smtp.Send(message);
        }
        public void SendOrderConfirmationSms(OrderHeader order)
        {
            _smsSender.SendSms(order.PhoneNumber,"Thank you for your order. " +
                "Your order with Order ID #" + order.Id + " has been successfully placed.");
        }
    }
}

