using HR_Management_System.Interfaces;
using HR_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;


namespace HR_Management_System.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AttendanceController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
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
        [HttpGet]
        public async Task<IActionResult> DownloadAttendanceSummary(DateTime monthYear)
        {
            var selectedComId = Guid.Parse(Request.Cookies["SelectedCompany"]);

            var fromDate = new DateTime(monthYear.Year, monthYear.Month, 1);
            var toDate = fromDate.AddMonths(1).AddDays(-1);

            var attendanceData = await _unitOfWork.Attendance.GetAttendanceSummaryAsync(selectedComId, fromDate, toDate);

            string reportPath = Path.Combine(_webHostEnvironment.WebRootPath, "Reports", "AttendanceReport.rdlc.rdl");

            if (!System.IO.File.Exists(reportPath))
            {
                return NotFound("RDLC Report File Not Found!"); 
            }

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = reportPath;

            // ৩. Report Builder-এর Dataset Name-এর সাথে ম্যাচ করে ডাটা পাঠানো
            localReport.DataSources.Add(new ReportDataSource("AttendanceDataSet", attendanceData));

            // ৪. PDF জেনারেট করা
            byte[] pdfBytes = localReport.Render("PDF");

            // ৫. ফাইল রেসপন্স দেওয়া
            return File(pdfBytes, "application/pdf", $"Attendance_Summary_{monthYear:MMM_yyyy}.pdf");
        }

    }
}