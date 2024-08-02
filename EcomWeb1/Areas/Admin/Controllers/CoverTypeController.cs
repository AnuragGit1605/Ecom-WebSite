using EcomWeb1.DataAccess.Repo.IRepo;
using EcomWeb1.Models;
using EcomWeb1.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
namespace EcomWeb1.Areas.Admin.Controllers
{


    [Area("Admin")]
   // [Authorize(Roles = SD.Role_Admin + "" + SD.Role_Employee)]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var CoverTypeList = _unitOfWork.CoverType.GetAll();
            return Json(new { data = CoverTypeList });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var covertypeInDb = _unitOfWork.CoverType.Get(id);
            if (covertypeInDb == null)
                return Json(new { success = false, message = "Something Went Wrong While Deleting" });
            _unitOfWork.CoverType.Remove(covertypeInDb);
            _unitOfWork.save();
            return Json(new { success = true, message = "Data Deleted Successfully" });
        }
        #endregion
        public IActionResult Upsert(int? id)
        {
            CoverType covertype = new CoverType();
            if (id == null) return View(covertype);
            covertype = _unitOfWork.CoverType.Get(id.GetValueOrDefault());
            if (covertype == null) return NotFound();
            return View(covertype);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (coverType == null) return BadRequest();
            if (!ModelState.IsValid) return View(coverType);
            if (coverType.Id == 0)
                _unitOfWork.CoverType.Add(coverType);
            else
                _unitOfWork.CoverType.Update(coverType);
            _unitOfWork.save();
            return RedirectToAction(nameof(Index));
        }
    }
}