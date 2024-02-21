using Microsoft.AspNetCore.Mvc;

namespace Hallodocweb.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult AdminLogin()
        {
            return View();
        }

        public IActionResult AdminDashboard()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
