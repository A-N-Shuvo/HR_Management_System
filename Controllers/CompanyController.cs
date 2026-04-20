using HR_Management_System.Interfaces;
using HR_Management_System.Models;
using Microsoft.AspNetCore.Mvc;

namespace HR_Management_System.Controllers
{
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // মেইন পেজ লোড করার জন্য
        public IActionResult Index()
        {
            return View();
        }

        // সব কোম্পানির লিস্ট Ajax দিয়ে রিড করার জন্য
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var companies = await _unitOfWork.Repository<Company>().GetAllAsync();
            return Json(new { data = companies });
        }

        // ডাটা সেভ বা আপডেট করার জন্য
        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.ComId == Guid.Empty)
                {
                    await _unitOfWork.Repository<Company>().AddAsync(company);
                }
                else
                {
                    _unitOfWork.Repository<Company>().Update(company);
                }

                await _unitOfWork.CompleteAsync();
                return Json(new { success = true, message = "Saved Successfully" });
            }
            return Json(new { success = false, message = "Error while saving" });
        }

        // ডাটা ডিলিট করার জন্য
        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            var company = await _unitOfWork.Repository<Company>().GetByIdAsync(id);
            if (company == null) return Json(new { success = false, message = "Not Found" });

            _unitOfWork.Repository<Company>().Remove(company);
            await _unitOfWork.CompleteAsync();
            return Json(new { success = true, message = "Deleted Successfully" });
        }
    }
}