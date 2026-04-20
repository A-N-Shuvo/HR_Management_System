using HR_Management_System.Interfaces;
using HR_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HR_Management_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            // UnitOfWork ব্যবহার করে সব কোম্পানি নিয়ে আসা
            var companies = await _unitOfWork.Repository<Company>().GetAllAsync();

            // শুধু ComId এবং ComName সিলেক্ট করে পাঠানো যাতে ডাটা লাইট থাকে
            var result = companies.Select(c => new {
                comId = c.ComId,
                comName = c.ComName
            }).ToList();

            return Json(new { data = result });
        }

        [HttpPost]
        public IActionResult SetCompanyContext(Guid comId)
        {
            if (comId != Guid.Empty)
            {
                // কুকিতে কোম্পানি আইডি সেভ করা (Path=/ দিলে পুরো প্রজেক্টে পাওয়া যাবে)
                Response.Cookies.Append("SelectedCompany", comId.ToString(), new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(1),
                    Path = "/"
                });
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}