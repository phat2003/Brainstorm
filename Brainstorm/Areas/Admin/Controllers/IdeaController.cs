using Brainstorm.DataAccess.Repository.IRepository;
using Brainstorm.Models;
using Brainstorm.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Brainstorm.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class IdeaController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;//biến này chỉ được đọc(không được ghi hay làm gì khác).
        private IWebHostEnvironment _webHostEnvironment;
        public IdeaController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<Idea> objIdeaList = _unitOfWork.Idea.GetAll(includeProperties: "Category,Topic");//chỗ này buộc phải có includeProperties để lấy dữ liệu từ bảng Category và Topic liên kết với bảng Idea. Nếu không View này sẽ báo lỗi null.
            return View(objIdeaList);
        }
        public IActionResult Upsert(int? id)
        {
            IdeaVM ideaVM = new IdeaVM();
            ideaVM.idea = new Idea();
            ideaVM.CategoryList = _unitOfWork.Category.GetAll().Select(
                u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }
                );
            ideaVM.TopicList = _unitOfWork.Topic.GetAll().Select(
                u => new SelectListItem { Text = u.Name, Value = u.Id.ToString() }
                );

            if (id == null || id == 0)//nếu id null hoặc = 0 thì trả về notfound (không tìm thấy).
            {
                //Create product
                return View(ideaVM);//mỗi lần tạo sản phẩm xong là return về View là product
            }
            else
            {
                //Update product
                ideaVM.idea = _unitOfWork.Idea.GetFirstOrDefault(u => u.Id == id);//lấy data sản phẩm có sẵn của bảng product đã thêm vào database trước đó có id trùng với id truyền vào.
                //lúc này productVM.product sẽ có dữ liệu của sản phẩm cần update chứ vẫn chưa update vì đây là action get để lấy dữ liệu từ database và show dữ liệu ra thôi.
                //action httppost sẽ làm nhiệm vụ update dữ liệu.

            }


            return View(ideaVM);//trả về view dù cho có đáp ứng 2 điều kiện trên hay không.
            
        }
        [HttpPost]
        public IActionResult Upsert(IdeaVM obj, IFormFile? filepath)
        {
            if (ModelState.IsValid)
            {
                //upload images
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (filepath != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\ideas");
                    var extension = Path.GetExtension(filepath.FileName);
                    if (obj.idea.FilePath != null)
                    {
                        //this is an edit and we need to remove old image
                        var oldImagePath = Path.Combine(wwwRootPath, obj.idea.FilePath.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        filepath.CopyTo(fileStreams);
                    }
                    obj.idea.FilePath = @"images\ideas\" + fileName + extension;

                }
                if (obj.idea.Id == 0)
                {
                    _unitOfWork.Idea.Add(obj.idea);
                }
                else
                {
                    _unitOfWork.Idea.Update(obj.idea);
                }

                _unitOfWork.Save();
                TempData["Sucess"] = "Product create sucessfully";
                return RedirectToAction("index");
            }

            // --- THÊM ĐOẠN NÀY ĐỂ FIX LỖI MẤT DROPDOWN KHI CÓ LỖI ---
            obj.CategoryList = _unitOfWork.Category.GetAll().Select(
                u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            obj.TopicList = _unitOfWork.Topic.GetAll().Select(
                u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            // --------------------------------------------------------

            return View(obj);
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var ideaFromDbFirst = _unitOfWork.Idea.GetFirstOrDefault(u => u.Id == id, includeProperties: "Category,Topic");
            if (ideaFromDbFirst == null)
            {
                return NotFound();
            }
            return View(ideaFromDbFirst);
        }
        [HttpPost]
        public IActionResult DeletePost(int? id)
        {
            var obj = _unitOfWork.Idea.GetFirstOrDefault(u => u.Id == id);

            if (obj == null)
            {
                return NotFound();
            }
            else
            {
                if (obj.FilePath != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    //this is an edit and we need to remove old image
                    var oldImagePath = Path.Combine(wwwRootPath, obj.FilePath.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                _unitOfWork.Idea.Remove(obj);
                _unitOfWork.Save();
                TempData["Sucess"] = "Category Delete sucessfully";
                return RedirectToAction("index");

            }
            return View(obj);
        }
    }
}
