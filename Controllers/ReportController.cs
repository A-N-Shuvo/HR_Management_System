using HR_Management_System.Interfaces;
using HR_Management_System.Models;
using HR_Management_System.Reports;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;

namespace HR_Management_System.Controllers
{
    public class ReportController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        }

        public async Task<IActionResult> Index()
        {
            string selectedComId = Request.Cookies["SelectedCompany"];
            if (string.IsNullOrEmpty(selectedComId))
            {
                return RedirectToAction("Index", "Home");
            }

            var comIdGuid = Guid.Parse(selectedComId);
            var allDepartments = await _unitOfWork.Repository<Department>().GetAllAsync();
            var departments = allDepartments.Where(x => x.ComId == comIdGuid).ToList();

            ViewBag.Departments = departments;
            return View();
        }

        // এটেনডেন্স প্রিভিউ দেখার জন্য JSON ডাটা মেথড
        [HttpGet]
        public async Task<IActionResult> GetAttendanceData(Guid? deptId, DateTime fromDate, DateTime toDate)
        {
            string selectedComId = Request.Cookies["SelectedCompany"];
            if (string.IsNullOrEmpty(selectedComId)) return Json(new { success = false });

            var comIdGuid = Guid.Parse(selectedComId);
            var start = DateTime.SpecifyKind(fromDate, DateTimeKind.Utc);
            var end = DateTime.SpecifyKind(toDate, DateTimeKind.Utc);

            var data = await _unitOfWork.Employee.GetAttendanceListForReport(comIdGuid, deptId, start, end);
            return Json(new { data = data });
        }

        public async Task<IActionResult> PrintEmployeeList(Guid? deptId)
        {
            string selectedComId = Request.Cookies["SelectedCompany"];
            if (string.IsNullOrEmpty(selectedComId)) return RedirectToAction("Index", "Home");

            var comIdGuid = Guid.Parse(selectedComId);
            var data = await _unitOfWork.Employee.GetEmployeeListForReport(comIdGuid, deptId);

            var document = new EmployeeReportDocument(data);
            byte[] pdfBytes = document.GeneratePdf();
            return File(pdfBytes, "application/pdf");
        }

        public async Task<IActionResult> PrintAttendance(Guid? deptId, DateTime fromDate, DateTime toDate)
        {
            string selectedComId = Request.Cookies["SelectedCompany"];
            if (string.IsNullOrEmpty(selectedComId)) return RedirectToAction("Index", "Home");

            var comIdGuid = Guid.Parse(selectedComId);
            var start = DateTime.SpecifyKind(fromDate, DateTimeKind.Utc);
            var end = DateTime.SpecifyKind(toDate, DateTimeKind.Utc);

            var data = await _unitOfWork.Employee.GetAttendanceListForReport(comIdGuid, deptId, start, end);

            var document = new AttendanceReportDocument(data, start, end);
            byte[] pdfBytes = document.GeneratePdf();

            return File(pdfBytes, "application/pdf", "AttendanceReport.pdf");
        }

        // Salary List Report (ডিপার্টমেন্ট ফিল্টারসহ)
        public async Task<IActionResult> PrintSalaryList(int month, int year, Guid? deptId)
        {
            string selectedComId = Request.Cookies["SelectedCompany"];
            if (string.IsNullOrEmpty(selectedComId)) return RedirectToAction("Index", "Home");

            var comIdGuid = Guid.Parse(selectedComId);
            var data = await _unitOfWork.Employee.GetSalaryListForReport(comIdGuid, month, year, deptId);

            // Title প্যারামিটারটি সরিয়ে ফেলুন কারণ আপনার ডকুমেন্টে ৩টি প্যারামিটার (data, month, year) আছে
            var document = new SalaryReportDocument(data, month, year);

            byte[] pdfBytes = document.GeneratePdf();
            return File(pdfBytes, "application/pdf");
        }


        //=======================================
        public async Task<IActionResult> PrintSalarySummary(int month, int year, Guid? deptId)
        {
            string selectedComId = Request.Cookies["SelectedCompany"];
            if (string.IsNullOrEmpty(selectedComId)) return RedirectToAction("Index", "Home");

            var comIdGuid = Guid.Parse(selectedComId);

            // ডাটা নিয়ে আসা (একই রিপোজিটরি মেথড ব্যবহার করা যাবে)
            var data = await _unitOfWork.Employee.GetSalaryListForReport(comIdGuid, month, year, deptId);

            // ডিপার্টমেন্টের নাম বের করা (হেডারে দেখানোর জন্য)
            string deptName = "All Departments";
            if (deptId.HasValue)
            {
                var dept = await _unitOfWork.Repository<Department>().GetByIdAsync(deptId.Value);
                deptName = dept?.DeptName ?? "All Departments";
            }

            var document = new SalarySummaryReportDocument(data, month, year, deptName);
            byte[] pdfBytes = document.GeneratePdf();

            return File(pdfBytes, "application/pdf", $"SalarySummary_{deptName}_{month}.pdf");
        }
    }
}