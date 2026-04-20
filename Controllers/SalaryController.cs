using HR_Management_System.Data;
using HR_Management_System.Interfaces;
using HR_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HR_Management_System.Controllers
{
    public class SalaryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;

        public SalaryController(IUnitOfWork unitOfWork, ApplicationDbContext context)
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
        public async Task<IActionResult> GetSalaryList(int year, int month)
        {
            var cookieValue = Request.Cookies["SelectedCompany"];
            if (string.IsNullOrEmpty(cookieValue)) return Json(new { data = new List<object>() });

            var selectedComId = Guid.Parse(cookieValue);

            var salaries = await _unitOfWork.Repository<Salary>().GetAllAsync();
            var employees = await _unitOfWork.Repository<Employee>().GetAllAsync();

            // স্যালারি এবং এমপ্লয়ী টেবিল জয়েন করে ডেটা তৈরি (রিপোর্ট বা গ্রিডের জন্য সুবিধাজনক)
            var result = salaries
                .Where(s => s.ComId == selectedComId && s.dtYear == year && s.dtMonth == month)
                .Join(employees,
                    s => s.EmpId,
                    e => e.EmpId,
                    (s, e) => new {
                        id = s.Id,
                        empCode = e.EmpCode,
                        empName = e.EmpName,
                        gross = s.Gross,
                        basic = s.Basic,
                        absentAmount = s.AbsentAmount,
                        payableAmount = s.PayableAmount,
                        isPaid = s.IsPaid,
                        paidAmount = s.PaidAmount
                    }).ToList();

            return Json(new { data = result });
        }

        [HttpPost]
        public async Task<IActionResult> CalculateSalary(int year, int month)
        {
            var selectedComId = Request.Cookies["SelectedCompany"];
            if (string.IsNullOrEmpty(selectedComId))
                return Json(new { success = false, message = "Please select a company first!" });

            var comIdGuid = Guid.Parse(selectedComId);

            try
            {
                // ১. প্রথমে Attendance Summary জেনারেট করা
                await _unitOfWork.ExecuteRawSqlAsync($"CALL \"sp_GenerateAttendanceSummary\"('{comIdGuid}', {year}, {month})");

                // ২. তারপর স্যালারি ক্যালকুলেট করা
                await _unitOfWork.ExecuteRawSqlAsync($"CALL \"sp_CalculateSalary\"('{comIdGuid}', {year}, {month})");

                return Json(new { success = true, message = "Salary Calculated Successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePaymentStatus(Guid id)
        {
            try
            {
                var salary = await _unitOfWork.Repository<Salary>().GetByIdAsync(id);
                if (salary != null)
                {
                    salary.IsPaid = true;
                    salary.PaidAmount = salary.PayableAmount; // সম্পূর্ণ টাকা পরিশোধ
                    _unitOfWork.Repository<Salary>().Update(salary);
                    await _unitOfWork.CompleteAsync();
                    return Json(new { success = true, message = "Payment Successful!" });
                }
                return Json(new { success = false, message = "Data not found!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}