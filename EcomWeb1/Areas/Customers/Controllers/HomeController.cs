using EcomWeb1.DataAccess.Repo.IRepo;
using EcomWeb1.Models;
using EcomWeb1.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace EcomWeb1.Areas.Customers.Controllers
{
    [Area("Customers")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            HttpContext.Session.SetInt32(SD.SS_SessionCartCount, 0);
            var claimIdentiy = (ClaimsIdentity)(User.Identity);
            var claim = claimIdentiy.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var count = _unitOfWork.ShoppingKart.GetAll(sc => sc.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.SS_SessionCartCount, count);
            }
            var products = _unitOfWork.Product.GetAll().ToList();

            foreach (var product in products)
            {
                var totalCopiesSold = _unitOfWork.OrderDetails.GetAll(od => od.ProductId == product.Id).Sum(od => od.Count);

                product.TotalCopiesSold = totalCopiesSold;
            }
            products = products.OrderByDescending(p => p.TotalCopiesSold).ToList();

            return View(products);
        }

        public IActionResult Details(int id)
        {
            var claimIdentiy = (ClaimsIdentity)(User.Identity);
            var claim = claimIdentiy.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var count = _unitOfWork.ShoppingKart.GetAll(sc => sc.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.SS_SessionCartCount, count);
            }
            var productInDb = _unitOfWork.Product.FirstOrDefault(p => p.Id == id, includeProperties: "category,coverType");
            if (productInDb == null) return NotFound();
            var shoppingcart = new ShoppingKart()
            {
                Product = productInDb,
                ProductId = productInDb.Id
            };
            return View(shoppingcart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingKart shoppingcart)
        {
            shoppingcart.Id = 0;
            if (ModelState.IsValid)
            {
                var claimIdentiy = (ClaimsIdentity)(User.Identity);
                var claim = claimIdentiy.FindFirst(ClaimTypes.NameIdentifier);
                if (claim == null) return NotFound();
                shoppingcart.ApplicationUserId = claim.Value;
                var shoppingCartInDb = _unitOfWork.ShoppingKart.FirstOrDefault(sc => sc.ApplicationUserId == claim.Value && sc.ProductId == shoppingcart.ProductId);
                if (shoppingCartInDb == null)
                    _unitOfWork.ShoppingKart.Add(shoppingcart);
                else
                    shoppingCartInDb.Count += shoppingcart.Count;
                _unitOfWork.save();
                return RedirectToAction("Index");
            }
            else
            {
                var productInDb = _unitOfWork.Product.FirstOrDefault(p => p.Id == shoppingcart.Id, includeProperties: "category,coverType");
                if (productInDb == null) return NotFound();
                var shoppingcartEdit = new ShoppingKart()
                {
                    Product = productInDb,
                    ProductId = productInDb.Id
                };
                return View(shoppingcartEdit);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}