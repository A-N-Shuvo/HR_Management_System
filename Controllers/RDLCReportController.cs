using HR_Management_System.Data;
using HR_Management_System.DTO.ViewModel;
using HR_Management_System.Interfaces;
using HR_Management_System.Models;
using HR_Management_System.Reports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HR_Management_System.Controllers
{
    public class RDLCReportController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;

        public RDLCReportController(IUnitOfWork unitOfWork, IWebHostEnvironment env, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _env = env;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            string selectedComId = Request.Cookies["SelectedCompany"];
            if (string.IsNullOrEmpty(selectedComId))
                return RedirectToAction("Index", "Home");

            var comIdGuid = Guid.Parse(selectedComId);
            var allDepartments = await _unitOfWork.Repository<Department>().GetAllAsync();
            var departments = allDepartments.Where(x => x.ComId == comIdGuid).ToList();

            ViewBag.Departments = departments;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAttendanceData(Guid? deptId, DateTime fromDate, DateTime toDate)
        {
            string selectedComId = Request.Cookies["SelectedCompany"];
            if (string.IsNullOrEmpty(selectedComId)) return Json(new { success = false });

            var comIdGuid = Guid.Parse(selectedComId);
            var start = DateTime.SpecifyKind(fromDate, DateTimeKind.Utc);
            var end = DateTime.SpecifyKind(toDate, DateTimeKind.Utc);

            var empList = await _unitOfWork.Employee.GetAttendanceListForReport(comIdGuid, deptId, start, end);
            var result = empList.Select(e => new AttendanceSummaryDto
            {
                EmployeeCode = "",
                EmployeeName = e.EmpName,
                DepartmentName = "",
                TotalDays = (end - start).Days + 1,
                PresentDays = e.TotalPresent,
                AbsentDays = e.TotalAbsent,
                LateDays = e.TotalLate,
                LeaveDays = 0
            }).ToList();

            return Json(new { data = result });
        }

        public async Task<IActionResult> PrintEmployeeList(Guid? deptId)
        {
            string selectedComId = Request.Cookies["SelectedCompany"];
            if (string.IsNullOrEmpty(selectedComId)) return RedirectToAction("Index", "Home");

            var comIdGuid = Guid.Parse(selectedComId);
            var data = await _unitOfWork.Employee.GetEmployeeListForReport(comIdGuid, deptId);

            string deptName = "All Departments";
            if (deptId.HasValue)
            {
                var dept = await _unitOfWork.Repository<Department>().GetByIdAsync(deptId.Value);
                deptName = dept?.DeptName ?? "All Departments";
            }

            var dtoList = data.Select(e => new EmployeeReportDto
            {
                EmpCode = e.EmpCode,
                EmpName = e.EmpName,
                JoinDate = e.dtJoin,
                DeptName = e.DeptName,
                DesigName = e.DesigName,
                ShiftName = e.ShiftName
            }).ToList();

            string rdlcPath = Path.Combine(_env.WebRootPath, "Reports", "EmployeeReport.rdlc.rdl");

            var parameters = new Dictionary<string, string>
            {
                { "Department", deptId.HasValue ? deptName : "" }
            };

            byte[] pdfBytes = RdlcReportHelper.RenderReport(rdlcPath, "EmployeeDataSet", dtoList, "PDF", parameters);

            return File(pdfBytes, "application/pdf", "EmployeeList_RDLC.pdf");
        }

        public async Task<IActionResult> PrintAttendance(Guid? deptId, DateTime fromDate, DateTime toDate)
        {
            string selectedComId = Request.Cookies["SelectedCompany"];
            if (string.IsNullOrEmpty(selectedComId)) return RedirectToAction("Index", "Home");

            var comIdGuid = Guid.Parse(selectedComId);
            var start = DateTime.SpecifyKind(fromDate, DateTimeKind.Utc);
            var end = DateTime.SpecifyKind(toDate, DateTimeKind.Utc);

            var empList = await _unitOfWork.Employee.GetAttendanceListForReport(comIdGuid, deptId, start, end);

            string deptName = "All Departments";
            if (deptId.HasValue)
            {
                var dept = await _unitOfWork.Repository<Department>().GetByIdAsync(deptId.Value);
                deptName = dept?.DeptName ?? "All Departments";
            }

            var dtoList = empList.Select(e => new AttendanceSummaryDto
            {
                EmployeeCode = "",
                EmployeeName = e.EmpName,
                DepartmentName = "",
                TotalDays = (end - start).Days + 1,
                PresentDays = e.TotalPresent,
                AbsentDays = e.TotalAbsent,
                LateDays = e.TotalLate,
                LeaveDays = 0
            }).ToList();

            string rdlcPath = Path.Combine(_env.WebRootPath, "Reports", "AttendanceReport.rdlc.rdl");

            var parameters = new Dictionary<string, string>
            {
                { "Month", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fromDate.Month) },
                { "Year", fromDate.Year.ToString() },
                { "Department", deptId.HasValue ? deptName : "" },
                { "FromDate", fromDate.ToString("dd-MMM-yyyy") },
                { "ToDate", toDate.ToString("dd-MMM-yyyy") }
            };

            byte[] pdfBytes = RdlcReportHelper.RenderReport(rdlcPath, "AttendanceDataSet", dtoList, "PDF", parameters);

            return File(pdfBytes, "application/pdf", "Attendance_RDLC.pdf");
        }

        public async Task<IActionResult> PrintSalaryList(int month, int year, Guid? deptId)
        {
            string selectedComId = Request.Cookies["SelectedCompany"];
            if (string.IsNullOrEmpty(selectedComId)) return RedirectToAction("Index", "Home");

            var comIdGuid = Guid.Parse(selectedComId);
            var data = await _unitOfWork.Employee.GetSalaryListForReport(comIdGuid, month, year, deptId);

            var dtoList = data.Select(s => new SalaryReportDto
            {
                EmpName = s.EmpName,
                Gross = s.Gross,
                Basic = s.Basic,
                Hrent = s.Hrent,
                Medical = s.Medical,
                AbsentAmount = s.AbsentAmount,
                PayableAmount = s.PayableAmount
            }).ToList();

            string rdlcPath = Path.Combine(_env.WebRootPath, "Reports", "SalaryReport.rdlc.rdl");

            string deptName = "All Departments";
            if (deptId.HasValue)
            {
                var dept = await _unitOfWork.Repository<Department>().GetByIdAsync(deptId.Value);
                deptName = dept?.DeptName ?? "All Departments";
            }

            var parameters = new Dictionary<string, string>
            {
                { "Month", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month) },
                { "Year", year.ToString() },
                { "Department", deptId.HasValue ? deptName : "" }
            };

            byte[] pdfBytes = RdlcReportHelper.RenderReport(rdlcPath, "SalaryDataSet", dtoList, "PDF", parameters);

            return File(pdfBytes, "application/pdf", "Salary_RDLC.pdf");
        }

        public async Task<IActionResult> PrintSalarySummary(int month, int year, Guid? deptId)
        {
            string selectedComId = Request.Cookies["SelectedCompany"];
            if (string.IsNullOrEmpty(selectedComId)) return RedirectToAction("Index", "Home");

            var comIdGuid = Guid.Parse(selectedComId);
            var data = await _unitOfWork.Employee.GetSalaryListForReport(comIdGuid, month, year, deptId);

            string deptName = "All Departments";
            if (deptId.HasValue)
            {
                var dept = await _unitOfWork.Repository<Department>().GetByIdAsync(deptId.Value);
                deptName = dept?.DeptName ?? "All Departments";
            }

            var dtoList = data.Select(s => new SalarySummaryReportDto
            {
                EmpName = s.EmpName,
                Gross = s.Gross,
                AbsentAmount = s.AbsentAmount
            }).ToList();

            string rdlcPath = Path.Combine(_env.WebRootPath, "Reports", "SalarySummaryReport.rdlc.rdl");

            var parameters = new Dictionary<string, string>
            {
                { "Department", deptId.HasValue ? deptName : "" },
                { "Month", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month) },
                { "Year", year.ToString() }
            };

            byte[] pdfBytes = RdlcReportHelper.RenderReport(rdlcPath, "SalarySummaryDataSet", dtoList, "PDF", parameters);

            return File(pdfBytes, "application/pdf", $"SalarySummary_RDLC_{deptName}_{month}.pdf");
        }
    }
}
