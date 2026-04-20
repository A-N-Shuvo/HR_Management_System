using HR_Management_System.Interfaces;
using HR_Management_System.Models;
using Microsoft.AspNetCore.Mvc;

namespace HR_Management_System.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            // কোম্পানি সিলেক্ট করা না থাকলে হোম পেজে পাঠিয়ে দিবে
            if (string.IsNullOrEmpty(Request.Cookies["SelectedCompany"]))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var selectedComId = Request.Cookies["SelectedCompany"];
            if (string.IsNullOrEmpty(selectedComId)) return Json(new { data = new List<Department>() });

            var comIdGuid = Guid.Parse(selectedComId);
            var departments = await _unitOfWork.Repository<Department>().GetAllAsync();

            // শুধু বর্তমানে সিলেক্ট করা কোম্পানির ডিপার্টমেন্টগুলো দেখাবে
            var filtered = departments.Where(x => x.ComId == comIdGuid).ToList();
            return Json(new { data = filtered });
        }

        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] Department department)
        {
            var selectedComId = Request.Cookies["SelectedCompany"];
            if (string.IsNullOrEmpty(selectedComId))
                return Json(new { success = false, message = "Company context missing!" });

            department.ComId = Guid.Parse(selectedComId);

            if (ModelState.IsValid)
            {
                if (department.DeptId == Guid.Empty)
                    await _unitOfWork.Repository<Department>().AddAsync(department);
                else
                    _unitOfWork.Repository<Department>().Update(department);

                await _unitOfWork.CompleteAsync();
                return Json(new { success = true, message = "Department saved successfully" });
            }
            return Json(new { success = false, message = "Validation failed" });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id) // নাম পরিবর্তন করে Delete করুন যা JS থেকে কল হচ্ছে
        {
            var entity = await _unitOfWork.Repository<Department>().GetByIdAsync(id);
            if (entity == null) return Json(new { success = false, message = "Department not found" });

            _unitOfWork.Repository<Department>().Remove(entity); // রিপোজিটরির Remove কল হবে
            await _unitOfWork.CompleteAsync(); // এটি ডাটাবেজে পরিবর্তন সেভ করবে
            return Json(new { success = true, message = "Deleted successfully" });
        }
    }
}