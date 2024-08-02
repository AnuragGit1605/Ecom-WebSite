using EcomWeb1.Data;
using EcomWeb1.DataAccess.Repo.IRepo;
using EcomWeb1.Models;
using EcomWeb1.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Data;

namespace EcomWeb1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        public UserController(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var userList = _context.ApplicationUsers.ToList();
            var roles = _context.Roles.ToList();
            var userRoles = _context.UserRoles.ToList();
            foreach (var user in userList)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(r => r.Id == roleId).Name;
                if (user.CompanyId != null)
                {
                    user.Company = new Company()
                    {
                        Name = _unitOfWork.Company.Get(Convert.ToInt32(user.CompanyId)).Name
                    };
                }
                if (user.CompanyId == null)
                {
                    user.Company = new Company()
                    {
                        Name = " "
                    };
                }
            }
            var adminUser = userList.FirstOrDefault(u => u.Role == SD.Role_Admin);
            userList.Remove(adminUser);
            return Json(new {data=userList});
        }
        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            bool islock = false;
            var userInDb=_context.ApplicationUsers.FirstOrDefault(au=>au.Id==id);
            if (userInDb == null)
            return Json(new { success = false, message = "Something went wrong" });
            if(userInDb!=null && userInDb.LockoutEnd>DateTime.Now)
            {
                userInDb.LockoutEnd = DateTime.Now;
                islock= false;
            }
            else
            {
                userInDb.LockoutEnd = DateTime.Now.AddYears(100);
                islock = true;
            }
            _context.SaveChanges();
            return Json(new {success = true,message=islock==true?"User Successfully locked":"User Successfully Unlock"});
        }
        #endregion
        public IActionResult Index()
        {
            return View();
        }
    }
}
