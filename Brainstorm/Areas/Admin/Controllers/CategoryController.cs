using Brainstorm.DataAccess.Data;
using Brainstorm.DataAccess.Repository;
using Brainstorm.DataAccess.Repository.IRepository;
using Brainstorm.Models;
using Microsoft.AspNetCore.Mvc;

namespace Brainstorm.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;//biến này chỉ được đọc(không được ghi hay làm gì khác).
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> objCategoryList = _unitOfWork.Category.GetAll();
            return View(objCategoryList);

        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj); // Thêm vào danh sách chờ
                _unitOfWork.Save();       // Lưu thực sự vào Database
                return RedirectToAction("Index"); // Quay về trang danh sách
            }
            return View(obj);
        }

        
        
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            //var categoryfromDb = _db.Categories.Find(id);
            var categoryfromDbFirst = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
            //var categoryfromDbsingle = _db.Categories.SingleOrDefault(u => u.Id == id);
            if (categoryfromDbFirst == null)
            {
                return NotFound();
            }
            return View(categoryfromDbFirst);
            
        }

        [HttpPost]
        public IActionResult DeletePost(int? id)
        {
            var obj = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
            //var categoryfromDbFirst = _db.Categories.FirstOrDefault(u=>u.Id==id);
            //var categoryfromDbsingle = _db.Categories.SingleOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            else
            {
                _unitOfWork.Category.Remove(obj);
                _unitOfWork.Save();
                TempData["Sucess"] = "Category Delete sucessfully";
                return RedirectToAction("index");
            }
            return View(obj);
            
        }
    }
}
