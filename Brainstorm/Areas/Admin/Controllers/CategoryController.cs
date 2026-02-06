using Brainstorm.DataAccess.Data;
using Brainstorm.Models;
using Microsoft.AspNetCore.Mvc;

namespace Brainstorm.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> objCategoryList = _db.Categories.ToList();
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
                _db.Categories.Add(obj); // Thêm vào danh sách chờ
                _db.SaveChanges();       // Lưu thực sự vào Database
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
            var categoryfromDb = _db.Categories.Find(id);
            //var categoryfromDbFirst = _db.Categories.FirstOrDefault(u=>u.Id==id);
            //var categoryfromDbsingle = _db.Categories.SingleOrDefault(u => u.Id == id);
            if (categoryfromDb == null)
            {
                return NotFound();
            }
            return View(categoryfromDb);
            
        }

        [HttpPost]
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.Categories.Find(id);
            //var categoryfromDbFirst = _db.Categories.FirstOrDefault(u=>u.Id==id);
            //var categoryfromDbsingle = _db.Categories.SingleOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            else
            {
                _db.Categories.Remove(obj);
                _db.SaveChanges();
                TempData["Sucess"] = "Category Delete sucessfully";
                return RedirectToAction("index");
            }
            return View(obj);
            
        }
    }
}
