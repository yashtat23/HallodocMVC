using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using DataAccess.CustomModel;
using DataAccess.CustomModels;
using AspNetCore;
using AspNetCoreHero.ToastNotification.Abstractions;
using BusinessLogic.Repository;
using System.Text;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Net;
using BusinessLogic.Services;
using HalloDoc.mvc.Auth;
using System.Text.Json.Nodes;
using DataAccess.DataModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Hallodocweb.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IAdminService _adminService;
        private readonly INotyfService _notyf;
        private readonly IPatientService _petientService;
        private readonly IJwtService _jwtService;

        public AdminController(ILogger<AdminController> logger, IAdminService adminService, INotyfService notyf, IPatientService petientService, IJwtService jwtService)
        {
            _logger = logger;
            _adminService = adminService;
            _notyf = notyf;
            _petientService = petientService;
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
                        var jwtToken = _jwtService.GetJwtToken(aspnetuser);
                        Response.Cookies.Append("jwt", jwtToken);
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

        public IActionResult GetRequestsByStatus(int tabNo,int CurrentPage)
        {
            var list = _adminService.GetRequestsByStatus(tabNo, CurrentPage);
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
            else if(tabNo == 0)
            {
                return Json(list);
            }
            return View();
        }

        public IActionResult FilterRegion(int regionId, int tabNo)
        {
            var list = _adminService.GetRequestByRegion(regionId, tabNo);
            return PartialView("_NewRequest", list);
        }

        [CustomAuthorize("Admin")]
        public IActionResult AdminDashboard()
        {
            return View();
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("AdminLogin");
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
            bool isUpdated = _adminService.UpdateAdminNotes(model.AdditionalNotes, (int)reqId);
            if (isUpdated)
            {
                _notyf.Success("Saved Changes!!");
                return RedirectToAction("ViewNote", "Admin", new { ReqId = reqId });

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

        public IActionResult CancelCase(int reqId)
        {
            HttpContext.Session.SetInt32("CancelReqId", reqId);
            var model = _adminService.CancelCase(reqId);
            return PartialView("_CancelCase", model);
        }

        public IActionResult SubmitCancelCase(CancelCaseModel cancelCaseModel)
        {
            cancelCaseModel.reqId = HttpContext.Session.GetInt32("CancelReqId");
            bool isCancelled = _adminService.SubmitCancelCase(cancelCaseModel);
            if (isCancelled)
            {
                _notyf.Success("Cancelled successfully");
                return RedirectToAction("AdminDashboard", "Admin");
            }
            return View();
        }


        public IActionResult GetPhysicianData(int regionId)
        {
            var physicianData = _adminService.GetPhysician(regionId);
            return Json(physicianData);
        }

        [HttpPost]
        public IActionResult AssignCasePost(AssignCaseModel assignCaseModel)
        {
            _adminService.AssignCasePostData(assignCaseModel, assignCaseModel.requestId);
            return RedirectToAction("AdminDashboard");
        }

        public IActionResult assignCase(int requestId)
        {
            AssignCaseModel assignCase = new AssignCaseModel
            {
                requestId = requestId,
                region = _adminService.GetRegion(),
            };
            _notyf.Success("Assign Successfully!");
            return PartialView("_AssignCase", assignCase);
        }

        public IActionResult BlockCase(int reqId)
        {
            HttpContext.Session.SetInt32("BlockReqId", reqId);
            var model = _adminService.BlockCase(reqId);
            return PartialView("_BlockCase", model);
        }

        [HttpPost]
        public IActionResult SubmitBlockCase(BlockCaseModel blockCaseModel)
        {
            blockCaseModel.ReqId = HttpContext.Session.GetInt32("BlockReqId");
            bool isBlocked = _adminService.SubmitBlockCase(blockCaseModel);
            if (isBlocked)
            {
                _notyf.Success("Blocked Successfully");
                return RedirectToAction("AdminDashboard", "Admin");
            }
            _notyf.Error("BlockCase Failed");
            return RedirectToAction("AdminDashboard", "Admin");
        }

        public IActionResult ViewUploads(int reqId)
        {
            HttpContext.Session.SetInt32("rid", reqId);
            var model = _adminService.GetAllDocById(reqId);
            return View(model);
        }

        [HttpPost]
        public IActionResult UploadFiles(ViewUploadModel model)
        {
            var rid = (int)HttpContext.Session.GetInt32("rid");
            if (model.uploadedFiles == null)
            {
                _notyf.Error("First Upload Files");
                return RedirectToAction("ViewUploads", "Admin", new { reqId = rid });
            }
            bool isUploaded = _adminService.UploadFiles(model.uploadedFiles, rid);
            if (isUploaded)
            {
                _notyf.Success("Uploaded Successfully");
                return RedirectToAction("ViewUploads", "Admin", new { reqId = rid });
            }
            else
            {
                _notyf.Error("Upload Failed");
                return RedirectToAction("ViewUploads", "Admin", new { reqId = rid });
            }
        }

        public IActionResult DeleteFileById(int id)
        {
            var rid = (int)HttpContext.Session.GetInt32("rid");
            bool isDeleted = _adminService.DeleteFileById(id);
            if (isDeleted)
            {
                return RedirectToAction("ViewUploads", "Admin", new { reqId = rid });
            }
            else
            {
                _notyf.Error("SomeThing Went Wrong");
                return RedirectToAction("ViewUploads", "Admin", new { reqId = rid });
            }
        }

        public IActionResult DeleteAllFiles(List<string> selectedFiles)
        {
            var rid = (int)HttpContext.Session.GetInt32("rid");
            bool isDeleted = _adminService.DeleteAllFiles(selectedFiles, rid);
            if (isDeleted)
            {
                _notyf.Success("Deleted Successfully");
                return RedirectToAction("ViewUploads", "Admin", new { reqId = rid });
            }
            _notyf.Error("SomeThing Went Wrong");
            return RedirectToAction("ViewUploads", "Admin", new { reqId = rid });

        }

        public IActionResult SendAllFiles(List<string> selectedFiles)
        {
            var rid = (int)HttpContext.Session.GetInt32("rid");

            //var message = string.Join(", ", selectedFiles);
            SendEmail("yashvariya23@gmail.com", "Documents", selectedFiles);
            _notyf.Success("Send Mail Successfully");
            return RedirectToAction("ViewUploads", "Admin", new { reqId = rid });
        }

        private Task SendEmail(string email, string subject, List<string> filenames)
        {
            var mail = "tatva.dotnet.yashvariya@outlook.com";
            var password = "Itzvariya@23";

            var client = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, password)
            };

            MailMessage mailMessage = new MailMessage();
            for (var i = 0; i < filenames.Count; i++)
            {
                string pathname = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles", filenames[i]);
                Attachment attachment = new Attachment(pathname);
                mailMessage.Attachments.Add(attachment);
            }
            mailMessage.To.Add(email)
       ;
            mailMessage.From = new MailAddress(mail);

            mailMessage.Subject = subject;

            return client.SendMailAsync(mailMessage);
        }

        public IActionResult Order(int reqId)
        {
            var order = _adminService.FetchOrder(reqId);
            return View(order);
        }

        [HttpGet]
        public JsonArray FetchVendors(int selectedValue)
        {
            var result = _adminService.FetchVendors(selectedValue);
            return result;
        }

        [HttpGet]
        public Healthprofessional VendorDetails(int selectedValue)
        {
            var result = _adminService.VendorDetails(selectedValue);
            return result;
        }

        [HttpPost]
        public IActionResult Orderdetail(Order order)
        {
            _adminService.SendOrderDetails(order);
            return RedirectToAction("AdminDashboard", "Admin");
        }

        //public IActionResult _TransferRequests()
        //{
        //    return View();
        //}

        [HttpGet]
        public IActionResult Transferreq(int requestId)
        {
            AssignCaseModel assignCase = new AssignCaseModel
            {
                requestId = requestId,
                region = _adminService.GetRegion(),
            };
            _notyf.Success("Assign Successfully!");
            return PartialView("_TransferRequests", assignCase);
        }

        [HttpPost]
        public IActionResult TransferReqPost(AssignCaseModel assignCaseModel)
        {
            _adminService.TransferReqPostData(assignCaseModel, assignCaseModel.requestId);
            return View("AdminDashboard", "Admin");
        }

        public IActionResult clearCase(int reqId)
        {
            ViewBag.ClearCaseId = reqId;
            return PartialView("_ClearCase", "Admin");
        }

        [HttpPost]
        public IActionResult SubmitClearCase(int reqId)
        {

            _adminService.Clearcase(reqId);
            return View("AdminDashboard", "Admin");
        }


        [HttpGet]
        public IActionResult SendAgreement(int requestClientid, int reqType)
        {
            var model = _adminService.Agreement(requestClientid);
            model.reqType = reqType;
            return PartialView("_SendAgreement", model);
        }


        [HttpPost]
        public IActionResult SendAgreementSubmit(SendAgreement model)
        {
            var link = Url.Action("ReviewAgreement", "Home", new { ReqClientId = model.ReqClientId }, Request.Scheme);

            _adminService.SendAgreementEmail(model, link);
            return RedirectToAction("AdminDashboard");
        }

        public IActionResult CloseCase(int reqId)
        {
            var model = _adminService.closeCase(reqId);
            return View(model);
        }

        [HttpPost]
        public IActionResult EditCloseCase(CloseCase closeCase)
        {
            var edit = _adminService.EditCloseCase(closeCase);
            return View("AdminDashboard", edit);
        }

        public IActionResult ChangeCloseCase(CloseCase closeCase)
        {
            var change = _adminService.ChangeCloseCase(closeCase);
            return View("AdminDashboard");
        }

        public IActionResult AdminProfile()
        {
            var request = HttpContext.Request;
            var token = request.Cookies["jwt"];
            if (token == null || !_jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
            {
                return Json("ok");
            }
            var emailClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);

            var model = _adminService.MyProfile(emailClaim.Value);
            return PartialView("MyProfile",model);

        }

        public IActionResult EncounterForm(int reqId)
        {
            var form = _adminService.EncounterForm(reqId);
            return View(form);
        }

        [HttpPost]
        public IActionResult EncounterForm(EncounterFormModel model)
        {
            bool isSaved = _adminService.SubmitEncounterForm(model);
            if (isSaved)
            {
                _notyf.Success("Saved!!");
            }
            else
            {
                _notyf.Error("Failed");
            }
            return RedirectToAction("EncounterForm", new { ReqId = model.reqid });
        }

        [HttpPost]
        //public IActionResult ExportReq(List<RequestListAdminDash> reqList)
        public string ExportReq(List<AdminDashTableModel> reqList)
        {
            StringBuilder stringbuild = new StringBuilder();

            string header = "\"No\"," + "\"Name\"," + "\"DateOfBirth\"," + "\"Requestor\"," +
                "\"RequestDate\"," + "\"Phone\"," + "\"Notes\"," + "\"Address\"," 
                 + "\"Physician\"," + "\"DateOfService\"," + "\"Region\"," +
                "\"Status\"," + "\"RequestTypeId\"," + "\"OtherPhone\"," + "\"Email\"," + "\"RequestId\"," + Environment.NewLine + Environment.NewLine;

            stringbuild.Append(header);
            int count = 1;

            foreach (var item in reqList)
            {
                string content = $"\"{count}\"," + $"\"{item.firstName}\"," + $"\"{item.intDate}\"," + $"\"{item.requestorFname}\"," +
                    $"\"{item.intDate}\"," + $"\"{item.mobileNo}\"," + $"\"{item.notes}\"," + $"\"{item.street}\"," +
                    $"\"{item.lastName}\"," + $"\"{item.intDate}\"," + $"\"{item.street}\"," +
                    $"\"{item.status}\"," + $"\"{item.requestTypeId}\"," + $"\"{item.mobileNo}\"," + $"\"{item.firstName}\"," + $"\"{item.reqId}\"," + Environment.NewLine;

                count++;
                stringbuild.Append(content);
            }

            string finaldata = stringbuild.ToString();

            return finaldata;
            //return Json(new { message = finaldata });
        }

        [HttpGet]
        public IActionResult CreateReq()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateReq(CreateRequestModel model)
        {
            var request = HttpContext.Request;
            var token = request.Cookies["jwt"];
            if (token == null || !_jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
            {
                _notyf.Error("Token Expired");
                return View(model);
            }
            var emailClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
            var isSaved = _adminService.CreateRequest(model, emailClaim.Value);
            if (isSaved)
            {
                _notyf.Success("Request Created");
                return RedirectToAction("AdminDashboard","Admin");
            }
            else
            {
                _notyf.Error("Failed to Create");
                return View(model);
            }
        }

        public IActionResult RequestSupport()
        {
            return PartialView("_RequestSupport", "Admin");
        }

        //public IActionResult SendLink()
        //{
        //    return PartialView("_SendLink");
        //}


        [HttpGet]
        public IActionResult SendLink()
        {
            return PartialView("_SendLink");
        }

        public Task SendEmailFinal(string email, string subject, string message)
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

        [HttpPost]
        public IActionResult SendLink(SendLinkModel model)
        {
            bool isSend = false;
            try
            {
                string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
                string reviewPathLink = baseUrl + Url.Action("patientsubreq", "Patient");

                SendEmailFinal(model.email, "Create Patient Request", $"Hello, please create patient request from this link: {reviewPathLink}");
                _notyf.Success("Link Sent");
                isSend = true;
            }
            catch (Exception ex)
            {
                _notyf.Error("Failed to sent");
            }
            return Json(new { isSend = isSend });

        }

    }
}