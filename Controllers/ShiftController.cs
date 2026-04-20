using HR_Management_System.Interfaces;
using HR_Management_System.Models;
using Microsoft.AspNetCore.Mvc;

namespace HR_Management_System.Controllers
{
    public class ShiftController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShiftController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(Request.Cookies["SelectedCompany"]))
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var selectedComId = Guid.Parse(Request.Cookies["SelectedCompany"]);
            var shifts = await _unitOfWork.Repository<Shift>().GetAllAsync();
            var filtered = shifts.Where(x => x.ComId == selectedComId).ToList();
            return Json(new { data = filtered });
        }

        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] Shift shift)
        {
            // চেক ১: shift অবজেক্টটি নাল কি না
            if (shift == null) return Json(new { success = false, message = "Data is missing!" });

            // চেক ২: কুকি আছে কি না
            var selectedComId = Request.Cookies["SelectedCompany"];
            if (string.IsNullOrEmpty(selectedComId))
                return Json(new { success = false, message = "Please select a company first!" });

            shift.ComId = Guid.Parse(selectedComId);

            if (ModelState.IsValid)
            {
                if (shift.ShiftId == Guid.Empty)
                    await _unitOfWork.Repository<Shift>().AddAsync(shift);
                else
                    _unitOfWork.Repository<Shift>().Update(shift);

                await _unitOfWork.CompleteAsync();
                return Json(new { success = true, message = "Shift Saved Successfully" });
            }

            return Json(new { success = false, message = "Validation error!" });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var shift = await _unitOfWork.Repository<Shift>().GetByIdAsync(id);
                if (shift == null)
                {
                    return Json(new { success = false, message = "Shift not found!" });
                }

                // ডিলিট করার চেষ্টা
                _unitOfWork.Repository<Shift>().Remove(shift);
                await _unitOfWork.CompleteAsync();

                return Json(new { success = true, message = "Shift deleted successfully" });
            }
            catch (Exception ex)
            {
                // যদি এই শিফটটি কোনো এমপ্লয়ি টেবিলে ব্যবহার হয়ে থাকে, তবে এই এররটি আসবে
                return Json(new
                {
                    success = false,
                    message = "Cannot delete! This shift is already assigned to an employee."
                });
            }
        }
    }
}