﻿using DataAccess.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using DataAccess.DataModels;
using DataAccess.CustomModel;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using DataAccess.Enum;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Nodes;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using static Org.BouncyCastle.Math.EC.ECCurve;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Hosting;
using System.Text.Json;

namespace BusinessLogic.Repository
{
    public class AdminService : IAdminService
    {

        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _environment;

        public AdminService(ApplicationDbContext db, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }

        public Aspnetuser GetAspnetuser(string email)
        {
            var aspNetUser = _db.Aspnetusers.Include(x => x.Aspnetuserroles).FirstOrDefault(x => x.Email == email);
            return aspNetUser;
        }

        //public bool AdminLogin(AdminLogin adminLogin)
        //{
        //    return _db.Aspnetusers.Any(x=>x.Email == adminLogin.Email && x.Passwordhash==adminLogin.Password);
        //}

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

        public List<AdminDashTableModel> Expert(int tabNo)
        {
            var query = from r in _db.Requests
                        join rc in _db.Requestclients on r.Requestid equals rc.Requestid
                        select new AdminDashTableModel
                        {
                            firstName = rc.Firstname,
                            lastName = rc.Lastname,
                            intDate = rc.Intdate,
                            intYear = rc.Intyear,
                            strMonth = rc.Strmonth,
                            requestorFname = r.Firstname,
                            requestorLname = r.Lastname,
                            createdDate = r.Createddate,
                            mobileNo = rc.Phonenumber,
                            city = rc.City,
                            state = rc.State,
                            street = rc.Street,
                            zipCode = rc.Zipcode,
                            requestTypeId = r.Requesttypeid,
                            status = r.Status,
                            Requestclientid = rc.Requestclientid,
                            reqId = r.Requestid,
                            regionId = rc.Regionid
                        };


            if (tabNo == 1)
            {

                query = query.Where(x => x.status == (int)StatusEnum.Unassigned);
            }

            else if (tabNo == 2)
            {

                query = query.Where(x => x.status == (int)StatusEnum.Accepted);
            }
            else if (tabNo == 3)
            {

                query = query.Where(x => x.status == (int)StatusEnum.MDEnRoute || x.status == (int)StatusEnum.MDOnSite);
            }
            else if (tabNo == 4)
            {

                query = query.Where(x => x.status == (int)StatusEnum.Conclude);
            }
            else if (tabNo == 5)
            {

                query = query.Where(x => (x.status == (int)StatusEnum.Cancelled || x.status == (int)StatusEnum.CancelledByPatient) || x.status == (int)StatusEnum.Closed);
            }
            else if (tabNo == 6)
            {

                query = query.Where(x => x.status == (int)StatusEnum.Unpaid);
            }
            var result = query.ToList();
            return result;

        }

        public DashboardModel GetRequestsByStatus(int tabNo, int CurrentPage)
        {
            var query = from r in _db.Requests
                        join rc in _db.Requestclients on r.Requestid equals rc.Requestid
                        select new AdminDashTableModel
                        {
                            firstName = rc.Firstname,
                            lastName = rc.Lastname,
                            intDate = rc.Intdate,
                            intYear = rc.Intyear,
                            strMonth = rc.Strmonth,
                            requestorFname = r.Firstname,
                            requestorLname = r.Lastname,
                            createdDate = r.Createddate,
                            mobileNo = rc.Phonenumber,
                            city = rc.City,
                            state = rc.State,
                            street = rc.Street,
                            zipCode = rc.Zipcode,
                            requestTypeId = r.Requesttypeid,
                            status = r.Status,
                            Requestclientid = rc.Requestclientid,
                            reqId = r.Requestid,
                            regionId = rc.Regionid
                        };


            if (tabNo == 1)
            {

                query = query.Where(x => x.status == (int)StatusEnum.Unassigned);
            }

            else if (tabNo == 2)
            {

                query = query.Where(x => x.status == (int)StatusEnum.Accepted);
            }
            else if (tabNo == 3)
            {

                query = query.Where(x => x.status == (int)StatusEnum.MDEnRoute || x.status == (int)StatusEnum.MDOnSite);
            }
            else if (tabNo == 4)
            {

                query = query.Where(x => x.status == (int)StatusEnum.Conclude);
            }
            else if (tabNo == 5)
            {

                query = query.Where(x => (x.status == (int)StatusEnum.Cancelled || x.status == (int)StatusEnum.CancelledByPatient) || x.status == (int)StatusEnum.Closed);
            }
            else if (tabNo == 6)
            {

                query = query.Where(x => x.status == (int)StatusEnum.Unpaid);
            }

            //var region = query.ToList();

            //DashboardModel dashboard = new DashboardModel();
            //dashboard.adminDashboardList = region;
            //dashboard.regionList = _db.Regions.ToList();

            //var result = query.ToList();
            //int count = result.Count();
            //int TotalPage = (int)Math.Ceiling(count / (double)5);
            //result = result.Skip((CurrentPage - 1) * 5).Take(5).ToList();

            //DashboardModel dashboardModel = new DashboardModel();
            //dashboardModel.adminDashTableList = result;
            //dashboardModel.regionList = _db.Regions.ToList();
            //dashboardModel.TotalPage = TotalPage;
            //dashboardModel.CurrentPage = CurrentPage;
            //return dashboardModel;

            var result = query.ToList();
            int count = result.Count();
            int TotalPage = (int)Math.Ceiling(count / (double)5);
            result = result.Skip((CurrentPage - 1) * 5).Take(5).ToList();

            DashboardModel dashboard = new DashboardModel();
            dashboard.adminDashboardList = result;
            dashboard.regionList = _db.Regions.ToList();
            dashboard.TotalPage = TotalPage;
            dashboard.CurrentPage = CurrentPage;
            return dashboard;
        }

        public DashboardModel GetRequestByRegion(int regionId, int tabNo)
        {
            DashboardModel model = new DashboardModel();
            model = GetRequestsByStatus(tabNo, 1);
            model.adminDashboardList = model.adminDashboardList.Where(x => x.regionId == regionId).ToList();
            return model;
        }

        public bool UpdateAdminNotes(string additionalNotes, int reqId)
        {
            var reqNotes = _db.Requestnotes.FirstOrDefault(x => x.Requestid == reqId);
            try
            {

                if (reqNotes == null)
                {
                    Requestnote rn = new Requestnote();
                    rn.Requestid = reqId;
                    rn.Adminnotes = additionalNotes;
                    rn.Createdby = "admin";
                    //here instead of admin , add id of the admin through which admin is loggedIn 
                    rn.Createddate = DateTime.Now;
                    _db.Requestnotes.Add(rn);
                    _db.SaveChanges();
                }
                else
                {
                    reqNotes.Adminnotes = additionalNotes;
                    reqNotes.Modifieddate = DateTime.Now;
                    reqNotes.Modifiedby = "admin";
                    //here instead of admin , add id of the admin through which admin is loggedIn 
                    _db.Requestnotes.Update(reqNotes);
                    _db.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public StatusCountModel GetStatusCount()
        {
            var requestsWithClients = _db.Requests
     .Join(_db.Requestclients,
         r => r.Requestid,
         rc => rc.Requestid,
         (r, rc) => new { Request = r, RequestClient = rc })
     .ToList();

            StatusCountModel statusCount = new StatusCountModel
            {
                NewCount = requestsWithClients.Count(x => x.Request.Status == (int)StatusEnum.Unassigned),
                PendingCount = requestsWithClients.Count(x => x.Request.Status == (int)StatusEnum.Accepted),
                ActiveCount = requestsWithClients.Count(x => x.Request.Status == (int)StatusEnum.MDEnRoute || x.Request.Status == (int)StatusEnum.MDOnSite),
                ConcludeCount = requestsWithClients.Count(x => x.Request.Status == (int)StatusEnum.Conclude),
                ToCloseCount = requestsWithClients.Count(x => (x.Request.Status == (int)StatusEnum.Cancelled || x.Request.Status == (int)StatusEnum.CancelledByPatient) || x.Request.Status == (int)StatusEnum.Closed),
                UnpaidCount = requestsWithClients.Count(x => x.Request.Status == (int)StatusEnum.Unpaid)
            };

            return statusCount;
        }

        public ViewCaseViewModel ViewCaseViewModel(int Requestclientid, int RequestTypeId)
        {
            Requestclient obj = _db.Requestclients.FirstOrDefault(x => x.Requestclientid == Requestclientid);
            Request req = _db.Requests.FirstOrDefault(x => x.Requestid == obj.Requestid);
            ViewCaseViewModel viewCaseViewModel = new()
            {
                Requestclientid = obj.Requestclientid,
                Firstname = obj.Firstname,
                Lastname = obj.Lastname,
                Email = obj.Email,
                Phonenumber = obj.Phonenumber,
                City = obj.City,
                Street = obj.Street,
                State = obj.State,
                Zipcode = obj.Zipcode,
                Room = obj.Address,
                Notes = obj.Notes,
                RequestTypeId = RequestTypeId,
                ConfirmationNumber = req.Confirmationnumber,
            };
            return viewCaseViewModel;
        }

        public ViewNotesViewModel ViewNotes(int ReqId)
        {
            var requestNotes = _db.Requestnotes.Where(x => x.Requestid == ReqId).FirstOrDefault();
            var statuslogs = _db.Requeststatuslogs.Where(x => x.Requestid == ReqId).ToList();
            ViewNotesViewModel model = new ViewNotesViewModel();
            if (model == null)
            {
                model.TransferNotesList = null;
                model.PhysicianNotes = null;
                model.AdminNotes = null;
            }


            if (requestNotes != null)
            {
                model.PhysicianNotes = requestNotes.Physiciannotes;
                model.AdminNotes = requestNotes.Adminnotes;
            }
            if (statuslogs != null)
            {
                model.TransferNotesList = statuslogs;
            }

            return model;
        }

        public CancelCaseModel CancelCase(int reqId)
        {
            var casetags = _db.Casetags.ToList();
            var request = _db.Requests.Where(x => x.Requestid == reqId).FirstOrDefault();
            CancelCaseModel model = new()
            {
                PatientFName = request.Firstname,
                PatientLName = request.Lastname,
                casetaglist = casetags

            };
            return model;
        }

        public bool SubmitCancelCase(CancelCaseModel cancelCaseModel)
        {
            try
            {
                var req = _db.Requests.Where(x => x.Requestid == cancelCaseModel.reqId).FirstOrDefault();
                req.Status = (int)StatusEnum.Cancelled;
                req.Casetag = cancelCaseModel.casetag.ToString();
                var reqStatusLog = _db.Requeststatuslogs.Where(x => x.Requestid == cancelCaseModel.reqId).FirstOrDefault();
                if (reqStatusLog == null)
                {
                    Requeststatuslog rsl = new Requeststatuslog();
                    rsl.Requestid = (int)cancelCaseModel.reqId;
                    rsl.Status = (int)StatusEnum.Cancelled;
                    rsl.Notes = cancelCaseModel.notes;
                    rsl.Createddate = DateTime.Now;
                    _db.Requeststatuslogs.Add(rsl);
                    _db.SaveChanges();
                    return true;
                }
                else
                {
                    reqStatusLog.Status = (int)StatusEnum.Cancelled;
                    reqStatusLog.Notes = cancelCaseModel.notes;
                    _db.Requeststatuslogs.Update(reqStatusLog);
                    _db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public List<Region> GetRegion()
        {
            var region = _db.Regions.ToList();
            return region;
        }
        public JsonArray GetPhysician(int regionId)
        {
            var result = new JsonArray();
            IEnumerable<Physician> physician = _db.Physicians.Where(x => x.Regionid == regionId);

            foreach (Physician item in physician)
            {
                result.Add(new { physicianId = item.Physicianid, physicianName = item.Firstname });
            }
            return result;

        }

        public void AssignCasePostData(AssignCaseModel assignCaseModel, int requestId)
        {
            var reqData = _db.Requests.Where(i => i.Requestid == requestId).FirstOrDefault();

            var reqstatusData = new Requeststatuslog()
            {
                Requestid = requestId,
                Notes = assignCaseModel.additionalNotes,
                Createddate = DateTime.Now,
                Status = 2

            };
            reqData.Status = 2;
            reqData.Physicianid = assignCaseModel.physicanNo;

            _db.Add(reqstatusData);
            _db.SaveChanges();

        }

        public BlockCaseModel BlockCase(int reqId)
        {
            var reqClient = _db.Requestclients.Where(x => x.Requestid == reqId).FirstOrDefault();
            BlockCaseModel model = new()
            {
                ReqId = reqId,
                firstName = reqClient.Firstname,
                lastName = reqClient.Lastname,
                reason = null
            };

            return model;
        }

        public bool SubmitBlockCase(BlockCaseModel blockCaseModel)
        {
            try
            {
                var request = _db.Requests.FirstOrDefault(r => r.Requestid == blockCaseModel.ReqId);
                if (request != null)
                {
                    if (request.Isdeleted == null)
                    {
                        request.Isdeleted = new BitArray(1);
                        request.Isdeleted[0] = true;
                        request.Status = (int)StatusEnum.Clear;
                        request.Modifieddate = DateTime.Now;

                        _db.Requests.Update(request);

                    }
                    Blockrequest blockrequest = new Blockrequest();

                    blockrequest.Phonenumber = request.Phonenumber == null ? "+91" : request.Phonenumber;
                    blockrequest.Email = request.Email;
                    blockrequest.Reason = blockCaseModel.reason;
                    blockrequest.Requestid = (int)blockCaseModel.ReqId;
                    blockrequest.Createddate = DateTime.Now;

                    _db.Blockrequests.Add(blockrequest);
                    _db.SaveChanges();
                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public ViewUploadModel GetAllDocById(int requestId)
        {

            var list = _db.Requestwisefiles.Where(x => x.Requestid == requestId).ToList();
            var reqClient = _db.Requestclients.Where(x => x.Requestid == requestId).FirstOrDefault();

            ViewUploadModel result = new()
            {
                files = list,
                firstName = reqClient.Firstname,
                lastName = reqClient.Lastname,

            };

            return result;

        }

        public bool UploadFiles(List<IFormFile> files, int reqId)
        {

            try
            {
                if (files != null)
                {
                    foreach (IFormFile file in files)
                    {
                        if (file != null && file.Length > 0)
                        {
                            //get file name
                            var fileName = Path.GetFileName(file.FileName);

                            //define path
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles", fileName);

                            // Copy the file to the desired location
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                file.CopyTo(stream)
                       ;
                            }

                            Requestwisefile requestwisefile = new()
                            {
                                Filename = fileName,
                                Requestid = reqId,
                                Createddate = DateTime.Now
                            };

                            _db.Requestwisefiles.Add(requestwisefile);

                        }
                    }
                    _db.SaveChanges();
                    return true;
                }
                else { return false; }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteFileById(int reqFileId)
        {
            try
            {
                var reqWiseFile = _db.Requestwisefiles.Where(x => x.Requestwisefileid == reqFileId).FirstOrDefault();
                if (reqWiseFile != null)
                {
                    _db.Requestwisefiles.Remove(reqWiseFile);
                    _db.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteAllFiles(List<string> filenames, int reqId)
        {
            try
            {
                var list = _db.Requestwisefiles.Where(x => x.Requestid == reqId).ToList();

                foreach (var filename in filenames)
                {
                    var existFile = list.Where(x => x.Filename == filename && x.Requestid == reqId).FirstOrDefault();
                    if (existFile != null)
                    {
                        list.Remove(existFile);
                        _db.Requestwisefiles.Remove(existFile);
                    }
                }
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //public List<Healthprofessionaltype> GetProfession()
        //{
        //    var profession = _db.Healthprofessionaltypes.ToList();
        //    return profession;
        //}

        //public List<Healthprofessional> GetBusiness()
        //{
        //    var business = _db.Healthprofessionals.ToList();
        //    return business;
        //}

        public Order FetchOrder(int reqId)
        {
            var reqclientid = _db.Requests.Where(x => x.Requestid == reqId).FirstOrDefault();
            var Healthprofessional = _db.Healthprofessionals.ToList();
            var Healthprofessionaltype = _db.Healthprofessionaltypes.ToList();

            Order order = new()
            {
                Profession = Healthprofessionaltype,
                Business = Healthprofessional,
            };
            return order;
        }
        public JsonArray FetchVendors(int selectedValue)
        {
            var result = new JsonArray();
            IEnumerable<Healthprofessional> businesses = _db.Healthprofessionals.Where(prof => prof.Profession == selectedValue);

            foreach (Healthprofessional business in businesses)
            {
                result.Add(new { businessId = business.Vendorid, businessName = business.Vendorname });
            }
            return result;
        }

        public Healthprofessional VendorDetails(int selectedValue)
        {
            Healthprofessional business = _db.Healthprofessionals.First(prof => prof.Vendorid == selectedValue);

            return business;
        }

        public async Task SendOrderDetails(Order order)
        {
            Orderdetail orderDetail = new Orderdetail()
            {
                Vendorid = order.vendorid,
                Requestid = order.ReqId,
                Faxnumber = order.faxnumber,
                Email = order.email,
                Businesscontact = order.BusineesContact,
                Prescription = order.orderdetail,
                Noofrefill = order.refill,
                Createddate = DateTime.Now,
                Createdby = "admin",
            };
            await _db.Orderdetails.AddAsync(orderDetail);
            await _db.SaveChangesAsync();
        }


        public void TransferReqPostData(AssignCaseModel assignCaseModel, int requestId)
        {
            var reqData = _db.Requests.Where(i => i.Requestid == requestId).FirstOrDefault();

            reqData.Status = (int)StatusEnum.Accepted;
            reqData.Physicianid = assignCaseModel.physicanNo;

            Requeststatuslog requeststatuslog = new Requeststatuslog()
            {
                Requestid = requestId,
                Notes = assignCaseModel.additionalNotes,
                Createddate = DateTime.Now,
                Status = (int)StatusEnum.Accepted,
            };
            _db.Requests.Update(reqData);
            _db.Requeststatuslogs.Add(requeststatuslog);
            _db.SaveChanges();

        }

        public bool Clearcase(int requestId)
        {
            try
            {
                var req = _db.Requests.Where(x => x.Requestid == requestId).FirstOrDefault();

                if (req != null)
                {

                    req.Status = (int)StatusEnum.Clear;
                    _db.Requests.Update(req);
                    _db.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public SendAgreement Agreement(int reqClientId)
        {
            var obj = _db.Requestclients.FirstOrDefault(x => x.Requestclientid == reqClientId);

            SendAgreement sendAgreement = new()
            {
                phonenumber = obj.Phonenumber,
                ReqClientId = reqClientId,
                email = obj.Email,
            };
            return sendAgreement;
        }

        public void SendAgreementEmail(SendAgreement model, string link)
        {
            Requestclient reqCli = _db.Requestclients.FirstOrDefault(requestCli => requestCli.Requestclientid == model.ReqClientId);

            string mail = "tatva.dotnet.yashvariya@outlook.com";
            string password = "Itzvariya@23";

            SmtpClient client = new("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(mail, password),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            MailMessage mailMessage = new()
            {
                From = new MailAddress(mail, "HalloDoc"),
                Subject = "Hallodoc review agreement",
                IsBodyHtml = true,
                Body = "<h3>Admin has sent you the agreement papers to review. Click on the link below to read the agreement.</h3><a href=\"" + link + "\">Review Agreement link</a>",
            };

            mailMessage.To.Add(model.email);

            client.Send(mailMessage);
        }

        public bool ReviewAgree(ReviewAgreement Agreement)
        {
            try
            {
                var reqClient = _db.Requestclients.Where(x => x.Requestclientid == Agreement.ReqClientId).FirstOrDefault();
                var req = _db.Requests.FirstOrDefault(x => x.Requestid == reqClient.Requestid);
                if (req != null)
                {
                    req.Status = (int)StatusEnum.MDEnRoute;

                    Requeststatuslog requeststatuslog = new Requeststatuslog();
                    requeststatuslog.Requestid = req.Requestid;
                    requeststatuslog.Status = req.Status;
                    requeststatuslog.Createddate = DateTime.Now;

                    _db.Requests.Update(req);
                    _db.Requeststatuslogs.Add(requeststatuslog);
                    _db.SaveChanges();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public CancelAngreement CancelAgreement(int requestClientId)
        {
            Requestclient reqClient = _db.Requestclients.Where(x => x.Requestclientid == requestClientId).FirstOrDefault();
            Request request = _db.Requests.FirstOrDefault(x => x.Requestid == reqClient.Requestid);
            CancelAngreement obj = new()
            {
                ReqClientId = requestClientId,
                Firstname = request.Firstname + " " + request.Lastname,
            };
            return obj;
        }

        public bool CancelAgreement(CancelAngreement cancel)
        {
            try
            {
                Requestclient reqClient = _db.Requestclients.Where(x => x.Requestclientid == cancel.ReqClientId).FirstOrDefault();

                if (reqClient != null)
                {
                    Request request = _db.Requests.FirstOrDefault(x => x.Requestid == reqClient.Requestid);
                    request.Status = (int)StatusEnum.CancelledByPatient;

                    Requeststatuslog requeststatuslog = new Requeststatuslog();
                    requeststatuslog.Requestid = request.Requestid;
                    requeststatuslog.Status = request.Status;
                    requeststatuslog.Notes = cancel.Reason;
                    requeststatuslog.Createddate = DateTime.Now;

                    _db.Requests.Update(request);
                    _db.Requeststatuslogs.Add(requeststatuslog);
                    _db.SaveChanges();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public CloseCase closeCase(int reqId)
        {

            var requestclient = _db.Requestclients.FirstOrDefault(x => x.Requestid == reqId);
            var requestwisefile = _db.Requestwisefiles.Where(x => x.Requestid == reqId).ToList();

            CloseCase obj = new()
            {
                ReqId = reqId,
                Firstname = requestclient.Firstname,
                Lastname = requestclient.Lastname,
                email = requestclient.Email,
                phoneno = requestclient.Phonenumber,
                file = requestwisefile,

            };
            return obj;
        }

        public bool EditCloseCase(CloseCase closeCase)
        {
            try
            {
                var requestclient = _db.Requestclients.FirstOrDefault(x => x.Requestid == closeCase.ReqId);
                requestclient.Phonenumber = closeCase.phoneno;
                requestclient.Email = closeCase.email;
                _db.Requestclients.Update(requestclient);
                _db.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ChangeCloseCase(CloseCase closeCase)
        {
            try
            {
                Request req = _db.Requests.FirstOrDefault(x => x.Requestid == closeCase.ReqId);


                req.Status = (int)StatusEnum.Unpaid;
                _db.Requests.Update(req);
                _db.SaveChanges();
                return true;
            }
            catch { return false; }
        }

        public EncounterFormModel EncounterForm(int reqId)
        {
            var reqClient = _db.Requestclients.FirstOrDefault(x => x.Requestid == reqId);
            var encForm = _db.Encounterforms.FirstOrDefault(x => x.Requestid == reqId);
            EncounterFormModel ef = new EncounterFormModel();
            ef.reqid = reqId;
            ef.FirstName = reqClient.Firstname;
            ef.LastName = reqClient.Lastname;
            ef.Location = reqClient.Street + reqClient.City + reqClient.State + reqClient.Zipcode;
            //ef.BirthDate = new DateTime((int)(reqClient.Intyear), Convert.ToInt16(reqClient.Strmonth), (int)(reqClient.Intdate)).ToString("yyyy-MM-dd");
            ef.PhoneNumber = reqClient.Phonenumber;
            ef.Email = reqClient.Email;
            if (encForm != null)
            {
                ef.HistoryIllness = encForm.Illnesshistory;
                ef.MedicalHistory = encForm.Medicalhistory;
                //ef.Date = new DateTime((int)(encForm.Intyear), Convert.ToInt16(encForm.Strmonth), (int)(encForm.Intdate)).ToString("yyyy-MM-dd");
                ef.Medications = encForm.Medications;
                ef.Allergies = encForm.Allergies;
                ef.Temp = encForm.Temperature;
                ef.Hr = encForm.Heartrate;
                ef.Rr = encForm.Respirationrate;
                ef.BpS = encForm.Bloodpressuresystolic;
                ef.BpD = encForm.Bloodpressurediastolic;
                ef.O2 = encForm.Oxygenlevel;
                ef.Pain = encForm.Pain;
                ef.Heent = encForm.Heent;
                ef.Cv = encForm.Cardiovascular;
                ef.Chest = encForm.Chest;
                ef.Abd = encForm.Abdomen;
                ef.Extr = encForm.Extremities;
                ef.Skin = encForm.Skin;
                ef.Neuro = encForm.Neuro;
                ef.Other = encForm.Other;
                ef.Diagnosis = encForm.Diagnosis;
                ef.TreatmentPlan = encForm.Treatmentplan;
                ef.MedicationDispensed = encForm.Medicationsdispensed;
                ef.Procedures = encForm.Procedures;
                ef.FollowUp = encForm.Followup;
            }
            return ef;
        }

        public bool SubmitEncounterForm(EncounterFormModel model)
        {
            try
            {
                //concludeEncounter _obj = new concludeEncounter();

                var ef = _db.Encounterforms.FirstOrDefault(r => r.Requestid == model.reqid);

                if (ef == null)
                {
                    Encounterform _encounter = new Encounterform()
                    {
                        Requestid = model.reqid,
                        Firstname = model.FirstName,
                        Lastname = model.LastName,
                        Location = model.Location,
                        Phonenumber = model.PhoneNumber,
                        Email = model.Email,
                        Illnesshistory = model.HistoryIllness,
                        Medicalhistory = model.MedicalHistory,
                        //date = model.Date,
                        Medications = model.Medications,
                        Allergies = model.Allergies,
                        Temperature = model.Temp,
                        Heartrate = model.Hr,
                        Respirationrate = model.Rr,
                        Bloodpressuresystolic = model.BpS,
                        Bloodpressurediastolic = model.BpD,
                        Oxygenlevel = model.O2,
                        Pain = model.Pain,
                        Heent = model.Heent,
                        Cardiovascular = model.Cv,
                        Chest = model.Chest,
                        Abdomen = model.Abd,
                        Extremities = model.Extr,
                        Skin = model.Skin,
                        Neuro = model.Neuro,
                        Other = model.Other,
                        Diagnosis = model.Diagnosis,
                        Treatmentplan = model.TreatmentPlan,
                        Medicationsdispensed = model.MedicationDispensed,
                        Procedures = model.Procedures,
                        Followup = model.FollowUp,
                        Isfinalized = false
                    };

                    _db.Encounterforms.Add(_encounter);

                    //_obj.indicate = true;
                }
                else
                {
                    var efdetail = _db.Encounterforms.FirstOrDefault(x => x.Requestid == model.reqid);

                    efdetail.Requestid = model.reqid;
                    efdetail.Illnesshistory = model.HistoryIllness;
                    efdetail.Medicalhistory = model.MedicalHistory;
                    //efdetail.Date = model.Date;
                    efdetail.Medications = model.Medications;
                    efdetail.Allergies = model.Allergies;
                    efdetail.Temperature = model.Temp;
                    efdetail.Heartrate = model.Hr;
                    efdetail.Respirationrate = model.Rr;
                    efdetail.Bloodpressuresystolic = model.BpS;
                    efdetail.Bloodpressurediastolic = model.BpD;
                    efdetail.Oxygenlevel = model.O2;
                    efdetail.Pain = model.Pain;
                    efdetail.Heent = model.Heent;
                    efdetail.Cardiovascular = model.Cv;
                    efdetail.Chest = model.Chest;
                    efdetail.Abdomen = model.Abd;
                    efdetail.Extremities = model.Extr;
                    efdetail.Skin = model.Skin;
                    efdetail.Neuro = model.Neuro;
                    efdetail.Other = model.Other;
                    efdetail.Diagnosis = model.Diagnosis;
                    efdetail.Treatmentplan = model.TreatmentPlan;
                    efdetail.Medicationsdispensed = model.MedicationDispensed;
                    efdetail.Procedures = model.Procedures;
                    efdetail.Followup = model.FollowUp;
                    efdetail.Modifieddate = DateTime.Now;
                    ef.Isfinalized = false;
                    _db.Encounterforms.Update(efdetail);
                    // _obj.indicate = true;
                };


                _db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool CreateRequest(CreateRequestModel model, string sessionEmail)
        {
            try
            {
                CreateRequestModel _create = new CreateRequestModel();

                var stateMain = _db.Regions.Where(r => r.Name.ToLower() == model.state.ToLower().Trim()).FirstOrDefault();

                if (stateMain == null)
                {
                    return false;
                }
                else
                {
                    Request _req = new Request();
                    Requestclient _reqClient = new Requestclient();
                    User _user = new User();
                    Aspnetuser _asp = new Aspnetuser();
                    Requestnote _note = new Requestnote();

                    var admin = _db.Admins.Where(r => r.Email == sessionEmail).Select(r => r).First();

                    var existUser = _db.Aspnetusers.FirstOrDefault(r => r.Email == model.email);

                    if (existUser == null)
                    {
                        _asp.Username = model.firstname + "_" + model.lastname;
                        _asp.Email = model.email;
                        _asp.Phonenumber = model.phone;
                        _asp.Createddate = DateTime.Now;
                        _db.Aspnetusers.Add(_asp);
                        _db.SaveChanges();

                        _user.Aspnetuserid = _asp.Id;
                        _user.Firstname = model.firstname;
                        _user.Lastname = model.lastname;
                        _user.Email = model.email;
                        _user.Mobile = model.phone;
                        _user.City = model.city;
                        _user.State = model.state;
                        _user.Street = model.street;
                        _user.Zipcode = model.zipcode;
                        _user.Strmonth = model.dateofbirth.Substring(5, 2);
                        _user.Intdate = Convert.ToInt16(model.dateofbirth.Substring(0, 4));
                        _user.Intyear = Convert.ToInt16(model.dateofbirth.Substring(8, 2));
                        _user.Createdby = _asp.Id;
                        _user.Createddate = DateTime.Now;
                        _user.Regionid = _db.Regions.Where(r => r.Name.ToLower() == model.state.ToLower()).Select(r => r.Regionid).FirstOrDefault();
                        _db.Users.Add(_user);
                        _db.SaveChanges();

                        string registrationLink = "http://localhost:5145/Home/CreateAccount?aspuserId=" + _asp.Id;

                        try
                        {
                            //SendRegistrationEmailCreateRequest(data.email, registrationLink);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }

                    _req.Requesttypeid = 1;
                    _req.Userid = Convert.ToInt32(admin.Aspnetuserid);
                    _req.Firstname = admin.Firstname;
                    _req.Lastname = admin.Lastname;
                    _req.Phonenumber = admin.Mobile;
                    _req.Email = admin.Email;
                    _req.Status = 1;
                    _req.Confirmationnumber = admin.Firstname.Substring(0, 1) + DateTime.Now.ToString().Substring(0, 19);
                    _req.Createddate = DateTime.Now;

                    _db.Requests.Add(_req);
                    _db.SaveChanges();



                    _reqClient.Requestid = _req.Requestid;
                    _reqClient.Firstname = model.firstname;
                    _reqClient.Lastname = model.lastname;
                    _reqClient.Phonenumber = model.phone;
                    _reqClient.Strmonth = model.dateofbirth.Substring(5, 2);
                    _reqClient.Intdate = Convert.ToInt16(model.dateofbirth.Substring(8, 2));
                    _reqClient.Intyear = Convert.ToInt16(model.dateofbirth.Substring(0, 4));
                    _reqClient.Street = model.street;
                    _reqClient.City = model.city;
                    _reqClient.State = model.state;
                    _reqClient.Zipcode = model.zipcode;
                    _reqClient.Regionid = _db.Regions.Where(r => r.Name.ToLower() == model.state.ToLower()).Select(r => r.Regionid).FirstOrDefault();
                    _reqClient.Email = model.email;

                    _db.Requestclients.Add(_reqClient);
                    _db.SaveChanges();

                    _note.Requestid = _req.Requestid;
                    _note.Adminnotes = model.admin_notes;
                    _note.Createdby = _db.Aspnetusers.Where(r => r.Email == model.email).Select(r => r.Id).FirstOrDefault();
                    _note.Createddate = DateTime.Now;
                    _db.Requestnotes.Add(_note);
                    _db.SaveChanges();


                }
                return true;
            }
            catch
            {
                return false;
            }



        }

        //public AdminProfile ProfileInfo(int adminId)
        //{
        //    Admin? obj = _db.Admins.FirstOrDefault(x => x.Adminid == adminId);

        //    var region = _db.Regions.FirstOrDefault(x => x.Regionid == obj.Regionid).Name;
        //    var regionList = _db.Regions.ToList();

        //    AdminProfile profile = new()
        //    {
        //        UserName = obj.Firstname + obj.Lastname,
        //        AdminId = adminId.ToString(),
        //        //AdminPassword=obj.,
        //        Status = obj.Status,
        //        Role = obj.Roleid.ToString() ?? "",
        //        FirstName = obj.Firstname,
        //        LastName = obj.Lastname,
        //        AdminPhone = obj.Mobile,
        //        Email = obj.Email,
        //        ConfirmEmail = obj.Email,
        //        Address1 = obj.Address1,
        //        Address2 = obj.Address2,
        //        City = region,
        //        State = region,
        //        Zip = obj.Zip,
        //        BillingPhone = obj.Altphone,
        //        RegionList = regionList,
        //    };

        //    return profile;
        //}

        //public MyProfileModel MyProfile(string sessionEmail)
        //{
        //    var myProfileMain = _db.Admins.Where(x => x.Email == sessionEmail).Select(x => new MyProfileModel()
        //    {
        //        fname = x.Firstname,
        //        lname = x.Lastname,
        //        email = x.Email,
        //        confirm_email = x.Email,
        //        mobile_no = x.Mobile,
        //        addr1 = x.Address1,
        //        addr2 = x.Address2,
        //        city = x.City,
        //        zip = x.Zip,
        //        state = _db.Regions.Where(r => r.Regionid == x.Regionid).Select(r => r.Name).First(),
        //        roles = _db.Aspnetroles.ToList(),
        //    }).ToList().FirstOrDefault();

        //    var aspnetuser = _db.Aspnetusers.Where(r => r.Email == sessionEmail).First();



        //    myProfileMain.username = aspnetuser.Username;
        //    myProfileMain.password = aspnetuser.Passwordhash;

        //    return myProfileMain;
        //}

        public bool VerifyState(string state)
        {

            var stateMain = _db.Regions.Where(r => r.Name.ToLower() == state.ToLower().Trim()).FirstOrDefault();

            if (stateMain == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public List<ProviderModel> GetProvider()
        {
            var provider = from phy in _db.Physicians
                           join role in _db.Roles on phy.Roleid equals role.Roleid
                           join phynoti in _db.Physiciannotifications on phy.Physicianid equals phynoti.Pysicianid
                           orderby phy.Physicianid
                           select new ProviderModel
                           {
                               phyId = phy.Physicianid,
                               firstName = phy.Firstname,
                               lastName = phy.Lastname,
                               status = phy.Status.ToString(),
                               role = role.Name,
                               onCallStatus = "un available",
                               notification = phynoti.Isnotificationstopped[0],
                           };
            var result = provider.ToList();

            return result;
        }

        public bool StopNotification(int PhysicianId)
        {

            var phyNotification = _db.Physiciannotifications.Where(r => r.Pysicianid == PhysicianId).Select(r => r).First();

            var notification = new BitArray(1);
            notification[0] = false;

            if (phyNotification.Isnotificationstopped[0] == notification[0])
            {
                phyNotification.Isnotificationstopped = new BitArray(1);
                phyNotification.Isnotificationstopped[0] = true;
                _db.Physiciannotifications.Update(phyNotification);
                _db.SaveChanges();

                return true;
            }
            else
            {
                phyNotification.Isnotificationstopped = new BitArray(1);
                phyNotification.Isnotificationstopped[0] = false;
                _db.Physiciannotifications.Update(phyNotification);
                _db.SaveChanges();

                return false;
            }
        }


        public ProviderModel providerContact(int PhysicianId)
        {
            ProviderModel provider = new ProviderModel()
            {
                phyId = PhysicianId,
            };

            return provider;
        }
        public void providerContactEmail(int phyIdMain, string msg)
        {
            ProviderModel _provider = new ProviderModel();

            _provider.phyId = phyIdMain;

            var provider = _db.Physicians.FirstOrDefault(x => x.Physicianid == phyIdMain);

            try
            {
                SendRegistrationproviderContactEmail(provider.Email, msg, phyIdMain);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public void SendRegistrationproviderContactEmail(string provider, string msg, int phyIdMain)
        {
            string senderEmail = "tatva.dotnet.yashvariya@outlook.com";
            string senderPassword = "Itzvariya@23";
            SmtpClient client = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, "HalloDoc"),
                Subject = "Mail For provider",
                IsBodyHtml = true,
                Body = $"{msg}",
            };


            mailMessage.To.Add(provider);

            client.Send(mailMessage);

            Emaillog emailLog = new Emaillog()
            {
                Subjectname = mailMessage.Subject,
                Emailtemplate = "Sender : " + senderEmail + "Reciver :" + provider + "Subject : " + mailMessage.Subject + "Message : " + msg,
                Emailid = provider,
                Roleid = 1,
                Adminid = _db.Admins.Where(r => r.Email == provider).Select(r => r.Adminid).First(),
                Physicianid = phyIdMain,
                Createdate = DateTime.Now,
                Sentdate = DateTime.Now,
                Isemailsent = new BitArray(1, true),

            };

            _db.Emaillogs.Add(emailLog);
            _db.SaveChanges();
        }

        

        public EditPhysicianAccount EditPhysician(int PhysicianId)
        {
            var physician = _db.Physicians.FirstOrDefault(x => x.Physicianid == PhysicianId);
            var aspnetuser = _db.Aspnetusers.FirstOrDefault(x => x.Id == physician.Aspnetuserid);
            var region = _db.Regions.FirstOrDefault(x => x.Regionid == physician.Regionid).Name;
            EditPhysicianAccount obj = new()
            {
                PhysicianId = PhysicianId,
                UserName = aspnetuser.Username,
                Password = aspnetuser.Passwordhash,
                FirstName = physician.Firstname,
                LastName = physician.Lastname,
                Email = physician.Email,
                PhoneNumber = physician.Mobile,
                MedicalLicenseNumber = physician.Medicallicense,
                NPINumber = physician.Npinumber,
                SyncEmail = physician.Syncemailaddress,
                Address1 = physician.Address1,
                Address2 = physician.Address2,
                City = physician.City,
                State = region,
                Zip = physician.Zip,
                Phone = physician.Altphone,
                BusinessName = physician.Businessname,
                BusinessWebsite = physician.Businesswebsite,
                RegionList = _db.Regions.ToList(),
                
            };

            return obj;
        }

        //public bool EditSavePhysician(EditPhysicianAccount editPhysicianAccount)
        //{
        //    try
        //    {
                
        //        var physician = _db.Physicians.FirstOrDefault(x => x.Physicianid == editPhysicianAccount.PhysicianId);
        //        //var aspnetuser = _db.Aspnetusers.FirstOrDefault(x => x.Id == physician.Aspnetuserid);
        //        //var region = _db.Regions.Where(x => x.Regionid == physician.Regionid).Select(x=>x.Name).First();
        //        switch (editPhysicianAccount.FormId)
        //        {
        //            case 1:
        //                break;
        //            case 2:
        //                physician.Firstname = editPhysicianAccount.FirstName;
        //                physician.Lastname = editPhysicianAccount.LastName;
        //                physician.Email = editPhysicianAccount.Email;
        //                physician.Mobile = editPhysicianAccount.Phone;
        //                physician.Medicallicense = editPhysicianAccount.MedicalLicenseNumber;
        //                physician.Npinumber = editPhysicianAccount.NPINumber;
        //                physician.Syncemailaddress = editPhysicianAccount.SyncEmail;
        //                _db.Physicians.Update(physician);
        //                _db.SaveChanges();
        //                break;
        //            case 3:
        //                physician.Address1 = editPhysicianAccount.Address1;
        //                physician.Address2 = editPhysicianAccount.Address2;
        //                physician.City = editPhysicianAccount.City;
        //                physician.Zip = editPhysicianAccount.Zip;
        //                physician.Altphone = editPhysicianAccount.Phone;
        //                _db.Physicians.Update(physician);
        //                _db.SaveChanges();
        //                break;
        //            case 4:
        //                physician.Businessname = editPhysicianAccount.BusinessName;
        //                physician.Businesswebsite = editPhysicianAccount.BusinessWebsite;
        //                _db.Physicians.Update(physician);
        //                _db.SaveChanges();
        //                break;
        //            case 5:
        //                break;
        //        }

        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
            

        //}

        public List<Physicianlocation> GetPhysicianlocations()
        {
            var phyLocation = _db.Physicianlocations.ToList();
            return phyLocation;
        }

        public MyProfileModel MyProfile(string sessionEmail)
        {
            var myProfileMain = _db.Admins.Where(x => x.Email == sessionEmail).Select(x => new MyProfileModel()
            {
                fname = x.Firstname,
                lname = x.Lastname,
                email = x.Email,
                confirm_email = x.Email,
                mobile_no = x.Mobile,
                addr1 = x.Address1,
                addr2 = x.Address2,
                city = x.City,
                zip = x.Zip,
                state = _db.Regions.Where(r => r.Regionid == x.Regionid).Select(r => r.Name).First(),
                roles = _db.Aspnetroles.ToList(),
            }).ToList().FirstOrDefault();

            var aspnetuser = _db.Aspnetusers.Where(r => r.Email == sessionEmail).First();



            myProfileMain.username = aspnetuser.Username;
            //myProfileMain.password = aspnetuser.Passwordhash;

            return myProfileMain;
        }

        public bool ResetPassword(string tokenEmail, string resetPassword)
        {
            try
            {
                var aspUser = _db.Aspnetusers.Where(r => r.Email == tokenEmail).Select(r => r).First();

                if (aspUser.Passwordhash != resetPassword)
                {
                    aspUser.Passwordhash = resetPassword;
                    _db.Aspnetusers.Update(aspUser);

                    _db.SaveChanges();

                    return true;
                }
                return false;



            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool SubmitAdminInfo(MyProfileModel model, string tokenEmail)
        {
            try
            {

                var aspUser = _db.Aspnetusers.Where(r => r.Email == tokenEmail).Select(r => r).First();

                var adminInfo = _db.Admins.Where(r => r.Email == tokenEmail).Select(r => r).First();

                if (adminInfo.Firstname != model.fname || adminInfo.Lastname != model.lname || adminInfo.Email != model.email || adminInfo.Mobile != model.mobile_no)
                {
                    if (adminInfo.Firstname != model.fname)
                    {

                        adminInfo.Firstname = model.fname;

                    }

                    if (adminInfo.Lastname != model.lname)
                    {
                        adminInfo.Lastname = model.lname;
                    }

                    if (adminInfo.Email != model.email)
                    {
                        adminInfo.Email = model.email;
                        aspUser.Email = model.email;

                        int index = model.email.IndexOf('@');
                        var username = model.email.Substring(0, index);
                        aspUser.Username = username;
                    }

                    if (adminInfo.Mobile != model.mobile_no)
                    {
                        adminInfo.Mobile = model.mobile_no;
                        aspUser.Phonenumber = model.mobile_no;
                    }


                    aspUser.Modifieddate = DateTime.Now;

                    _db.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SubmitBillingInfo(MyProfileModel model, string tokenEmail)
        {
            try
            {
                var adminInfo = _db.Admins.Where(r => r.Email == tokenEmail).Select(r => r).First();

                var regionid = _db.Regions.Where(x => x.Name.ToLower() == model.state.ToLower()).Select(x => x.Regionid).First();

                if (adminInfo.Address1 != model.addr1 || adminInfo.Address2 != model.addr2 || adminInfo.City != model.city || adminInfo.Zip != model.zip || adminInfo.Regionid != regionid)
                {

                    if (adminInfo.Address1 != model.addr1)
                    {
                        adminInfo.Address1 = model.addr1;
                    }

                    if (adminInfo.Address2 != model.addr2)
                    {
                        adminInfo.Address2 = model.addr2;
                    }

                    if (adminInfo.City != model.city)
                    {
                        adminInfo.City = model.city;
                    }

                    if (adminInfo.Zip != model.zip)
                    {
                        adminInfo.Zip = model.zip;
                    }

                    if (adminInfo.Regionid != regionid)
                    {
                        adminInfo.Regionid = regionid;
                    }
                    if (adminInfo.Altphone != model.altphone)
                    {
                        adminInfo.Altphone = model.altphone;
                    }
                    _db.SaveChanges();

                    return true;
                }

                return false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public EditProviderModel EditProviderProfile(int phyId, string tokenEmail)
        {
            var phy = _db.Physicians.Where(r => r.Physicianid == phyId).Select(r => r).First();

            var user = _db.Aspnetusers.Where(r => r.Email == phy.Email).First();

            EditProviderModel _profile = new EditProviderModel()
            {
                //username = _context.Aspnetusers.Where(r => r.Email == sessionEmail).Select(r => r.Username).First(),
                Firstname = phy.Firstname,
                Lastname = phy.Lastname,
                Email = phy.Email,
                PhoneNumber = phy.Mobile,
                MedicalLicesnse = phy.Medicallicense,
                NPInumber = phy.Npinumber,
                SycnEmail = phy.Syncemailaddress,
                Address1 = phy.Address1,
                Address2 = phy.Address2,
                city = phy.City,
                zipcode = phy.Zip,
                altPhone = phy.Altphone,
                Businessname = phy.Businessname,
                BusinessWebsite = phy.Businesswebsite,
                Adminnotes = phy.Adminnotes,
                statusId = (int)phy.Status,
                PhyID = phyId,
                Roleid = phy.Roleid,
                Regionid = phy.Regionid,
                PhotoValue = phy.Photo,
                SignatureValue = phy.Signature,
                IsContractorAgreement = phy.Isagreementdoc == null ? false : true,
                IsBackgroundCheck = phy.Isbackgrounddoc == null ? false : true,
                IsHIPAA = phy.Istrainingdoc == null ? false : true,
                IsNonDisclosure = phy.Isnondisclosuredoc == null ? false : true,
                IsLicenseDocument = phy.Islicensedoc == null ? false : true,


                username = user.Username,
                password = user.Passwordhash,
            };

            return _profile;
        }

        public List<Region> RegionTable()
        {
            var region = _db.Regions.ToList();
            return region;
        }

        public List<Role> GetRoles()
        {
            BitArray deletedBit = new BitArray(new[] { false });
            var roles = _db.Roles.Where(x => x.Isdeleted.Equals(deletedBit)).ToList();
            return roles;
        }

        public List<PhysicianRegionTable> PhyRegionTable(int phyId)
        {
            var region = _db.Regions.ToList();
            var phyRegion = _db.Physicianregions.ToList();

            var checkedRegion = region.Select(r1 => new PhysicianRegionTable
            {
                Regionid = r1.Regionid,
                Name = r1.Name,
                ExistsInTable = phyRegion.Any(r2 => r2.Physicianid == phyId && r2.Regionid == r1.Regionid),
            }).ToList();

            return checkedRegion;
        }

        public bool providerResetPass(string email, string password)
        {
            var resetPass = _db.Aspnetusers.Where(r => r.Email == email).Select(r => r).First();

            if (resetPass.Passwordhash != password)
            {
                resetPass.Passwordhash = password;
                _db.SaveChanges();

                return true;
            }
            return false;

        }

        public List<AccountAccess> AccountAccess()
        {
            var obj = (from role in _db.Roles
                       where role.Isdeleted != new BitArray(1, true)
                       select new AccountAccess
                       {
                           Name = role.Name,
                           RoleId = role.Roleid,
                           AccountType = role.Accounttype,
                       }).ToList();
            return obj;
        }

        public bool DeleteRole(int roleId)
        {
            try
            {
                var role = _db.Roles.FirstOrDefault(x => x.Roleid == roleId);
                role.Isdeleted = new BitArray(1, true);
                _db.Roles.Update(role);
                _db.SaveChanges();

                var rolemenu = _db.Rolemenus.Where(x => x.Roleid == roleId).ToList();

                foreach (var item in rolemenu)
                {
                    _db.Rolemenus.Remove(item);
                }
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public CreateAccess FetchRole(short selectedValue)
        {
            if (selectedValue == 0)
            {
                CreateAccess obj = new()
                {
                    Menu = _db.Menus.ToList(),
                };
                return obj;
            }
            else if (selectedValue == 1 || selectedValue == 2)
            {

                CreateAccess obj = new()
                {
                    Menu = _db.Menus.Where(x => x.Accounttype == selectedValue).ToList(),
                };
                return obj;
            }
            else
            {
                CreateAccess obj = new();
                return obj;
            }
        }
        public bool RoleExists(string roleName, short accountType)
        {
            BitArray deletedBit = new BitArray(new[] { false });
            var isRoleExists = _db.Roles.Where(x => (x.Name.ToLower() == roleName.Trim().ToLower() && x.Accounttype == accountType) && (x.Isdeleted.Equals(deletedBit))).Any();
            if (isRoleExists)
            {
                return true;
            }
            return false;
        }
        public bool CreateRole(List<int> menuIds, string roleName, short accountType)
        {
            try
            {
                Role role = new()
                {
                    Name = roleName,
                    Accounttype = accountType,
                    Createdby = "Admin",
                    Createddate = DateTime.Now,
                    Isdeleted = new BitArray(1, false),
                };
                _db.Roles.Add(role);
                _db.SaveChanges();

                foreach (int menuId in menuIds)
                {
                    Rolemenu rolemenu = new()
                    {
                        Roleid = role.Roleid,
                        Menuid = menuId,
                    };
                    _db.Rolemenus.Add(rolemenu);
                };
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public CreateAdminAccount RegionList()
        {
            CreateAdminAccount obj = new()
            {
                RegionList = _db.Regions.ToList(),
            };
            return obj;
        }

        public bool CreateAdminAccount(CreateAdminAccount obj, string email)
        {
            var emailExists = _db.Aspnetusers.Where(x => x.Email == obj.Email).Any();
            if (emailExists)
            {
                return false;
            }
            else
            {
                Guid id = Guid.NewGuid();
                Aspnetuser aspnetuser = new()
                {
                    Id = id.ToString(),
                    Username = obj.UserName,
                    Passwordhash = obj.AdminPassword,
                    Email = obj.Email,
                    Phonenumber = obj.AdminPhone,
                    Createddate = DateTime.Now,


                };
                _db.Aspnetusers.Add(aspnetuser);
                _db.SaveChanges();

                var aspnetId = _db.Aspnetusers.Where(x => x.Email == email).Select(x => x.Id).First();
                Admin admin = new Admin();


                admin.Aspnetuserid = aspnetuser.Id;
                admin.Firstname = obj.FirstName;
                admin.Lastname = obj.LastName;
                admin.Email = obj.Email;

                admin.Mobile = obj.AdminPhone;
                admin.Address1 = obj.Address1;

                admin.Address2 = obj.Address2;
                admin.Zip = obj.Zip;
                admin.Altphone = obj.BillingPhone;
                admin.Createdby = aspnetId;
                admin.Createddate = DateTime.Now;
                admin.Isdeleted = new BitArray(1, false);


                _db.Admins.Add(admin);
                _db.SaveChanges();




                var AdminRegions = obj.AdminRegion.ToList();
                for (int i = 0; i < AdminRegions.Count; i++)
                {
                    Adminregion adminregion = new()
                    {
                        Adminid = admin.Adminid,
                        Regionid = _db.Regions.First(x => x.Regionid == AdminRegions[i]).Regionid,
                    };

                    _db.Adminregions.Add(adminregion);
                    _db.SaveChanges();
                }

                return true;

            }


        }

        public void InsertFileAfterRename(IFormFile file, string path, string updateName)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string[] oldfiles = Directory.GetFiles(path, updateName + ".*");
            foreach (string f in oldfiles)
            {
                System.IO.File.Delete(f);
            }

            string extension = Path.GetExtension(file.FileName);

            string fileName = updateName + extension;

            string fullPath = Path.Combine(path, fileName);

            using FileStream stream = new(fullPath, FileMode.Create);
            file.CopyTo(stream);
        }
        public CreateProviderAccount GetProviderList()
        {
            CreateProviderAccount obj = new()
            {
                RolesList = _db.Roles.ToList(),
                RegionList = _db.Regions.ToList(),
            };
            return obj;
        }
        
        public void CreateProviderAccount(CreateProviderAccount model,string loginId)
        {
            List<string> validProfileExtensions = new() { ".jpeg", ".png", ".jpg" };
            List<string> validDocumentExtensions = new() { ".pdf" };

            try
            {
                Guid generatedId = Guid.NewGuid();

                Aspnetuser aspUser = new()
                {
                    Id = generatedId.ToString(),
                    Username = model.UserName,
                    Passwordhash = GenerateSHA256(model.Password),
                    Email = model.Email,
                    Phonenumber = model.Phone,
                    Createddate = DateTime.Now,
                }; _db.Aspnetusers.Add(aspUser);
                _db.SaveChanges();


                Physician phy = new()
                {
                    Aspnetuserid = generatedId.ToString(),
                    Firstname = model.FirstName,
                    Lastname = model.LastName,
                    Email = model.Email,
                    Mobile = model.Phone,
                    Medicallicense = model.MedicalLicenseNumber,
                    Adminnotes = model.AdminNote,
                    Address1 = model.Address1,
                    Address2 = model.Address2,
                    City = model.City,
                    //Regionid = model.RegionId,
                    Zip = model.Zip,
                    Altphone = model.PhoneNumber,
                    Createdby = loginId,
                    Createddate = DateTime.Now,
                    Roleid = model.Role,
                    Npinumber = model.NPINumber,
                    Businessname = model.BusinessName,
                    Businesswebsite = model.BusinessWebsite,
                };

                _db.Physicians.Add(phy);
                _db.SaveChanges();


                Physiciannotification physiciannotification = new()
                {
                    Pysicianid = phy.Physicianid,
                    Isnotificationstopped = new BitArray(1,false),
                };
                _db.Physiciannotifications.Add(physiciannotification);
                _db.SaveChanges();


                string path = Path.Combine(_environment.WebRootPath, "PhysicianImages", phy.Physicianid.ToString());

                if (model.Photo != null)
                {
                    string fileExtension = Path.GetExtension(model.Photo.FileName);
                    if (validProfileExtensions.Contains(fileExtension))
                    {
                        InsertFileAfterRename(model.Photo, path, "ProfilePhoto");
                        phy.Photo = Path.GetFileName(model.Photo.FileName);

                    }
                }
                if (model.ICA != null)
                {
                    string fileExtension = Path.GetExtension(model.ICA.FileName);
                    if (validDocumentExtensions.Contains(fileExtension))
                    {
                        phy.Isagreementdoc = new BitArray(1, true);
                        InsertFileAfterRename(model.ICA, path, "ICA");
                    }
                }
                if (model.BGCheck != null)
                {
                    string fileExtension = Path.GetExtension(model.BGCheck.FileName);
                    if (validDocumentExtensions.Contains(fileExtension))
                    {
                        phy.Isbackgrounddoc = new BitArray(1, true);
                        InsertFileAfterRename(model.BGCheck, path, "BackgroundCheck");
                    }
                }
                if (model.HIPAACompliance != null)
                {
                    string fileExtension = Path.GetExtension(model.HIPAACompliance.FileName);
                    if (validDocumentExtensions.Contains(fileExtension))
                    {
                        phy.Isnondisclosuredoc = new BitArray(1, true);
                        InsertFileAfterRename(model.HIPAACompliance, path, "HipaaCompliance");
                    }
                }
                if (model.NDA != null)
                {
                    string fileExtension = Path.GetExtension(model.NDA.FileName);
                    if (validDocumentExtensions.Contains(fileExtension))
                    {
                        phy.Isnondisclosuredoc = new BitArray(1, true);
                        InsertFileAfterRename(model.NDA, path, "NDA");
                    }
                }
                _db.Physicians.Update(phy);
                _db.SaveChanges();
            }
            catch (Exception e)
            {
            };


        }

        public CreateShift GetCreateShift()
        {
            var regionList = _db.Regions.ToList();
            var phy = _db.Physicians.ToList();

            CreateShift obj = new()
            {
                Regions = regionList,
                Physicians = phy
            };

            return obj;
        }


        public void CreateNewShiftSubmit(string selectedDays, CreateShift obj, int adminId)
        {
            var admin = _db.Admins.FirstOrDefault(x => x.Adminid == adminId);

            var day = JsonSerializer.Deserialize<List<CheckBoxData>>(selectedDays);

            var curDate = obj.StartDate;
            var curDay = (int)obj.StartDate.DayOfWeek;

            if (!obj.IsRepeat)
            {
                var shift = new Shift()
                {
                    Physicianid = obj.PhysicianId,
                    Startdate = obj.StartDate,
                    Isrepeat = new BitArray(0, false),
                    Repeatupto = obj.RepeatUpto,
                    Createdby = admin.Aspnetuserid,
                    Createddate = DateTime.Now,
                };
                _db.Shifts.Add(shift);
                _db.SaveChanges();
            }
            else
            {
                var shift = new Shift()
                {
                    Physicianid = obj.PhysicianId,
                    Startdate = obj.StartDate,
                    Isrepeat = new BitArray(1, true),
                    Repeatupto = obj.RepeatUpto,
                    Createdby = admin.Aspnetuserid,
                    Createddate = DateTime.Now,
                };
                _db.Shifts.Add(shift);
                _db.SaveChanges();


                for (int i = 1; i <= obj.RepeatUpto; i++)
                {
                    foreach (var item in day)
                    {
                        if (item.Checked)
                        {
                            var shiftDay = 7 * i - curDay + item.Id;
                            var shiftDate = curDate.AddDays(shiftDay);

                            var shiftdetail = new Shiftdetail()
                            {
                                Shiftid = shift.Shiftid,
                                Shiftdate = shiftDate,
                                Starttime = obj.StartTime,
                                Endtime = obj.EndTime,
                                Status = (short)_db.Physicians.FirstOrDefault(x => x.Physicianid == obj.PhysicianId).Status,

                            };
                            _db.Shiftdetails.Add(shiftdetail);
                            _db.SaveChanges();

                            var shiftRegion = new Shiftdetailregion()
                            {
                                Regionid = obj.RegionId,
                                Shiftdetailid = shiftdetail.Shiftdetailid,
                            };
                            _db.Shiftdetailregions.Add(shiftRegion);
                            _db.SaveChanges();
                        }
                    }
                }
            }

        }

        public bool editProviderForm1(int phyId, int roleId, int statusId)
        {
            var user = _db.Physicians.Where(r => r.Physicianid == phyId).Select(r => r).First();

            if (user.Status != (short)statusId || user.Roleid != roleId)
            {
                user.Status = (short)statusId;
                user.Roleid = roleId;

                _db.SaveChanges();
                return true;
            }
            return false;
        }

        public bool editProviderForm2(string fname, string lname, string email, string phone, string medical, string npi, string sync, int phyId, int[] phyRegionArray)
        {
            var user = _db.Physicians.Where(r => r.Physicianid == phyId).Select(r => r).First();
            var aspUser = _db.Aspnetusers.Where(r => r.Id == user.Aspnetuserid).Select(r => r).First();

            var abc = _db.Physicianregions.Where(x => x.Physicianid == phyId).Select(r => r.Regionid).ToList();

            var changes = abc.Except(phyRegionArray);

            if (user.Firstname != fname || user.Lastname != lname || user.Email != email || user.Mobile != phone || user.Medicallicense != medical || user.Npinumber != npi || user.Syncemailaddress != sync || changes.Any() == true)
            {
                user.Firstname = fname;
                user.Lastname = lname;
                if (user.Email != email)
                {
                    user.Email = email;
                    aspUser.Email = email;
                }

                user.Mobile = phone;
                user.Medicallicense = medical;
                user.Npinumber = npi;
                user.Syncemailaddress = sync;

                _db.SaveChanges();




                if (changes.Any())
                {
                    if (_db.Physicianregions.Any(x => x.Physicianid == phyId))
                    {
                        var physicianRegion = _db.Physicianregions.Where(x => x.Physicianid == phyId).ToList();

                        _db.Physicianregions.RemoveRange(physicianRegion);
                        _db.SaveChanges();
                    }

                    var phyRegion = _db.Physicianregions.ToList();

                    foreach (var item in phyRegionArray)
                    {
                        var region = _db.Regions.FirstOrDefault(x => x.Regionid == item);

                        _db.Physicianregions.Add(new Physicianregion
                        {
                            Physicianid = phyId,
                            Regionid = region.Regionid,
                        });
                    }
                    _db.SaveChanges();
                }
                return true;
            }



            return false;
        }

        public bool editProviderForm3(EditProviderModel2 dataMain)
        {
            var data = _db.Physicians.Where(r => r.Physicianid == dataMain.editPro.PhyID).Select(r => r).First();
            if (data.Address1 != dataMain.editPro.Address1 || data.Address2 != dataMain.editPro.Address2 || data.City != dataMain.editPro.city || data.Regionid != dataMain.editPro.Regionid || data.Zip != dataMain.editPro.zipcode || data.Altphone != dataMain.editPro.altPhone)
            {
                data.Address1 = dataMain.editPro.Address1;
                data.Address2 = dataMain.editPro.Address2;
                data.City = dataMain.editPro.city;
                data.Regionid = dataMain.editPro.Regionid;
                data.Zip = dataMain.editPro.zipcode;
                data.Altphone = dataMain.editPro.altPhone;

                _db.SaveChanges();

                return true;
            }
            return false;
        }
        public bool PhysicianBusinessInfoUpdate(EditProviderModel2 dataMain)
        {


            var physician = _db.Physicians.FirstOrDefault(x => x.Physicianid == dataMain.editPro.PhyID);

            if (physician != null)
            {
                physician.Businessname = dataMain.editPro.Businessname;
                physician.Businesswebsite = dataMain.editPro.BusinessWebsite;
                physician.Adminnotes = dataMain.editPro.Adminnotes;
                physician.Modifieddate = DateTime.Now;

                _db.SaveChanges();

                if (dataMain.editPro.Photo != null || dataMain.editPro.Signature != null)
                {
                    AddProviderBusinessPhotos(dataMain.editPro.Photo, dataMain.editPro.Signature, dataMain.editPro.PhyID);
                }

            }

            return true;
        }
        public void AddProviderBusinessPhotos(IFormFile photo, IFormFile signature, int phyId)
        {
            var physician = _db.Physicians.FirstOrDefault(x => x.Physicianid == phyId);

            if (photo != null)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles", photo.FileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    photo.CopyTo(fileStream);
                }

                physician.Photo = photo.FileName;
            }

            if (signature != null)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles", signature.FileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    signature.CopyTo(fileStream);
                }

                physician.Signature = signature.FileName;
            }

            _db.SaveChanges();

        }

        public bool EditOnBoardingData(EditProviderModel2 dataMain)
        {

            var physicianData = _db.Physicians.FirstOrDefault(x => x.Physicianid == dataMain.editPro.PhyID);

            string directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles", physicianData.Physicianid.ToString());

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (dataMain.editPro.ContractorAgreement != null)
            {
                string path = Path.Combine(directory, "Independent_Contractor" + Path.GetExtension(dataMain.editPro.ContractorAgreement.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    dataMain.editPro.ContractorAgreement.CopyTo(fileStream);
                }

                physicianData.Isagreementdoc = new BitArray(1, true);
            }

            if (dataMain.editPro.BackgroundCheck != null)
            {
                string path = Path.Combine(directory, "Background" + Path.GetExtension(dataMain.editPro.BackgroundCheck.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    dataMain.editPro.BackgroundCheck.CopyTo(fileStream);
                }

                physicianData.Isbackgrounddoc = new BitArray(1, true);
            }

            if (dataMain.editPro.HIPAA != null)
            {
                string path = Path.Combine(directory, "HIPAA" + Path.GetExtension(dataMain.editPro.HIPAA.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    dataMain.editPro.HIPAA.CopyTo(fileStream);
                }

                physicianData.Istrainingdoc = new BitArray(1, true);
            }

            if (dataMain.editPro.NonDisclosure != null)
            {
                string path = Path.Combine(directory, "Non_Disclosure" + Path.GetExtension(dataMain.editPro.NonDisclosure.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    dataMain.editPro.NonDisclosure.CopyTo(fileStream);
                }

                physicianData.Isnondisclosuredoc = new BitArray(1, true);
            }

            if (dataMain.editPro.LicenseDocument != null)
            {
                string path = Path.Combine(directory, "Licence" + Path.GetExtension(dataMain.editPro.LicenseDocument.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    dataMain.editPro.LicenseDocument.CopyTo(fileStream);
                }

                physicianData.Islicensedoc = new BitArray(1, true);
            }

            _db.SaveChanges();



            return true;

        }

        public void editProviderDeleteAccount(int phyId)
        {
            var phy = _db.Physicians.Where(r => r.Physicianid == phyId).Select(r => r).First();

            if (phy.Isdeleted == null)
            {
                phy.Isdeleted = new BitArray(1);
                phy.Isdeleted[0] = true;

                _db.SaveChanges();
            }
        }

        public List<BusinessTable> BusinessTable()
        {
            var obj = (from t1 in _db.Healthprofessionals
                       join t2 in _db.Healthprofessionaltypes on t1.Profession equals t2.Healthprofessionalid
                       where t2.Isdeleted != new BitArray(1,true)
                       select new BusinessTable
                       {
                           BusinessId = t1.Vendorid,
                           ProfessionId = t2.Healthprofessionalid,
                           ProfessionName = t2.Professionname,
                           Email = t1.Email,
                           PhoneNumber = t1.Phonenumber,
                           FaxNumber = t1.Faxnumber,
                           BusinessContact = t1.Businesscontact
                       });
            return obj.ToList();    
        }

        public void AddBusiness(AddBusinessModel obj)
        {
            

            Healthprofessional healthprofessional = new()
            {
                Vendorname = obj.BusinessName,
                Profession = obj.ProfessionId,
                Faxnumber = obj.FaxNumber,
                Address = obj.Street,
                City = obj.City,
                State = _db.Regions.FirstOrDefault(x => x.Regionid == obj.RegionId).Name,
                Zip = obj.Zip,
                Regionid = obj.RegionId,
                Createddate = DateTime.Now,
                Phonenumber = obj.PhoneNumber,
                Email = obj.BusinessContact,
                Isdeleted = new BitArray(1, false),
            };
            _db.Healthprofessionals.Add(healthprofessional);
            _db.SaveChanges();

        }

        public List<Healthprofessionaltype> GetProfession()
        {
            var obj = _db.Healthprofessionaltypes.ToList();
            return obj;
        }


    }
}
