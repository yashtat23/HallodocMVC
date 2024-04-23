using AspNetCoreHero.ToastNotification.Abstractions;
using BusinessLogic.Interfaces;
using DataAccess.CustomModels;
using HalloDoc.mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System.Text;
using DataAccess.CustomModel;
using DataAccess.DataModels;
using Hallodocweb.Controllers;
using System.IdentityModel.Tokens.Jwt;
using BusinessLogic.Repository;
using System.Security.Claims;
using Rotativa.AspNetCore;

namespace HalloDoc.mvc.Controllers
{
    //[CustomAuthorize("Physician")]
    public class ProviderController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly INotyfService _notyf;
        private readonly IAdminService _adminService;
        private readonly IPatientService _patientService;
        private readonly IJwtService _jwtService;
        private readonly IProviderService _providerService;


        public ProviderController(ILogger<AdminController> logger, INotyfService notyfService, IAdminService adminService, IPatientService patientService, IJwtService jwtService, IProviderService providerService)
        {
            _logger = logger;
            _notyf = notyfService;
            _adminService = adminService;
            _patientService = patientService;
            _jwtService = jwtService;
            _providerService = providerService;
        }



        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize("Physician")]
        public IActionResult ProviderDashboard()
        {

            return View();
        }
        [HttpGet]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("AdminLogin", "Home");
        }
        public string GetLoginId()
        {
            var token = HttpContext.Request.Cookies["jwt"];
            if (token == null || !_jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
            {
                return "";
            }
            var loginId = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "aspNetUserId");
            return loginId.Value;
        }
        public IActionResult GetCount()
        {
            var aspid = GetLoginId();
            var phyid = _providerService.GetPhysicianId(aspid);
            var statusCountModel = _providerService.GetStatusCount(phyid);
            return PartialView("_PAllRequests", statusCountModel);
        }
        public IActionResult GetRequestsByStatus(int tabNo, int CurrentPage)
                {
            var aspid = GetLoginId();
            var phyid = _providerService.GetPhysicianId(aspid); 
            var list = _providerService.GetRequestsByStatus(tabNo, CurrentPage,phyid);

            if (tabNo == 0)
            {
                return Json(list);
            }
            if (tabNo == 1)
            {
                return PartialView("_PNewRequest", list);
            }
            else if (tabNo == 2)
            {
                return PartialView("_PPendingRequest", list);
            }
            else if (tabNo == 3)
            {
                return PartialView("_PActiveRequest", list);
            }
            else if (tabNo == 4)
            {
                return PartialView("_PConcludeRequest", list);
            }

            return View();
        }

        public IActionResult FilterRegion(FilterModel filterModel)
        {
            var aspid = GetLoginId();
            var phyid = _providerService.GetPhysicianId(aspid);
            var list = _providerService.GetRequestByRegion(filterModel, phyid);
            return PartialView("_PNewRequest", list);
        }

        [HttpPost]
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

        }

        [HttpGet]
        public IActionResult PSendLink()
        {
            return PartialView("_PSendLink");
        }
        [HttpPost]
        public IActionResult SendLink(SendLinkModel model)
        {
            bool isSend = false;
            try
            {
                string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
                string reviewPathLink = baseUrl + Url.Action("RequestScreen", "Patient");

                SendEmail(model.email, "Create Patient Request", $"Hello, please create patient request from this link: {reviewPathLink}");
                _notyf.Success("Link Sent");
                isSend = true;
            }
            catch (Exception ex)
            {
                _notyf.Error("Failed to sent");
            }
            return Json(new { isSend = isSend });

        }
        public Task SendEmail(string email, string subject, string message)
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

        [CustomAuthorize("Physician")]
        public IActionResult ViewCase(int Requestclientid, int RequestTypeId, int ReqId)
        {
            var model = _adminService.ViewCase(Requestclientid, RequestTypeId, ReqId);

            return View("_PViewCase", model);
        }

        [HttpPost]
        public IActionResult UpdateNotes(ViewNotesViewModel model)
        {
            int? reqId = HttpContext.Session.GetInt32("RNId");
            bool isUpdated = _adminService.UpdateAdminNotes(model.AdditionalNotes, (int)reqId);
            if (isUpdated)
            {
                _notyf.Success("Saved Changes!!");
                return RedirectToAction("ViewNote", "Provider", new { ReqId = reqId });

            }
            return View();
        }

        [CustomAuthorize("Physician")]
        public IActionResult ViewNote(int ReqId)
        {
            HttpContext.Session.SetInt32("RNId", ReqId);
            ViewNotesViewModel data = _adminService.ViewNotes(ReqId);
            return View("_PViewNotes", data);
        }

        public IActionResult CancelCase(int reqId)
        {

            var model = _adminService.CancelCase(reqId);
            model.reqId = reqId;
            return PartialView("_CancelCase", model);
        }


        [HttpPost]
        public IActionResult SubmitCancelCase(CancelCaseModel cancelCaseModel, int reqId)
        {
            cancelCaseModel.reqId = reqId;
            bool isCancelled = _adminService.SubmitCancelCase(cancelCaseModel);

            if (isCancelled)
            {
                _notyf.Success("Cancelled successfully");
                return RedirectToAction("ProviderDashboard");
            }
            _notyf.Error("Cancellation Failed");
            return RedirectToAction("ProviderDashboard");
        }

        [HttpGet]
        public IActionResult assigncase(int reqId)
        {
            HttpContext.Session.SetInt32("AssignReqId", reqId);
            var model = _adminService.AssignCase(reqId);
            return PartialView("_AssignCase", model);
        }

        public IActionResult GetPhysician(int selectRegion)
        {
            List<Physician> physicianlist = _adminService.GetPhysicianByRegion(selectRegion);
            return Json(new { physicianlist });
        }

        //[HttpPost]
        //public IActionResult SubmitAssignCase(AssignCaseModel assignCaseModel)
        //{
        //    assignCaseModel.requestId = HttpContext.Session.GetInt32("AssignReqId");
        //    bool isAssigned = _adminService.SubmitAssignCase(assignCaseModel);
        //    if (isAssigned)
        //    {
        //        _notyf.Success("Assigned successfully");
        //        return RedirectToAction("ProviderDashboard", "Provider");
        //    }
        //    return View();
        //}

        public IActionResult BlockCase(int reqId)
        {

            var model = _adminService.BlockCase(reqId);
            return PartialView("_BlockCase", model);
        }

        [HttpPost]
        public IActionResult SubmitBlockCase(BlockCaseModel blockCaseModel, int reqId)
        {
            blockCaseModel.ReqId = reqId;
            bool isBlocked = _adminService.SubmitBlockCase(blockCaseModel);
            if (isBlocked)
            {
                _notyf.Success("Blocked Successfully");
                return RedirectToAction("ProviderDashboard", "Provider");
            }
            _notyf.Error("BlockCase Failed");
            return RedirectToAction("ProviderDashboard", "Provider");
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
                return RedirectToAction("ViewUploads", "Provider", new { reqId = rid });
            }
            bool isUploaded = _adminService.UploadFiles(model.uploadedFiles, rid);
            if (isUploaded)
            {
                _notyf.Success("Uploaded Successfully");
                return RedirectToAction("ViewUploads", "Provider", new { reqId = rid });
            }
            else
            {
                _notyf.Error("Upload Failed");
                return RedirectToAction("ViewUploads", "Provider", new { reqId = rid });
            }
        }

        public IActionResult DeleteFileById(int id)
        {
            var rid = (int)HttpContext.Session.GetInt32("rid");
            bool isDeleted = _adminService.DeleteFileById(id);
            if (isDeleted)
            {
                return RedirectToAction("ViewUploads", "Provider", new { reqId = rid });
            }
            else
            {
                _notyf.Error("SomeThing Went Wrong");
                return RedirectToAction("ViewUploads", "Provider", new { reqId = rid });
            }
        }

        public IActionResult DeleteAllFiles(List<string> selectedFiles)
        {
            var rid = (int)HttpContext.Session.GetInt32("rid");
            bool isDeleted = _adminService.DeleteAllFiles(selectedFiles, rid);
            if (isDeleted)
            {
                _notyf.Success("Deleted Successfully");
                return RedirectToAction("ViewUploads", "Provider", new { reqId = rid });
            }
            _notyf.Error("SomeThing Went Wrong");
            return RedirectToAction("ViewUploads", "Provider", new { reqId = rid });

        }

        public IActionResult SendAllFiles(List<string> selectedFiles)
        {
            var rid = (int)HttpContext.Session.GetInt32("rid");


            //var message = string.Join(", ", selectedFiles);
            SendEmail("yashvariya23@gmail.com", "Documents", selectedFiles);
            _notyf.Success("Send Mail Successfully");
            return RedirectToAction("ViewUploads", "Provider", new { reqId = rid });
        }

        public Task SendEmail(string email, string subject, List<string> filenames)
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
            mailMessage.To.Add(email);
            mailMessage.From = new MailAddress(mail);

            mailMessage.Subject = subject;


            return client.SendMailAsync(mailMessage);
        }

        //[HttpGet]
        //public IActionResult Order(int reqId)
        //{
        //    var order = _adminService.FetchProfession();
        //    order.ReqId = reqId;
        //    return View(order);
        //}


        public IActionResult AcceptCase(int requestId)
        {
            var loginUserId = GetLoginId();
            _providerService.acceptCase(requestId, loginUserId);
            return RedirectToAction("ProviderDashboard");
        }

        [CustomAuthorize("Physician")]
        public IActionResult PViewDocument(int reqId)
        {
            HttpContext.Session.SetInt32("rid", reqId);
            var model = _adminService.GetAllDocById(reqId);
            return View(model);
        }

        [HttpGet]
        public IActionResult TranferRequest(int reqId)
        {

            TransferRequest model = new();
            model.ReqId = reqId;
            return PartialView("_PTransferRequest", model);
        }

        [HttpPost]
        public IActionResult TranferRequest(TransferRequest model)
        {

            bool isTranferred = _providerService.TransferRequest(model);
            if (isTranferred)
            {
                _notyf.Success("Tranferred Successfully");
                return RedirectToAction("ProviderDashboard", "Provider");
            }
            _notyf.Error("Tranferred Failed");
            return RedirectToAction("ProviderDashboard", "Provider");
        }

        [CustomAuthorize("Physician")]
        [HttpGet]
        public IActionResult POrder(int reqId)
        {
            var order = _adminService.FetchProfession();
            order.ReqId = reqId;
            return View(order);
        }

        [CustomAuthorize("Physician")]
        public IActionResult PEncounterForm(int reqId)
        {
            ViewBag.reqId = reqId;
            var form = _adminService.EncounterForm(reqId);
            return View(form);
        }

        [HttpPost]
        public IActionResult PEncounterForm(EncounterFormModel model)
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
            return RedirectToAction("PEncounterForm", new { ReqId = model.reqid });
        }

        public IActionResult pcaremodal(int reqId)
        {
            ViewBag.reqid = reqId;
            return PartialView("_PCareModal");
        }
        [HttpPost]
        public IActionResult EncounterTypeModalSubmit(int requestId, short encounterType)
        {
            _providerService.CallType(requestId, encounterType);
            return RedirectToAction("ProviderDashboard");
        }

        public IActionResult HouseCallSubmit(int requestId)
        {
            _providerService.housecall(requestId);
            return RedirectToAction("ProviderDashboard");
        }
        [HttpPost]
        public IActionResult EncounterFormSubmit(EncounterFormModel model)
        {
            _providerService.PSubmitEncounterForm(model);
            _notyf.Success("Successfully Finalize!!");
            return RedirectToAction("ProviderDashboard");
        }

        public IActionResult DownloadEncounterPopUp(int reqId)
        {
            ViewBag.reqId = reqId;
            return PartialView("_PDownloadModal");
        }

        public IActionResult DownloadEncounterPDF([FromQuery] int reqId)
        {
            var data = _adminService.EncounterForm(reqId);
            return new ViewAsPdf("PdfPartial", data)
            {
                FileName = "EncounterForm.pdf"
            };
            //return PartialView("_PConcludeRequest");
        }

        //public IActionResult concludeEncounter(int data)
        //{
        //    var model = _IAdminDash.concludeEncounter(data);
        //    return PartialView("ConcludeCare", model);
        //}
        //[HttpPost]
        //public IActionResult ConcludeViewUploadMain(adminDashData obj)
        //{
        //    adminDashData adminDashObj = new adminDashData();
        //    _IAdminDash.viewUploadMain(obj);
        //    return Json(new { data = obj._viewUpload[0].reqid });
        //}

        //[HttpPost]
        //public IActionResult DeleteFile(bool data, int id, int reqFileId)
        //{
        //    _IAdminDash.DeleteFile(data, reqFileId);
        //    return Json(new { data = id });

        //}


        //[HttpPost]
        //public IActionResult ProviderConcludeCarePost(adminDashData obj)
        //{
        //    adminDashData adminDashObj = new adminDashData();
        //    var sessionEmail = HttpContext.Session.GetString("UserSession");
        //    _IProviderDash.ProviderConcludeCarePost(obj, sessionEmail);
        //    return Ok();
        //}
        //public IActionResult FinalizePopup(int data)
        //{
        //    var dataMain = _IProviderDash.ProviderEncounterFormDownload(data);
        //    return Json(new { isSend = dataMain._ProviderEncounterPopUp.reqId });
        //}
        //public IActionResult FinalizeEncounter(int reqId)
        //{
        //    _IProviderDash.FinalizeEncounter(reqId);
        //    return Ok();
        //}
        //public async Task<IActionResult> ConcludeCare(int request_id)
        //{
        //    var data = await _providerService.ConcludeCare(request_id);
        //    return View(data);
        //}



        public string GetTokenEmail()
        {
            var token = HttpContext.Request.Cookies["jwt"];
            if (token == null || !_jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
            {
                return "";
            }
            var emailClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
            return emailClaim.Value;
        }

      

        [HttpGet]
        public IActionResult MyProfile()
        {
            var userid = GetLoginId();
            var tokenemail = GetTokenEmail();
            int phyId = _providerService.GetPhysicianId(userid);
            EditProviderModel2 model = new EditProviderModel2();
            model.editPro = _adminService.EditProviderProfile(phyId, tokenemail);
            model.regions = _adminService.RegionTable();
            model.physicianregiontable = _adminService.PhyRegionTable(phyId);
            model.roles = _adminService.GetRoles();
            return PartialView("_PMyProfile", model);

        }

        public IActionResult Scheduling(SchedulingViewModel model)
        {

            model.regions = _adminService.RegionTable().ToList();
            return PartialView("_MyScheduling", model);
        }

        public IActionResult LoadSchedulingPartial(string date, int regionid, int status)
        {
            var aspnetuserid = GetLoginId();
            var month = _providerService.PhysicianMonthlySchedule(date, status, aspnetuserid);
            return PartialView("_MonthlySchedule", month);

        }

        [HttpPost]
        public IActionResult AddShift(SchedulingViewModel model, List<int> repeatdays)
        {
            var email = GetTokenEmail();

            //var email = User.FindFirstValue(ClaimTypes.Email);
            var isAdded = _adminService.CreateShift(model, email, repeatdays);
            return Json(new { isAdded });
        }

        public IActionResult ViewShift(int ShiftDetailId)
        {
            var data = _adminService.ViewShift(ShiftDetailId);
            return PartialView("_PViewShift", data);
        }

        [CustomAuthorize("Physician")]
        public IActionResult ConcludeCare(int reqId)
        {
            HttpContext.Session.SetInt32("rid", reqId);
            var model = _adminService.GetAllDocById(reqId);
            return View(model);
        }

        [HttpPost]
        public IActionResult CUploadFiles(ViewUploadModel model)
        {
            var rid = (int)HttpContext.Session.GetInt32("rid");
            if (model.uploadedFiles == null)
            {
                _notyf.Error("First Upload Files");
                return RedirectToAction("ConcludeCare", "Provider", new { reqId = rid });
            }
            bool isUploaded = _adminService.UploadFiles(model.uploadedFiles, rid);
            if (isUploaded)
            {
                _notyf.Success("Uploaded Successfully");
                return RedirectToAction("ConcludeCare", "Provider", new { reqId = rid });
            }
            else
            {
                _notyf.Error("Upload Failed");
                return RedirectToAction("ConcludeCare", "Provider", new { reqId = rid });
            }
        }

        public IActionResult CDeleteFileById(int id)
        {
            var rid = (int)HttpContext.Session.GetInt32("rid");
            bool isDeleted = _adminService.DeleteFileById(id);
            if (isDeleted)
            {
                return RedirectToAction("ConcludeCare", "Provider", new { reqId = rid });
            }
            else
            {
                _notyf.Error("SomeThing Went Wrong");
                return RedirectToAction("ConcludeCare", "Provider", new { reqId = rid });
            }
        }

        public IActionResult CDeleteAllFiles(List<string> selectedFiles)
        {
            var rid = (int)HttpContext.Session.GetInt32("rid");
            bool isDeleted = _adminService.DeleteAllFiles(selectedFiles, rid);
            if (isDeleted)
            {
                _notyf.Success("Deleted Successfully");
                return RedirectToAction("ConcludeCare", "Provider", new { reqId = rid });
            }
            _notyf.Error("SomeThing Went Wrong");
            return RedirectToAction("ConcludeCare", "Provider", new { reqId = rid });

        }

        [HttpPost]
        public IActionResult ConcludeCareSubmit(int ReqId,string ProviderNote)
        {
            _providerService.concludecaresubmit(ReqId,ProviderNote);
            return RedirectToAction("ProviderDashboard");
        }

        [HttpPost]
        public IActionResult Finalizesubmit(int reqid)
        {
            var isFinalized = _providerService.finalizesubmit(reqid);
            _notyf.Success("Form Finilize");
            return Json(new { isFinalized });
        }

        public IActionResult RequestAdmin()
        {
            return PartialView("_PRequestAdmin");
        }

        [HttpPost]
        public IActionResult RequestAdmin(RequestAdmin model)
        {
            try
            {
                var email = GetTokenEmail();
                _providerService.RequestAdmin(model, email);
                _notyf.Success("Email Send Successfully");
                return Ok();
            }
            catch
            {
                return NotFound();
            }
        }

        public IActionResult LogoutProvider()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("AdminLogin", "Home");
        }

    }
}