using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Repository;
using BusinessLogic.Interfaces;
using DataAccess.CustomModel;
using DataAccess.DataContext;
using DataAccess.DataModels;
using Microsoft.EntityFrameworkCore;
using DataAccess.CustomModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using static BusinessLogic.Interfaces.IAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System.Text;
//using System.Web.Mvc;

namespace Hallodocweb.Controllers
{
    public class PatientController : Controller
    {
        private readonly ILogger<PatientController> _logger;
        private readonly IAuth _Auth;
        private readonly ApplicationDbContext _context;
        private readonly IPatientService _patientService;
        private readonly INotyfService _notyf;
        private readonly IHttpContextAccessor _htttpcontext;

        public PatientController(ILogger<PatientController> logger, IAuth auth, ApplicationDbContext context, IPatientService patientService, INotyfService notyf, IHttpContextAccessor httpContext)
        {
            _logger = logger;
            _Auth = auth;
            _context = context;
            _patientService = patientService;
            _notyf = notyf;
            _htttpcontext = httpContext;
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
        public async Task<IActionResult> CheckEmailExists(string email)
        {
            bool emailExists = await _patientService.IsEmailExists(email);
            return Json(new { emailExists });
        }

        public IActionResult patientsubreq()
        {
            return View();
        }

        [HttpPost]

        public IActionResult patientreg(LoginVm loginvm)
        {
            if (ModelState.IsValid)
            {
                //User user = _context.Users.FirstOrDefault(u => u.Aspnetuserid == item.Id);
                var user = _Auth.Login(loginvm);
                if (user != null)
                {
                    HttpContext.Session.SetString("Email", loginvm.Email);
                    _notyf.Success("Logged In Successfully !!");
                    return RedirectToAction("patientdashboard", user);
                }
                else
                {
                    _notyf.Error("Invalid Credentials");

                    //ViewBag.AuthFailedMessage = "Please enter valid username and password !!";
                }
                return View();
            }
            else
            {
                return View(loginvm);
            }
        }

        public IActionResult patientreg()
        {
            return View();
        }


        public IActionResult patientresetpass()
        {
            return View();
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        public IActionResult patientreq()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult patientreq(PatientInfoModel patientInfoModel)
        {
            if (ModelState.IsValid)
            {
                if (patientInfoModel.password != null)
                {
                    patientInfoModel.password = GenerateSHA256(patientInfoModel.password);
                }
                _patientService.AddPatientInfo(patientInfoModel);
                _notyf.Success("Submit Successfully !!");
                return RedirectToAction("patientsubreq", "Patient");
            }
            else
            {
                return View(patientInfoModel);
            }
        }

        public IActionResult familyfriendreq()
        {
            return View();
        }

        [HttpPost]
        public IActionResult familyfriendreq(FamilyReqModel familyReqModel)
        {
            if (ModelState.IsValid)
            {
                _patientService.AddFamilyReq(familyReqModel);
                return RedirectToAction("patientsubreq", "Patient");
            }
            else
            {
                return View(familyReqModel);
            }

        }

        public IActionResult businessInforeq()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult businessInforeq(BusinessReqModel businessReqModel)
        {
            if (ModelState.IsValid)
            {
                _patientService.AddBusinessReq(businessReqModel);
                return RedirectToAction("patientsubreq", "Patient");
            }
            else
            {
                return View(businessReqModel);
            }
        }

        public IActionResult conciergereq()
        {
            return View();
        }

        [HttpPost]
        public IActionResult conciergereq(ConciergeReqModel conciergeReqModel)
        {
            if (ModelState.IsValid)
            {
                _patientService.AddConciergeReq(conciergeReqModel);
                return RedirectToAction("patientsubreq", "Patient");
            }
            else
            {
                return View(conciergeReqModel);
            }
        }


        public IActionResult patientfpassword()
        {
            return View();
        }

        //[HttpPost]
        //public IActionResult patientfpassword(forgotpassword forgotpassword)
        //{
        //    _Auth.Resetreq(forgotpassword);
        //    return RedirectToAction("patientreg", "patient");
        //}

        [AllowAnonymous, HttpPost]
        public IActionResult patientfpassword(forgotpassword forgotpassword)
        {
            string? user = _context.Aspnetusers.Where(e => e.Email == forgotpassword.forgotemail).Select(x => x.Id).FirstOrDefault();
            if (user != null)
            {
                _Auth.Resetreq(forgotpassword);
                return RedirectToAction("patientreg", "patient");
            }
            else
            {
                _notyf.Error("Please Enter a Valid email adrress");
                return View();
            }

        }

        //public IActionResult patientsubinformation()
        //{
        //    return View();
        //}

        public IActionResult patientsubinformation(PatientInfoModel patientInfo)
        {
            var y = _patientService.subinformation(patientInfo);
            return View(y);
        }

        public IActionResult patientsomeoneelse()
        {
            return View();
        }

        public IActionResult patientcreateacc()
        {
            return View();
        }

        public IActionResult patientdashboard(User user, Requestwisefile requestwisefile)
        {
            //var rid = HttpContext.Session.GetInt32("rid");
            var infos = _patientService.GetMedicalHistory(user);
            var viewmodel = new MedicalHistory { MedicalHistoryList = infos , User = user};
            return View(viewmodel);
        }

        public IActionResult SubmitMeInfo()
        {
            return View();
        }

        public IActionResult _DocumentList(int Rid)
        {
            HttpContext.Session.SetInt32("rid", Rid);
            var y = _patientService.GetAllDocById(Rid);
            return View(y);
        }

        [HttpPost]
        public IActionResult _DocumentList()
        {
            int? rid = (int)HttpContext.Session.GetInt32("rid");
            var file = HttpContext.Request.Form.Files.FirstOrDefault();
            _patientService.AddFile(file);
            return RedirectToAction("_DocumentList","Patient", new { Rid = rid });
        }

        //[HttpGet]
        //public IActionResult GetDcoumentsById(int requestId)
        //{
        //    var list = _patientService.GetAllDocById(requestId);
        //    return PartialView("_DocumentList", list.ToList());
        //}

        //sending email
     
        public IActionResult UpdateProfile(User user)
        {
            user.Firstname = user.Firstname;
            return View(user);
        }

        public IActionResult _Profile()
        {
            return View();
        }

        public IActionResult Editing(User model)
        {
            var rid = HttpContext.Session.GetInt32("rid");
            var email = HttpContext.Session.GetString("Email");
            _patientService.Editing(email, model);
            return PartialView("_Profile", new { Rid = rid});
        }
    }
}
