using EcomWeb1.Data;
using EcomWeb1.DataAccess.Repo.IRepo;
using EcomWeb1.Models;
using EcomWeb1.Models.ViewModels;
using EcomWeb1.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Xml.Linq;
using static EcomWeb1.Models.ViewModels.OrderVM;

namespace EcomWeb1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        public OrderController(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }
        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Detail(int? id)
        {
            var orderHeader = _unitOfWork.OrderHeader.FirstOrDefault(o => o.Id == id, includeProperties: "ApplicationUser");
            if (orderHeader == null)
            {
                return NotFound();
            }

            var orderItems = _unitOfWork.OrderDetails.GetAll(od => od.OrderHeaderId == id, includeProperties: "Product");

            var orderDetailViewModel = new OrderVM
            {
                OrderId = orderHeader.Id,
                Name = orderHeader.ApplicationUser.Name,
                OrderDate = orderHeader.OrderDate.ToString("dd-MM-yyyy HH:mm"),
                OrderItems = orderItems.Select((item, index) => new OrderItem
                {
                    SerialNumber = index + 1,
                    ProductId = item.ProductId,
                    ProductName = item.Product.Title,
                    Quantity = item.Count,
                    Price = item.Price
                }).ToList()
            };

            return View(orderDetailViewModel);
        }
        public IActionResult GetAll()
        {
            var orders = _unitOfWork.OrderHeader.GetAll();
            var orderList = new List<OrderVM>();
            foreach (var order in orders)
            {
                var userName = _context.ApplicationUsers
                                   .Where(u => u.Id == order.ApplicationUserId)
                                   .Select(u => u.Name)
                                   .FirstOrDefault();
                var orderViewModel = new OrderVM
                {
                    Name = userName,
                    OrderDate = order.OrderDate.ToString("dd-MM-yyyy HH:mm"),
                    OrderTotal = order.OrderTotal,
                    OrderId = order.Id

                };
                orderList.Add(orderViewModel);
            }
            return Json(new { data = orderList });
        }

    }
}
