using HR_Management_System.Interfaces;
using HR_Management_System.Models;
using Microsoft.AspNetCore.Mvc;

namespace HR_Management_System.Controllers
{
    public class DesignationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DesignationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            // পেজ লোড হওয়ার সময় আমরা নিশ্চিত হবো কোম্পানি কুকি আছে কি না
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
            if (string.IsNullOrEmpty(selectedComId)) return Json(new { data = new List<Designation>() });

            var comIdGuid = Guid.Parse(selectedComId);
            var designations = await _unitOfWork.Repository<Designation>().GetAllAsync();

            // শুধু সিলেক্ট করা কোম্পানির পদবীগুলো দেখানো হবে
            var filtered = designations.Where(x => x.ComId == comIdGuid).ToList();
            return Json(new { data = filtered });
        }

        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] Designation designation)
        {
            var selectedComId = Request.Cookies["SelectedCompany"];
            if (string.IsNullOrEmpty(selectedComId)) return Json(new { success = false, message = "Company not selected!" });

            designation.ComId = Guid.Parse(selectedComId); // অটোমেটিক কুকি থেকে আইডি সেট করা

            if (ModelState.IsValid)
            {
                if (designation.DesigId == Guid.Empty)
                    await _unitOfWork.Repository<Designation>().AddAsync(designation);
                else
                    _unitOfWork.Repository<Designation>().Update(designation);

                await _unitOfWork.CompleteAsync();
                return Json(new { success = true, message = "Saved Successfully" });
            }
            return Json(new { success = false, message = "Error while saving" });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _unitOfWork.Repository<Designation>().GetByIdAsync(id);
            if (entity == null) return Json(new { success = false, message = "Not found" });

            _unitOfWork.Repository<Designation>().Remove(entity);
            await _unitOfWork.CompleteAsync();
            return Json(new { success = true, message = "Deleted Successfully" });
        }
    }
}