using HR_Management_System.Data;
using HR_Management_System.Interfaces;
using HR_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HR_Management_System.Controllers
{
    public class AttendanceSummaryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context; // প্রসিডিওর কলের জন্য সরাসরি কন্টেক্সট লাগবে

        public AttendanceSummaryController(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(Request.Cookies["SelectedCompany"]))
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetSummary(int year, int month)
        {
            try
            {
                var selectedComId = Guid.Parse(Request.Cookies["SelectedCompany"]);

                // ১. ওই কোম্পানির সব এমপ্লয়ি এবং সামারি ডাটা নিয়ে আসা
                var summaries = await _unitOfWork.Repository<AttendanceSummary>().GetAllAsync();
                var employees = await _unitOfWork.Repository<Employee>().GetAllAsync();

                // ২. Join লজিক: EmpId এর বদলে EmpCode এবং EmpName ম্যাপ করা
                var result = summaries
                    .Where(s => s.ComId == selectedComId && s.dtYear == year && s.dtMonth == month)
                    .Join(employees,
                        s => s.EmpId,
                        e => e.EmpId,
                        (s, e) => new {
                            empCode = e.EmpCode, // GUID এর পরিবর্তে Code
                            empName = e.EmpName,
                            present = s.Present,
                            late = s.Late,
                            absent = s.Absent,
                            period = $"{s.dtYear}-{s.dtMonth:D2}"
                        }).ToList();

                return Json(new { data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GenerateSummary(int year, int month)
        {
            try
            {
                var selectedComId = Request.Cookies["SelectedCompany"];
                if (string.IsNullOrEmpty(selectedComId))
                    return Json(new { success = false, message = "Please select a company!" });

                // প্রসিডিওর কল করা (PostgreSQL সিনট্যাক্স)
                string query = $"CALL \"sp_GenerateAttendanceSummary\"('{selectedComId}', {year}, {month})";
                await _context.Database.ExecuteSqlRawAsync(query);

                return Json(new { success = true, message = "Attendance Summary Generated Successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}