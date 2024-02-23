using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using DataAccess.CustomModel;
using DataAccess.CustomModels;
using AspNetCore;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace Hallodocweb.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IAdminService _adminService;
        private readonly INotyfService _notyf;

        public AdminController(ILogger<AdminController> logger,IAdminService adminService, INotyfService notyf)
        {
            _logger = logger;
            _adminService = adminService;
            _notyf = notyf;
        }

        public IActionResult AdminLogin(AdminLogin adminLogin)
        {
            if (ModelState.IsValid)
            {
                var user = _adminService.AdminLogin(adminLogin);
                if (user != null)
                {
                    _notyf.Success("Successfully Login!!");
                    return RedirectToAction("AdminDashboard",user);
                }
                else
                {
                    _notyf.Error("Invalid Credentials");
                }
                return View();
            }
            else
            {
                return View(adminLogin);
            }
        }

        public IActionResult AdminDashboard()
        {
            var list = _adminService.GetRequestsByStatus();
            return View(list);
        }

        public IActionResult ViewCase()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
