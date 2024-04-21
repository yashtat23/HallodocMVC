using AspNetCoreHero.ToastNotification.Abstractions;
using BusinessLogic.Interfaces;
using BusinessLogic.Repository;
using BusinessLogic.Services;
using DataAccess.CustomModel;
using Hallodocweb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using System.Security.Cryptography;

namespace Hallodocweb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAdminService _adminService;
        private readonly INotyfService _notyf;
        private readonly IJwtService _jwtService;

        public HomeController(ILogger<HomeController> logger, IAdminService adminService,INotyfService notyf, IJwtService jwtService)
        {
            _logger = logger;
            _adminService = adminService;
            _notyf = notyf;
            _jwtService = jwtService;
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
            return RedirectToAction("Index");
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
            return RedirectToAction("Index");
        }

        public static string GenerateSHA256(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            using (var hashEngine = SHA256.Create())
            {
                var hashedBytes = hashEngine.ComputeHash(bytes, 0, bytes.Length);
                var sb = new StringBuilder();
                foreach (var b in hashedBytes)
                {
                    var hex = b.ToString("x2");
                    sb.Append(hex);
                }
                return sb.ToString();
            }
        }

        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View();
        }

        public IActionResult AdminLogout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("AdminLogin", "Home");
        }

        [HttpPost]
        public IActionResult AdminLogin(AdminLoginModelR adminLoginModel)
        {
            if (ModelState.IsValid)
            {
                var aspnetuser = _adminService.GetAspnetuser(adminLoginModel.email);
                if (aspnetuser != null)
                {
                    adminLoginModel.password = GenerateSHA256(adminLoginModel.password);
                    if (aspnetuser.Passwordhash == adminLoginModel.password)
                    {
                         
                        //string Aspid = HttpContext.Session.SetString("UserId");
                        int role = aspnetuser.Aspnetuserroles.Where(x => x.Userid == aspnetuser.Id).Select(x => x.Roleid).First();
                        if (role == 1)
                        {
                            var jwtToken = _jwtService.GetJwtToken(aspnetuser);
                            Response.Cookies.Append("jwt", jwtToken);
                            _notyf.Success("Logged in Successfully");
                            return RedirectToAction("AdminDashboard", "Admin");
                        }
                        else
                        {
                            var jwtToken = _jwtService.GetJwtToken(aspnetuser);
                            Response.Cookies.Append("jwt", jwtToken);
                            _notyf.Success("Logged in Successfully");
                            return RedirectToAction("ProviderDashboard", "Provider");

                        }
                    }
                    else
                    {
                        _notyf.Error("Invalid credentials!!");

                        return View();
                    }
                }
                _notyf.Error("Email is incorrect");
                return View();
            }
            else
            {
                return View(adminLoginModel);
            }
        }

    }
}