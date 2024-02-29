using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using DataAccess.CustomModel;
using DataAccess.CustomModels;
using AspNetCore;
using AspNetCoreHero.ToastNotification.Abstractions;
using BusinessLogic.Repository;
using System.Text;
using System.Security.Cryptography;

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
        public IActionResult AdminLogin(AdminLoginModel adminLoginModel)
        {
            if (ModelState.IsValid)
            {
                var aspnetuser = _adminService.GetAspnetuser(adminLoginModel.email);
                if (aspnetuser != null)
                {
                    adminLoginModel.password = GenerateSHA256(adminLoginModel.password);
                    if (aspnetuser.Passwordhash == adminLoginModel.password)
                    {
                        _notyf.Success("Logged in Successfully");
                        return RedirectToAction("AdminDashboard", "Admin");
                    }
                    else
                    {
                        _notyf.Error("Password is incorrect");

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

        //public IActionResult AdminLogin(AdminLogin adminLogin)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        return RedirectToAction("AdminDashboard");
        //    }
        //    else
        //    {
        //        return View(adminLogin);
        //    }
        //}

        public IActionResult GetCount()
        {
            var statusCountModel = _adminService.GetStatusCount();
            return PartialView("_AllRequests", statusCountModel);
        }

        public IActionResult GetRequestsByStatus(int tabNo)
        {
            var list = _adminService.GetRequestsByStatus(tabNo);
            if (tabNo == 1)
            {
                return PartialView("_NewRequests", list);
            }
            else if (tabNo == 2)
            {
                return PartialView("_PendingRequests", list);
            }
            else if (tabNo == 3)
            {
                return PartialView("_ActiveRequests", list);
            }
            else if (tabNo == 4)
            {
                return PartialView("_ConcludeRequests", list);
            }
            else if (tabNo == 5)
            {
                return PartialView("_ToCloseRequests", list);
            }
            else if (tabNo == 6)
            {
                return PartialView("_UnpaidRequests", list);
            }
            return View();
        }

        public IActionResult AdminDashboard()
        {

            return View();
        }
        public IActionResult ViewCase(int Requestclientid, int RequestTypeId)
        {
            var obj = _adminService.ViewCaseViewModel(Requestclientid, RequestTypeId);

            return View(obj);

        }

        [HttpPost]
        public IActionResult UpdateNotes(ViewNotesViewModel model)
        {
            int? reqId = HttpContext.Session.GetInt32("RNId");
            bool isUpdated =  _adminService.UpdateAdminNotes(model.AdditionalNotes,(int)reqId);
            if (isUpdated)
            {
                _notyf.Success("Saved Changes!!");
                return RedirectToAction("ViewNote","Admin",new { ReqId = reqId });

            }
            return View();
        }

        public IActionResult ViewNote(int ReqId)
        {
            HttpContext.Session.SetInt32("RNId", ReqId);
            ViewNotesViewModel data = _adminService.ViewNotes(ReqId);
            return View(data);
        }
        
        public IActionResult Index()
        {
            return View();
        }

    }
}
