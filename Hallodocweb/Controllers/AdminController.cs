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
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using DataAccess.Data;
using DataAccess.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Drawing;

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

        public IActionResult GetRequest(int tabNo)
        {
            var list = _adminService.Expert(tabNo);
            return Json(list);
        }

        public IActionResult GetRequestsByStatus(int tabNo, int CurrentPage)
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
            else if (tabNo == 0)
            {
                return Json(list);
            }
            return View();
        }

        //public IActionResult FilterRegion(FilterModel filterModel)
        //{
        //    var list = _adminService.GetRequestByRegion(filterModel);
        //    return PartialView("_NewRequest", list);
        //}


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

        public IActionResult ViewCase(int Requestclientid, int RequestTypeId,int ReqId)
        {
            var obj = _adminService.ViewCase(Requestclientid, RequestTypeId,ReqId);

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


        public JsonArray GetPhysicianData(int regionId)
        {
            var result = _adminService.GetPhysician(regionId);
            return result;
        }

        [HttpPost]
        public IActionResult AssignCasePost(AssignCaseModel assignCaseModel)
        {
            _adminService.AssignCasePostData(assignCaseModel, assignCaseModel.requestId);
            _notyf.Success("Assign Successfully!");
            return RedirectToAction("AdminDashboard");
        }

        public IActionResult assignCase(int requestId)
        {
            AssignCaseModel assignCase = new AssignCaseModel
            {
                requestId = requestId,
                region = _adminService.GetRegion(),
            };
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

        [HttpGet]
        public IActionResult ClearCase(int reqId)
        {
            ViewBag.ClearCaseId = reqId;
            return PartialView("_ClearCase");
        }

        [HttpPost]
        public IActionResult SubmitClearCase(int reqId)
        {
            bool isClear = _adminService.Clearcase(reqId);
            if (isClear)
            {
                _notyf.Success("Cleared SUccessfully");
                return RedirectToAction("AdminDashboard");
            }
            _notyf.Error("Failed");
            return RedirectToAction("AdminDashboard");
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
            return PartialView("MyProfile", model);

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
                return RedirectToAction("AdminDashboard", "Admin");
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

        public IActionResult Provider()
        {
            var provider = _adminService.GetProvider();
            return PartialView("_Provider", provider);
        }

        public void ProviderCheckbox(int PhysicianId)
        {
            _adminService.StopNotification(PhysicianId);
        }

        public IActionResult ContactProvider(int physicianId)
        {
            var contact = _adminService.providerContact(physicianId);
            return PartialView("_ContactProvider", contact);
        }

        [HttpPost]
        public IActionResult providerContactModalEmail(int phyIdMain, string msg)
        {
            _adminService.providerContactEmail(phyIdMain, msg);
            return RedirectToAction("AdminDashboard");
        }

        public IActionResult EditProvider(int phyId)
        {
            var tokenemail = GetTokenEmail();
            if (tokenemail != null)
            {
                EditProviderModel2 model = new EditProviderModel2();
                model.editPro = _adminService.EditProviderProfile(phyId, tokenemail);
                model.regions = _adminService.RegionTable();
                model.physicianregiontable = _adminService.PhyRegionTable(phyId);
                model.roles = _adminService.GetRoles();
                return PartialView("_EditProvider", model);
            }
            _notyf.Error("Token is expired,Login again");
            return RedirectToAction("AdminLogin");
        }

        [HttpPost]
        public IActionResult providerEditFirst(string password, int phyId, string email)
        {

            bool editProvider = _adminService.providerResetPass(email, password);
            //_notyf.Success("Edit Save!");
            return Json(new { indicate = editProvider, phyId = phyId });
        }
        [HttpPost]
        public IActionResult editProviderForm1(int phyId, int roleId, int statusId)
        {
            bool editProviderForm1 = _adminService.editProviderForm1(phyId, roleId, statusId);
            _notyf.Success("Edit Save!");
            return Json(new { indicate = editProviderForm1, phyId = phyId });
        }
        [HttpPost]
        public IActionResult editProviderForm2(string fname, string lname, string email, string phone, string medical, string npi, string sync, int phyId, int[] phyRegionArray)
        {
            bool editProviderForm2 = _adminService.editProviderForm2(fname, lname, email, phone, medical, npi, sync, phyId, phyRegionArray);
            _notyf.Success("Edit Save!");
            return Json(new { indicate = editProviderForm2, phyId = phyId });
        }
        [HttpPost]
        public IActionResult editProviderForm3(EditProviderModel2 payloadMain)
        {
            bool editProviderForm3 = _adminService.editProviderForm3(payloadMain);
            _notyf.Success("Edit Save!");
            return Json(new { indicate = editProviderForm3, phyId = payloadMain.editPro.PhyID });
        }
        [HttpPost]
        public IActionResult PhysicianBusinessInfoEdit(EditProviderModel2 payloadMain)
        {
            bool editProviderForm4 = _adminService.PhysicianBusinessInfoUpdate(payloadMain);
            _notyf.Success("Edit Save!");
            return Json(new { indicate = editProviderForm4, phyId = payloadMain.editPro.PhyID });


        }
        [HttpPost]
        public IActionResult UpdateOnBoarding(EditProviderModel2 payloadMain)
        {
            var editProviderForm5 = _adminService.EditOnBoardingData(payloadMain);
            _notyf.Success("Edit Save!");
            return Json(new { indicate = editProviderForm5, phyId = payloadMain.editPro.PhyID });
        }
        public IActionResult editProviderDeleteAccount(int phyId)
        {
            _adminService.editProviderDeleteAccount(phyId);
            return Ok();
        }



        public IActionResult ProviderLocation()
        {
            return PartialView("_ProviderLocation");
        }
        public IActionResult GetLocation()
        {
            List<Physicianlocation> getLocation = _adminService.GetPhysicianlocations();
            return Ok(getLocation);
        }


        [HttpGet]
        public IActionResult ShowMyProfile()
        {
            var request = HttpContext.Request;
            var token = request.Cookies["jwt"];
            if (token == null || !_jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
            {
                return Json("token expired");
            }
            var emailClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);

            var model = _adminService.MyProfile(emailClaim.Value);
            return PartialView("_MyProfile", model);
        }
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
        [HttpPost]
        public IActionResult ResetPassword(string resetPassword)
        {
            var tokenEmail = GetTokenEmail();
            if (tokenEmail != "")
            {
                resetPassword = GenerateSHA256(resetPassword);
                bool isReset = _adminService.ResetPassword(tokenEmail, resetPassword);
                _notyf.Success("Reset Password!!");
                return Json(new { isReset = isReset });
            }
            return Json(new { isReset = false });
        }

        [HttpPost]
        public IActionResult SubmitAdminInfo(MyProfileModel model)
        {
            var tokenEmail = GetTokenEmail();
            if (tokenEmail != "")
            {
                bool isSubmit = _adminService.SubmitAdminInfo(model, tokenEmail);
                return Json(new { isSubmit = isSubmit });
            }
            return Json(new { isSubmit = false });
        }

        [HttpPost]
        public IActionResult SubmitBillingInfo(MyProfileModel model)
        {
            var tokenEmail = GetTokenEmail();
            if (tokenEmail != "")
            {
                var isRegionExists = _adminService.VerifyState(model.state);
                if (isRegionExists)
                {
                    bool isSubmit = _adminService.SubmitBillingInfo(model, tokenEmail);
                    return Json(new { isSubmit = isSubmit, isRegionExists = isRegionExists });
                }
                else
                {
                    return Json(new { isRegionExists = isRegionExists });
                }
            }
            return Json(new { isSubmit = false });
        }



        [HttpGet]
        public IActionResult ShowAccountAccess()
        {
            var obj = _adminService.AccountAccess();
            return PartialView("_AccountAccess", obj);
        }

        [HttpGet]
        public IActionResult DeleteRole(int RoleId)
        {
            var isDeleted = _adminService.DeleteRole(RoleId);
            return Json(new { isDeleted = isDeleted });
        }


        [HttpGet]
        public IActionResult CreateAccess()
        {
            var obj = _adminService.FetchRole(0);
            return PartialView("_CreateAccess", obj);
        }


        [HttpPost]
        public IActionResult CreateAccessPost(List<int> MenuIds, string RoleName, short AccountType)
        {
            var isRoleExists = _adminService.RoleExists(RoleName, AccountType);
            if (isRoleExists)
            {
                return Json(new { isRoleExists = true });
            }

            else
            {
                var isCreated = _adminService.CreateRole(MenuIds, RoleName, AccountType);
                return Json(new { isCreated = isCreated });
            }
            //return PartialView("_CreateAccess");
        }

        [HttpGet]
        public CreateAccess FetchRoles(short selectedValue)
        {
            var obj = _adminService.FetchRole(selectedValue);
            return obj;
        }


        [HttpGet]
        public IActionResult CreateAdminAccount()
        {
            var obj = _adminService.RegionList();
            return PartialView("_CreateAdminAccount", obj);
        }

        [HttpPost]
        public IActionResult AdminAccount(CreateAdminAccount model)
        {
                var email = GetTokenEmail();
                var isCreated = _adminService.CreateAdminAccount(model, email);
                if (isCreated)
                {
                    _notyf.Success("Account Created!!");
                    return RedirectToAction("AdminDashboard");
                }
                
                else
                {
                    _notyf.Error("Somethng Went Wrong!!");
                    return CreateAdminAccount();
                }
        }

        //public IActionResult CreateProviderAccount()
        //{
        //    var obj = _adminService.GetProviderList();
        //    return PartialView("_CreateProviderAccount", obj);
        //}

        public IActionResult CreateProviderAccount()
        {
            AdminEditPhysicianProfile data = new AdminEditPhysicianProfile();
            data.regions = _adminService.RegionTable();
            data.roles = _adminService.physicainRole();
            return PartialView("_CreateProviderAccount",data);
        }

        [HttpPost]
        public IActionResult createprovideraccount(AdminEditPhysicianProfile obj, List<int> physicianregions)
        {
            AdminEditPhysicianProfile data = new AdminEditPhysicianProfile();
            var createprovideraccount = _adminService.createProviderAccount(obj, physicianregions);
            return Json(new { indicate = createprovideraccount.indicate });
        }


        //public IActionResult CreateProviderAccount()
        //{
        //    return View("ProviderMenu/CreateProviderAccount", obj);
        //}

        //[HttpPost]
        //public IActionResult CreateProviderAccount(CreateProviderAccount model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var loginId = GetLoginId();
        //        if (loginId == "")
        //        {
        //            return RedirectToAction("AdminLogin");
        //        }
        //        _adminService.CreateProviderAccount(model, loginId);
        //        return RedirectToAction("AdminDashboard");

        //    }
        //    else
        //    {
        //        return PartialView("_CreateProviderAccount");
        //    }
        //}

        public IActionResult CreateShift()
        {
            var obj = _adminService.GetCreateShift();
            return View("_CreateShift", obj);
        }

        //public IActionResult Scheduling()
        //{
        //    //var obj = _adminService.CreateNewShiftSubmit();
        //    return PartialView("_Scheduling");
        //}

        public IActionResult MonthTable()
        {
            return PartialView("_MonthTable");
        }
        public IActionResult WeekTable()
        {
            return PartialView("_WeekTable");
        }
        public IActionResult DayTable()
        {
            return PartialView("_DayTable");
        }

        public IActionResult Partners()
        {
            return PartialView("_Patners");
        }
        public IActionResult BusinessTable()
        {
            var obj = _adminService.BusinessTable();
            return PartialView("_BusinessTable", obj);
        }

        public IActionResult Patners()
        {
            AddBusinessModel obj = new()
            {
                RegionList = _adminService.GetRegion(),
                ProfessionList = _adminService.GetProfession()
            };
            return PartialView("_Partners", obj);
        }

        [HttpPost]
        public IActionResult AddBusiness(AddBusinessModel obj)
        {
            if (obj.BusinessName != null && obj.FaxNumber != null)
            {
                _adminService.AddBusiness(obj);
                _notyf.Success("Save Data!!");
                return Ok();
            }
            else
            {
                _notyf.Error("Please Enter a Data");
                return BadRequest();
            }


            //int? adminId = HttpContext.Session.GetInt32("adminId");

        }

        public IActionResult AddVendor()
        {
            AddBusinessModel obj = new()
            {
                RegionList = _adminService.GetRegion(),
                ProfessionList = _adminService.GetProfession()
            };
            return PartialView("_AddVendor", obj);
        }

        public void DeleteBusiness(int VendorId)
        {
            _adminService.RemoveBusiness(VendorId);
            _notyf.Success("Delete Successfully!!");
        }

        public IActionResult EditBusinessData(int VendorId)
        {
            var obj = _adminService.GetEditBusiness(VendorId);
            return PartialView("_EditBusiness", obj);
        }

        [HttpPost]
        public IActionResult EditBusinessSubmit(EditBusinessModel model)
        {
            _adminService.EditBusiness(model);
            _notyf.Success("Data Updated!!");
            return Partners();
        }

        [HttpGet]
        public IActionResult SearchRecords(RecordsModel recordsModel)
        {
            RecordsModel model = new RecordsModel();
            model.requestListMain = _adminService.SearchRecords(recordsModel);
            if (model.requestListMain.Count() == 0)
            {
                RequestsRecordModel rec = new RequestsRecordModel();
                rec.flag = 1;
                model.requestListMain.Add(rec);
            }

            return PartialView("_SearchRecord", model);


        }

        [HttpGet]
        public IActionResult PatientRecords(PatientRecordsModel patientRecordsModel)
        {
            PatientRecordsModel model = new PatientRecordsModel();
            model.users = _adminService.PatientRecords(patientRecordsModel);

            if (model.users.Count() == 0)
            {
                model.flag = 1;
            }

            return PartialView("_PatientRecord", model);
        }

        [HttpGet]
        public IActionResult ShowUserAccess(short selectedValue)
        {
            var obj = _adminService.FetchAccess(selectedValue);
            return PartialView("_UserAccess", obj);
        }

        public IActionResult Scheduling(SchedulingViewModel model)
        {

            model.regions = _adminService.GetRegion().ToList();
            return PartialView("_Scheduling", model);
        }

        public IActionResult LoadSchedulingPartial(string PartialName, string date, int regionid, int status)
        {
            if (PartialName == "_DayTable")
            {
                var day = _adminService.GetDayTable(PartialName, date, regionid, status);
                return PartialView("_DayTable", day);

            }

            else if (PartialName == "_WeekTable")
            {
                var week = _adminService.GetWeekTable(date, regionid, status);
                return PartialView("_WeekTable", week);
            }
            else if (PartialName == "_MonthTable")
            {
                var month = _adminService.GetMonthTable(date, regionid, status);
                return PartialView("_MonthTable", month);
            }
            else
            {
                var day = _adminService.GetDayTable(PartialName, date, regionid, status);
                return PartialView("_DayTable", day);
            }
        }

        //[HttpGet]
        //public IActionResult WeekTable(string date, int regionid, int status)
        //{

        //}

        [HttpPost]
        public async Task<IActionResult> AddShift(SchedulingViewModel model, List<int> repeatdays)
        {
            var email = GetTokenEmail();

            //var email = User.FindFirstValue(ClaimTypes.Email);
            await _adminService.CreateShift(model, email, repeatdays);
            return RedirectToAction("AdminDashboard");
        }

        public async Task<IActionResult> ViewShift(int ShiftDetailId)
        {
            var data = await _adminService.ViewShift(ShiftDetailId);
            return View("_ViewShift", data);
        }

        public IActionResult BlockedHistory(BlockHistory2 blockHistory2)
        {
            BlockHistory2 _data = new BlockHistory2();
            _data.blockHistories = _adminService.BlockHistory(blockHistory2);


            return PartialView("_BlockHistory", _data);
        }


        public IActionResult unblockBlockHistory(int blockId)
        {
            bool isUnblocked = _adminService.UnblockRequest(blockId);
            return Json(new { isUnblocked = isUnblocked });
        }

        public IActionResult BlockHistoryCheckBox(int blockId)
        {
            bool isActive = _adminService.IsBlockRequestActive(blockId);
            return Json(new { isActive });
        }

        [HttpGet]
        public IActionResult EmailLogs(EmailSmsRecords2 recordsModel)
        {
            EmailSmsRecords2 _data = new EmailSmsRecords2();
            _data = _adminService.EmailSmsLogs(0, recordsModel);
            return PartialView("_EmailLogs", _data);
        }

        public IActionResult ReturnShift(int ShiftDetailId)
        {
            var email = GetTokenEmail();
            var data = _adminService.ReturnShift(ShiftDetailId, email);
            return Json(new { isReturned = data });
        }

        public IActionResult DeleteShift(int ShiftDetailId)
        {
            var email = GetTokenEmail();
            var data = _adminService.DeleteShift(ShiftDetailId, email);
            return Json(new { isDeleted = data });
        }

        public IActionResult EditViewShift(CreateNewShift model)
        {
            var email = GetTokenEmail();
            bool isEditted = _adminService.EditShift(model, email);

            return Json(new { isEditted });
        }

        //public IActionResult MdOnCall()
        //{
        //    var data =  _adminService.MdOnCall();
        //    return View(data);
        //}

        ////public IActionResult MdOnCallData(int region)
        ////{
        ////    var data = _adminService.MdOnCallData(region);
        ////    return PartialView("_ProviderOnCall", data);
        ////}


        public IActionResult MdOnCallData(int region)
        {
            var data = _adminService.GetOnCallDetails(region);
            return PartialView("_ProviderOnCall", data);
        }
        public IActionResult ShiftReview(int regionId, int callId)
        {
            ShiftReview2 schedulingCm = new ShiftReview2()
            {
                regions = _adminService.RegionTable(),
                ShiftReview = _adminService.GetShiftReview(regionId, callId),
                regionId = regionId,
                callId = callId,
            };

            return PartialView("_ShiftForReview", schedulingCm);
        }

        public IActionResult ApproveShift(int[] shiftDetailsId)
        {
            var Aspid = GetLoginId();
            bool isApproved = _adminService.ApproveSelectedShift(shiftDetailsId, Aspid);

            return Json(new { isApproved });
        }

        public IActionResult DeleteSelectedShift(int[] shiftDetailsId)
        {
            var Aspid = GetLoginId();

            bool isDeleted = _adminService.DeleteShiftReview(shiftDetailsId, Aspid);

            return Json(new { isDeleted });
        }
        public IActionResult FilterRegion(FilterModel filterModel)
        {
            var list = _adminService.GetRequestByRegion(filterModel);
            return PartialView("_NewRequests", list);
        }

        public IActionResult GetPatientRecordExplore(int userId)
        {

            var _data = _adminService.GetPatientRecordExplore(userId);

            return PartialView("_PatientRecordExplore", _data);
        }

    }
}