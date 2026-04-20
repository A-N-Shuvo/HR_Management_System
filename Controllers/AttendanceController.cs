using HR_Management_System.Interfaces;
using HR_Management_System.Models;
using Microsoft.AspNetCore.Mvc;

namespace HR_Management_System.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AttendanceController(IUnitOfWork unitOfWork)
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
        public async Task<IActionResult> GetAttendanceData(DateTime date)
        {
            var selectedComId = Guid.Parse(Request.Cookies["SelectedCompany"]);

            // নির্দিষ্ট দিনের এটেনডেন্স চেক করা
            var existingAtt = (await _unitOfWork.Repository<Attendance>().GetAllAsync())
                                .Where(x => x.ComId == selectedComId && x.dtDate.Date == date.Date).ToList();

            // ওই কোম্পানির সব এমপ্লয়ি আনা
            var employees = (await _unitOfWork.Repository<Employee>().GetAllAsync())
                                .Where(x => x.ComId == selectedComId).ToList();

            var result = employees.Select(emp => new {
                empId = emp.EmpId,
                empCode = emp.EmpCode,
                empName = emp.EmpName,
                // যদি আগে থেকে ওই দিনের ডাটা থাকে তবে তা দেখাবে, নাহলে ডিফল্ট ডাটা
                attendance = existingAtt.FirstOrDefault(a => a.EmpId == emp.EmpId) ?? new Attendance
                {
                    dtDate = date,
                    InTime = TimeSpan.Parse("09:00"),
                    OutTime = TimeSpan.Parse("18:00"),
                    AttStatus = "P"
                }
            });

            return Json(new { data = result });
        }

        [HttpPost]
        public async Task<IActionResult> SaveBulkAttendance([FromBody] List<Attendance> attendanceList)
        {
            try
            {
                var selectedComId = Guid.Parse(Request.Cookies["SelectedCompany"]);

                foreach (var att in attendanceList)
                {
                    att.ComId = selectedComId;
                    att.dtDate = DateTime.SpecifyKind(att.dtDate, DateTimeKind.Utc);

                    // আগে থেকে ডাটা থাকলে আপডেট, নাহলে নতুন এন্ট্রি
                    var existing = (await _unitOfWork.Repository<Attendance>().GetAllAsync())
                                    .FirstOrDefault(x => x.EmpId == att.EmpId && x.dtDate.Date == att.dtDate.Date);

                    if (existing == null)
                    {
                        att.Id = Guid.NewGuid();
                        await _unitOfWork.Repository<Attendance>().AddAsync(att);
                    }
                    else
                    {
                        existing.InTime = att.InTime;
                        existing.OutTime = att.OutTime;
                        existing.AttStatus = att.AttStatus;
                        _unitOfWork.Repository<Attendance>().Update(existing);
                    }
                }

                await _unitOfWork.CompleteAsync();
                return Json(new { success = true, message = "Attendance saved successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}