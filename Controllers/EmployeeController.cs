using HR_Management_System.Interfaces;
using HR_Management_System.Models;
using Microsoft.AspNetCore.Mvc;

namespace HR_Management_System.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeController(IUnitOfWork unitOfWork)
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
            var selectedComId = Request.Cookies["SelectedCompany"];
            if (string.IsNullOrEmpty(selectedComId)) return Json(new { data = new List<Employee>() });

            var comIdGuid = Guid.Parse(selectedComId);

            // Join করে ডাটা আনা (আপনার Repository/UnitOfWork অনুযায়ী)
            var employees = await _unitOfWork.Repository<Employee>().GetAllAsync();
            var depts = await _unitOfWork.Repository<Department>().GetAllAsync();
            var desigs = await _unitOfWork.Repository<Designation>().GetAllAsync();

            var result = employees.Where(x => x.ComId == comIdGuid).Select(emp => new
            {
                empId = emp.EmpId,
                empCode = emp.EmpCode,
                empName = emp.EmpName,
                deptName = depts.FirstOrDefault(d => d.DeptId == emp.DeptId)?.DeptName ?? "N/A",
                desigName = desigs.FirstOrDefault(d => d.DesigId == emp.DesigId)?.DesigName ?? "N/A",
                gross = emp.Gross,
                dtJoin = emp.dtJoin
            }).ToList();

            return Json(new { data = result });
        }


        [HttpGet]
        public async Task<IActionResult> GetFormResources()
        {
            var cookie = Request.Cookies["SelectedCompany"];
            if (string.IsNullOrEmpty(cookie)) return Json(new { success = false });

            var selectedComId = Guid.Parse(cookie);

            var depts = (await _unitOfWork.Repository<Department>().GetAllAsync()).Where(x => x.ComId == selectedComId);
            var desigs = (await _unitOfWork.Repository<Designation>().GetAllAsync()).Where(x => x.ComId == selectedComId);
            var shifts = (await _unitOfWork.Repository<Shift>().GetAllAsync()).Where(x => x.ComId == selectedComId);

            return Json(new { departments = depts, designations = desigs, shifts = shifts });
        }

        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] Employee employee)
        {
            try
            {
                // ১. কুকি থেকে কোম্পানি আইডি চেক [cite: 17, 29]
                var selectedComId = Request.Cookies["SelectedCompany"];
                if (string.IsNullOrEmpty(selectedComId))
                    return Json(new { success = false, message = "Please select a company first!" });

                var comIdGuid = Guid.Parse(selectedComId);
                employee.ComId = comIdGuid;

                // ২. ডাটাবেজ থেকে কোম্পানির ক্যালকুলেশন পার্সেন্টেজ নিয়ে আসা [cite: 12, 13, 32]
                var company = await _unitOfWork.Repository<Company>().GetByIdAsync(comIdGuid);
                if (company == null)
                    return Json(new { success = false, message = "Selected company not found!" });

                // ৩. স্যালারি ক্যালকুলেশন লজিক (Company Table এর ভ্যালু অনুযায়ী) 
                if (employee.Gross > 0)
                {
                    employee.Basic = employee.Gross * (company.Basic > 1 ? company.Basic / 100 : company.Basic);
                    employee.HRent = employee.Gross * (company.Hrent > 1 ? company.Hrent / 100 : company.Hrent);
                    employee.Medical = employee.Gross * (company.Medical > 1 ? company.Medical / 100 : company.Medical);
                    employee.Others = 0; 
        }

                // ৪. dtJoin ফিক্স (PostgreSQL এর জন্য Utc বাধ্যতামূলক)
                if (employee.dtJoin.HasValue)
                {
                    employee.dtJoin = DateTime.SpecifyKind(employee.dtJoin.Value, DateTimeKind.Utc);
                }

                // ৫. ShiftId হ্যান্ডলিং (Guid.Empty হলে ডাটাবেজে null হিসেবে পাঠাতে)
                if (employee.ShiftId == Guid.Empty)
                {
                    employee.ShiftId = null;
                }

                if (ModelState.IsValid)
                {
                    if (employee.EmpId == Guid.Empty)
                    {
                        // নতুন এমপ্লয়ি তৈরির আগে EmpCode ইউনিক কিনা চেক করা ভালো 
                        employee.EmpId = Guid.NewGuid();
                        await _unitOfWork.Repository<Employee>().AddAsync(employee);
                    }
                    else
                    {
                        _unitOfWork.Repository<Employee>().Update(employee);
                    }

                    await _unitOfWork.CompleteAsync();
                    return Json(new { success = true, message = "Employee saved successfully!" });
                }

                // ভ্যালিডেশন এরর মেসেজ পাঠানো
                var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Json(new { success = false, message = "Validation failed: " + errors });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + (ex.InnerException?.Message ?? ex.Message) });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            var emp = await _unitOfWork.Repository<Employee>().GetByIdAsync(id);
            if (emp == null) return Json(new { success = false, message = "Employee not found" });

            _unitOfWork.Repository<Employee>().Remove(emp);
            await _unitOfWork.CompleteAsync();
            return Json(new { success = true, message = "Deleted successfully" });
        }
    }
}