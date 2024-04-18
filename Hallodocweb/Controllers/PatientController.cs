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
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HalloDoc.mvc.Auth;
using NuGet.Protocol;
using DataAccess.Enums;
using System.Net;
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
        private readonly IJwtService _jwtService;

        public PatientController(ILogger<PatientController> logger, IAuth auth, ApplicationDbContext context, IPatientService patientService, INotyfService notyf, IHttpContextAccessor httpContext, IJwtService jwtService)
        {
            _logger = logger;
            _Auth = auth;
            _context = context;
            _patientService = patientService;
            _notyf = notyf;
            _htttpcontext = httpContext;
            _jwtService = jwtService;
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
            loginvm.Password = GenerateSHA256(loginvm.Password);
            if (ModelState.IsValid)
            {
                LoginResponseViewModel? result = _patientService.PatientLogin(loginvm);
                if (result.Status == ResponseStatus.Success)
                {
                    HttpContext.Session.SetString("Email", loginvm.Email);
                    Response.Cookies.Append("jwt", result.Token);
                    TempData["Success"] = "Login Successfully";
                    return RedirectToAction("PatientDashboard", "Patient");
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                    TempData["Error"] = result.Message;
                    return View();
                }
            }
            return View();

            //if (ModelState.IsValid)
            //{
            //    string passwordhash = GenerateSHA256(loginvm.Password);
            //    loginvm.Password = passwordhash;
            //    var user = _Auth.Login(loginvm);

            //    //var userId = user.Userid;
            //    HttpContext.Session.SetInt32("UserId", user.Userid);

            //    if (user != null)
            //    {
            //        var jwtToken = _jwtService.GetJwtToken();
            //        Response.Cookies.Append("jwt", jwtToken);
            //        _notyf.Success("Logged In Successfully !!");
            //        return RedirectToAction("patientdashboard","Patient");
            //    }
            //    else
            //    {
            //        _notyf.Error("Invalid Credentials");

            //        //ViewBag.AuthFailedMessage = "Please enter valid username and password !!";
            //    }
            //    return View();
            //}
            //else
            //{
            //    return View(loginvm);
            //}
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
                bool isvalid = _patientService.AddPatientInfo(patientInfoModel);
                if (isvalid == false)
                {
                    _notyf.Error("Service is not available in entered Region");
                    return View(patientInfoModel);
                }
                //_patientService.AddPatientInfo(patientInfoModel);
            }
            _notyf.Success("Submit Successfully !!");
            return RedirectToAction("patientsubreq", "Patient");
            //else
            //{
            //    return View(patientInfoModel);
            //}
        }

        public IActionResult familyfriendreq()
        {
            return View();
        }

        [HttpPost]
        public IActionResult familyfriendreq(FamilyReqModel familyReqModel)
        {
            //if (ModelState.IsValid)
            //{
            string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            string createAccountLink = baseUrl + Url.Action("patientcreateacc", "Patient");
            bool isValid = _patientService.AddFamilyReq(familyReqModel, createAccountLink);
            if (!isValid)
            {
                _notyf.Error("Service is not available in entered Region");
                return View(familyReqModel);
            }
            _notyf.Success("Submit Successfully!!");
            //_patientService.AddFamilyReq(familyReqModel,createAccountLink);
            return RedirectToAction("patientsubreq", "Patient");
            //}
            //else
            //{
            //    return View(familyReqModel);
            //}

        }

        public IActionResult businessInforeq()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult businessInforeq(BusinessReqModel businessReqModel)
        {
            string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            string createAccountLink = baseUrl + Url.Action("patientcreateacc", "Patient");
            bool isValid = _patientService.AddBusinessReq(businessReqModel, createAccountLink);
            if (!isValid)
            {
                _notyf.Error("Service is not available in entered Region");
                return View(businessReqModel);
            }
            _notyf.Success("Submit Successfully !!");
            return RedirectToAction("patientsubreq", "Patient");
        }

        public IActionResult conciergereq()
        {
            return View();
        }

        [HttpPost]
        [HttpPost]
        public IActionResult conciergereq(ConciergeReqModel conciergeReqModel)
        {
            string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            string createAccountLink = baseUrl + Url.Action("patientcreateacc", "Patient");
            bool isValid = _patientService.AddConciergeReq(conciergeReqModel, createAccountLink);
            if (!isValid)
            {
                _notyf.Error("Service is not available in entered Region");
                return View(conciergeReqModel);
            }
            _notyf.Success("Submit Successfully !!");
            return RedirectToAction("patientsubreq", "Patient");
        }


        public IActionResult patientfpassword()
        {
            return View();
        }

        private string GenerateResetPasswordUrl(string userId)
        {
            string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            string resetPasswordPath = Url.Action("patientresetpass", new { id = userId });
            return baseUrl + resetPasswordPath;
        }
        private Task SendEmail(string email, string subject, string message)
        {
            var mail = "tatva.dotnet.yashvariya@outlook.com";
            var password = "Itzvariya@23";

            var client = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, password)
            };
            return client.SendMailAsync(new MailMessage(from: mail, to: email, subject, message));
        }

        public IActionResult PatientResetPasswordEmail(forgotpassword user)
        {
            var usr = _context.Aspnetusers.FirstOrDefault(x => x.Email == user.forgotemail);
            if (usr != null)
            {
                string Id = _context.Aspnetusers.FirstOrDefault(x => x.Email == user.forgotemail).Id;
                string resetPasswordUrl = GenerateResetPasswordUrl(Id);
                SendEmail(user.forgotemail, "Reset Your Password", $"Hello, reset your password using this link: {resetPasswordUrl}");
            }
            else
            {
                _notyf.Error("Email Id Not Registered");
                return RedirectToAction("patientfpassword", "Patient");
            }
            _notyf.Success("Kindly Check your mail for Reset Password");
            return RedirectToAction("patientreg");
        }

        //public IActionResult patientresetpass(string id)
        //{
        //    var aspuser = _context.Aspnetusers.FirstOrDefault(x => x.Id == id);
        //    return View(aspuser);
        //}

        [HttpPost]
        public IActionResult patientresetpass(Aspnetuser aspnetuser)
        {
            var aspuser = _context.Aspnetusers.FirstOrDefault(x => x.Id == aspnetuser.Id);
            aspnetuser.Passwordhash = GenerateSHA256(aspnetuser.Passwordhash);
            _context.Aspnetusers.Update(aspuser);
            _context.SaveChanges();
            _notyf.Success("Password change successfully!!");
            return RedirectToAction("patientreg");
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
                return View();
            }

        }

        //public IActionResult patientsubinformation()
        //{
        //    return View();
        //}

        //public IActionResult patientsubinformation(PatientInfoModel patientInfo)
        //{
        //    var y = _patientService.subinformation(patientInfo);
        //    return View(y);
        //}
        //[CustomAuthorize("User")]
        public IActionResult patientsomeoneelse()
        {
            return View();

        }

        public IActionResult patientcreateacc()
        {
            return View();
        }

        [HttpPost]
        public IActionResult patientcreateacc(CreateAccountModel createAccountModel)
        {
            
                bool isCreated = _patientService.CreateAccount(createAccountModel);
                if (isCreated)
                {
                    _notyf.Success("Account Created Successfully !!");
                    return RedirectToAction("patientreg", "Patient");
                }
                else
                {
                    _notyf.Error("Something went wrong !!");
                    return RedirectToAction("patientcreateacc");
                }

            
        }

        [CustomAuthorize("User")]
        public IActionResult patientdashboard()
        {
            string? Email = HttpContext.Session.GetString("Email");
            var user = _context.Users.Where(x=>x.Email == Email).FirstOrDefault();

            var infos = _patientService.GetMedicalHistory(user.Userid);

            return View(infos);
        }

        [CustomAuthorize("User")]
        public IActionResult SubmitMeInfo()
        {
            return View();
        }

        [CustomAuthorize("User")]
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

        //[httpget]
        //public iactionresult getdcoumentsbyid(int requestid)
        //{
        //    var list = _patientservice.getalldocbyid(requestid);
        //    return partialview("_documentlist", list.tolist());
        //}

        //sending email

        [CustomAuthorize("User")]
        public IActionResult ShowProfile(int userid)
        {
            HttpContext.Session.SetInt32("EditUserId", userid);
            var profile = _patientService.GetProfile(userid);
            return PartialView("_Profile", profile);
        }

        public IActionResult SaveEditProfile(Profile profile)
        {
            int EditUserId = (int)HttpContext.Session.GetInt32("EditUserId");
            profile.userId = EditUserId;
            bool isEdited = _patientService.EditProfile(profile);
            if (isEdited)
            {
                _notyf.Success("Profile Edited Successfully");
                return RedirectToAction("PatientDashboard");
            }
            else
            {
                _notyf.Error("Profile Edited Failed");
                return RedirectToAction("PatientDashboard");
            }
        }

        [CustomAuthorize("User")]
        public IActionResult _Profile()
        {
            return View();
        }

        [CustomAuthorize("User")]
        //me & someoneelse page
        public IActionResult patientsubinformation()
        {
            string Email = _htttpcontext.HttpContext.Session.GetString("Email");
            PatientInfoModel Reqobj = _patientService.FetchData(Email);
            return View(Reqobj);

        }

        [HttpPost]
        public IActionResult patientsubinformation(PatientInfoModel insertPatientRegister)
        {
            int reqTypeid = 1;
            int userid = (int)_htttpcontext.HttpContext.Session.GetInt32("UserId");
            try
            {
                _patientService.StoreData(insertPatientRegister, reqTypeid, userid);
                return RedirectToAction("patientdashboard");
            }
            catch
            {

                return View();
            }
        }

        public string GetLoginId()
        {
            var token = HttpContext.Request.Cookies["PatientJwt"];
            if (token == null || !_jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
            {
                return "";
            }
            var loginId = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "aspNetUserId");
            return loginId.Value;
        }

        [HttpPost]
        public IActionResult SubmitElseInfo(FamilyReqModel model)
        {
            string resetPasswordPath = Url.Action("ResetPassword");
            string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            string createAccountLink = baseUrl + resetPasswordPath;
            var loginid = GetLoginId();
            bool issubmitted = _patientService.SomeElseReq(model, createAccountLink, loginid);
            if (issubmitted)
            {
                _notyf.Success("Submitted Successfully");
                return RedirectToAction("patientdashboard");
            }
            else
            {
                _notyf.Error("Something wrong");
                return View(model);
            }
        }

    }
}
