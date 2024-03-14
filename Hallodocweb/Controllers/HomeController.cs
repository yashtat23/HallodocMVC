using BusinessLogic.Interfaces;
using BusinessLogic.Repository;
using DataAccess.CustomModel;
using Hallodocweb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Hallodocweb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAdminService _adminService;

        public HomeController(ILogger<HomeController> logger, IAdminService adminService)
        {
            _logger = logger;
            _adminService = adminService;

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult ReviewAgreement(int ReqClientId)
        {
            ReviewAgreement model = new()
            {
                ReqClientId = ReqClientId,
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult ReviewAgreement(ReviewAgreement Agreement)
        {
            bool isChange = _adminService.ReviewAgree(Agreement);
            return RedirectToAction("AdminDashboard");
        }

        [HttpGet]
        public IActionResult CancelAgreement(int requestClientId)
        {
            var obj = _adminService.CancelAgreement(requestClientId);
            return PartialView("CancelAgreementModal",obj);
        }

        [HttpPost]
        public IActionResult CancelAgreementSubmit(int ReqClientid, string Description)
        {
            CancelAngreement model = new()
            {
                ReqClientId= ReqClientid,
                Reason = Description
            };
            _adminService.CancelAgreement(model);
            return RedirectToAction("Index","Home");
        }


    }
}