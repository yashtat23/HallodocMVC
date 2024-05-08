using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DataContext;
using DataAccess.DataModels;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using DataAccess.CustomModel;
using DataAccess.Enum;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using System.Collections;
using System.Net.Mail;
using System.Net;
using DataAccess.Enums;
using DataAccess.CustomModels;
using System.Linq.Expressions;
using LinqKit;

namespace BusinessLogic.Repository
{
    public class ProviderService : IProviderService
    {

        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _http;
        private readonly IJwtService _jwtService;
        private IWebHostEnvironment _environment;
        private readonly IGenericService<WeeklyTimeSheet> _weeklyTimeSheetRepo;
        private readonly IGenericService<Shiftdetail> _shiftDetailrepo;
        private readonly IGenericService<PayRate> _payRateRepo;
        private readonly IGenericService<WeeklyTimeSheetDetail> _weeklyTimeSheetDetailRepo;

        public ProviderService(ApplicationDbContext db, IHttpContextAccessor http, IJwtService jwtService, IWebHostEnvironment hostEnvironment, IGenericService<WeeklyTimeSheet> weeklyTimeSheetRepo, IGenericService<Shiftdetail> shiftDetailrepo, IGenericService<WeeklyTimeSheetDetail> weeklyTimeSheetDetailRepo, IGenericService<PayRate> payRateRepo)
        {
            _db = db;
            _http = http;
            _jwtService = jwtService;
            _environment = hostEnvironment;
            _weeklyTimeSheetRepo = weeklyTimeSheetRepo;
            _shiftDetailrepo = shiftDetailrepo;
            _weeklyTimeSheetDetailRepo = weeklyTimeSheetDetailRepo;
            _payRateRepo = payRateRepo;
        }

        public DashboardModel GetRequestsByStatus(int tabNo, int CurrentPage, int phyid)
        {
            var query = from r in _db.Requests
                        join rc in _db.Requestclients on r.Requestid equals rc.Requestid
                        where r.Physicianid == phyid
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
                            regionId = rc.Regionid,
                            calltype = (short)r.Calltype,
                            isFinalized = _db.Encounterforms.Where(x => x.Requestid == r.Requestid).Select(x => x.Isfinalized).First() ?? null
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



            var result = query.ToList();
            int count = result.Count();
            int TotalPage = (int)Math.Ceiling(count / (double)5);
            result = result.Skip((CurrentPage - 1) * 5).Take(5).ToList();

            DashboardModel dashboardModel = new DashboardModel();
            dashboardModel.adminDashboardList = result;
            dashboardModel.regionList = _db.Regions.ToList();
            dashboardModel.TotalPage = TotalPage;
            dashboardModel.CurrentPage = CurrentPage;
            return dashboardModel;
        }

        public StatusCountModel GetStatusCount(int phyid)
        {
            var requestsWithClients = _db.Requests
     .Join(_db.Requestclients,
         r => r.Requestid,
         rc => rc.Requestid,
         (r, rc) => new { Request = r, RequestClient = rc })
     .Where(r => r.Request.Physicianid == phyid).ToList();

            StatusCountModel statusCount = new StatusCountModel
            {
                NewCount = requestsWithClients.Count(x => x.Request.Status == (int)StatusEnum.Unassigned),
                PendingCount = requestsWithClients.Count(x => x.Request.Status == (int)StatusEnum.Accepted),
                ActiveCount = requestsWithClients.Count(x => x.Request.Status == (int)StatusEnum.MDEnRoute || x.Request.Status == (int)StatusEnum.MDOnSite),
                ConcludeCount = requestsWithClients.Count(x => x.Request.Status == (int)StatusEnum.Conclude),

            };

            return statusCount;

        }

        public DashboardModel GetRequestByRegion(FilterModel filterModel, int phyid)
        {

            var query = from r in _db.Requests
                        join rc in _db.Requestclients on r.Requestid equals rc.Requestid
                        where r.Physicianid == phyid
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
                            regionId = rc.Regionid,
                            calltype = (short)r.Calltype,
                            phyId = r.Physicianid ?? null,
                            isFinalized = _db.Encounterforms.Where(x => x.Requestid == r.Requestid).Select(x => x.Isfinalized).First() ?? null,
                            reqDate = r.Createddate.ToString("yyyy-MMM-dd"),
                            notes = _db.Requeststatuslogs
                                     .Where(x => x.Requestid == r.Requestid)
                                     .OrderBy(x => x.Requeststatuslogid)
                                     .Select(x => x.Notes)
                                     .LastOrDefault() ?? null,

                        };


            if (filterModel.tabNo == 1)
            {

                query = query.Where(x => x.status == (int)StatusEnum.Unassigned);
            }

            else if (filterModel.tabNo == 2)
            {

                query = query.Where(x => x.status == (int)StatusEnum.Accepted);
            }
            else if (filterModel.tabNo == 3)
            {

                query = query.Where(x => x.status == (int)StatusEnum.MDEnRoute || x.status == (int)StatusEnum.MDOnSite);
            }
            else if (filterModel.tabNo == 4)
            {

                query = query.Where(x => x.status == (int)StatusEnum.Conclude);
            }


            if (filterModel.searchWord != null)
            {
                query = query.Where(x => x.firstName.Trim().ToLower().Contains(filterModel.searchWord.Trim().ToLower()));
            }
            if (filterModel.requestTypeId != null)
            {
                query = query.Where(x => x.requestTypeId == filterModel.requestTypeId);
            }

            var result = query.ToList();
            int count = result.Count();
            int TotalPage = (int)Math.Ceiling(count / (double)5);
            result = result.Skip((filterModel.CurrentPage - 1) * 5).Take(5).ToList();

            DashboardModel dashboardModel = new DashboardModel();
            dashboardModel.adminDashboardList = result;
            dashboardModel.regionList = _db.Regions.ToList();
            dashboardModel.TotalPage = TotalPage;
            dashboardModel.CurrentPage = filterModel.CurrentPage;
            return dashboardModel;
        }


        public void acceptCase(int requestId, string loginUserId)
        {
            //string? aspId = HttpContext.Session.GetString("UserId");
            var req = _db.Requests.FirstOrDefault(x => x.Requestid == requestId);

            //int phyId = _db.Physicians.Where(x => x.Aspnetuserid == aspId).Select(i => i.Physicianid).FirstOrDefault();

            Request? req_data = _db.Requests.Where(i => i.Requestid == requestId).FirstOrDefault();
            var reqStatLog = _db.Requeststatuslogs.Where(i => i.Requestid == requestId).FirstOrDefault();

            int phyId = _db.Physicians.Where(x => x.Aspnetuserid == loginUserId).Select(x => x.Physicianid).FirstOrDefault();
            Requeststatuslog requestList = new Requeststatuslog()
            {
                Requestid = requestId,
                Status = req_data.Status,
                Physicianid = phyId,
                Createddate = DateTime.Now,
                Notes = "Req Accepted By physicion ",
            };
            _db.Add(requestList);
            req_data.Status = 2;

            _db.SaveChanges();

        }

        public bool TransferRequest(TransferRequest model)
        {
            var req = _db.Requests.Where(x => x.Requestid == model.ReqId).FirstOrDefault();
            if (req != null)
            {
                req.Status = (int)StatusEnum.Unassigned;
                req.Modifieddate = DateTime.Now;
                _db.Requests.Update(req);

                Requeststatuslog rsl = new Requeststatuslog();
                rsl.Requestid = (int)model.ReqId;
                rsl.Status = (int)StatusEnum.Unassigned;
                rsl.Notes = model.description;
                rsl.Createddate = DateTime.Now;
                _db.Requeststatuslogs.Add(rsl);
                _db.SaveChanges();

                return true;
            }
            return false;
        }

        public void CallType(int requestId, short callType)
        {
            Request? req = _db.Requests.FirstOrDefault(x => x.Requestid == requestId);
            req.Calltype = callType;

            if (callType == 1)
            {
                req.Status = (int)StatusEnum.MDOnSite;
            }
            else
            {
                req.Status = (int)StatusEnum.Conclude;
            }
            _db.Requests.Update(req);
            _db.SaveChanges();

        }

        public void housecall(int requestId)
        {
            Request? req = _db.Requests.FirstOrDefault(x => x.Requestid == requestId);
            req.Status = (int)StatusEnum.Conclude;
            _db.Requests.Update(req);
            _db.SaveChanges();
        }

        public bool PSubmitEncounterForm(EncounterFormModel model)
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
                        //Isfinalized = true
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
                    //ef.Isfinalized = true;
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

        public async Task<ConcludeCareViewModel> ConcludeCare(int request_id)
        {
            Requestclient? data = await _db.Requestclients.Include(a => a.Request).Include(a => a.Request.Requestwisefiles).Where(a => a.Requestid == request_id).FirstOrDefaultAsync();
            Encounterform? encounterForm = await _db.Encounterforms.FirstOrDefaultAsync(a => a.Requestid == request_id);

            if (data != null)
            {
                data!.Request.Requestwisefiles = data.Request.Requestwisefiles.Where(x => x.Isdeleted == null || x.Isdeleted[0] == false).ToList();

                ConcludeCareViewModel model = new ConcludeCareViewModel()
                {
                    RequestId = request_id,
                    PatientName = data.Firstname + " " + data.Lastname,
                };
                if (encounterForm != null && encounterForm.Isfinalized == true)
                {
                    model.IsFinalize = 1;
                }
                else
                {
                    model.IsFinalize = 0;
                }
                foreach (var item in data.Request.Requestwisefiles)
                {
                    model.Documents.Add(item.Filename);
                }
                return model;
            }
            return new ConcludeCareViewModel();
        }

        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 6)
                      + Path.GetExtension(fileName);
        }

        public async Task UploadDocuments(ConcludeCareViewModel model, int RequestId)
        {
            if (model.Upload != null)
            {
                IEnumerable<IFormFile> upload = model.Upload;
                foreach (var item in upload)
                {
                    var file = item.FileName;
                    var uniqueFileName = GetUniqueFileName(file);
                    var uploads = Path.Combine(_environment.WebRootPath, "provideruploads");
                    var filePath = Path.Combine(uploads, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await item.CopyToAsync(fileStream);
                    }

                    Requestwisefile requestWiseFile = new Requestwisefile()
                    {

                        Filename = uniqueFileName,
                        Createddate = DateTime.Now,
                    };
                    _db.Requestwisefiles.Add(requestWiseFile);
                    requestWiseFile.Requestid = RequestId;
                }
            }
            await _db.SaveChangesAsync();
        }

        public async Task ConcludeCase(ConcludeCareViewModel model, string email)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var obj = await _db.Requests.Where(a => a.Requestid == model.RequestId).FirstOrDefaultAsync();
                    var data = await _db.Requestnotes.Where(a => a.Requestid == model.RequestId).FirstOrDefaultAsync();
                    var physician = await _db.Physicians.FirstOrDefaultAsync(a => a.Email == email);

                    if (data != null)
                    {
                        data!.Physiciannotes = model.ProviderNotes;
                        data.Modifiedby = physician!.Aspnetuserid;
                        data.Modifieddate = DateTime.Now;
                        _db.Requestnotes.Update(data);
                    }
                    else
                    {
                        Requestnote requestNote = new Requestnote()
                        {
                            Requestid = model.RequestId,
                            Adminnotes = model.ProviderNotes,
                            Createdby = physician!.Aspnetuserid!,
                            Createddate = DateTime.Now,
                        };
                        _db.Requestnotes.Add(requestNote);
                    }

                    if (obj != null && physician != null)
                    {
                        obj.Status = (int)StatusEnum.Closed;
                        Requeststatuslog requestStatusLog = new Requeststatuslog()
                        {
                            Requestid = model.RequestId,
                            Notes = model.ProviderNotes,
                            Status = (int)StatusEnum.Closed,
                            Createddate = DateTime.Now,
                        };
                        await _db.Requeststatuslogs.AddAsync(requestStatusLog);
                        _db.Requests.Update(obj);
                    }
                    await _db.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
            }
        }

        public int GetPhysicianId(string userid)
        {
            int phyid = _db.Physicians.Where(x => x.Aspnetuserid == userid).Select(x => x.Physicianid).First();
            return phyid;
        }

        public MonthWiseScheduling PhysicianMonthlySchedule(string date, int status, string aspnetuserid)
        {
            var currentDate = DateTime.Parse(date);
            int? phy = _db.Physicians.Where(x => x.Aspnetuserid == aspnetuserid).Select(x => x.Physicianid).FirstOrDefault();

            BitArray deletedBit = new BitArray(new[] { false });
            MonthWiseScheduling month = new MonthWiseScheduling
            {
                date = currentDate,

            };

            if (status != 0)
            {
                var shiftdetailList = from t1 in _db.Shiftdetails
                                      join t2 in _db.Shifts
                                      on t1.Shiftid equals t2.Shiftid
                                      where t1.Status == status && t2.Physicianid == phy
                                      orderby t1.Starttime ascending
                                      select t1;
                month.shiftdetails = shiftdetailList.ToList();
            }
            else
            {
                var shiftdetailList = from t1 in _db.Shiftdetails
                                      join t2 in _db.Shifts
                                      on t1.Shiftid equals t2.Shiftid
                                      where t2.Physicianid == phy
                                      orderby t1.Starttime ascending
                                      select t1;
                month.shiftdetails = shiftdetailList.ToList();
            }
            return month;
        }

        public bool concludecaresubmit(int ReqId, string ProviderNote)
        {
            try
            {
                var req1 = _db.Requests.FirstOrDefault(x => x.Requestid == ReqId);
                var ise = new BitArray(1, false);
                req1.Status = (int)StatusEnum.Closed;
                req1.Isurgentemailsent = ise;
                _db.Requests.Update(req1);

                Requeststatuslog rsl = new Requeststatuslog();
                rsl.Requestid = ReqId;
                rsl.Status = (int)StatusEnum.Closed;
                rsl.Notes = ProviderNote;
                rsl.Createddate = DateTime.Now;
                _db.Requeststatuslogs.Add(rsl);

                _db.SaveChanges();
                return true;

            }

            catch { return false; }
        }

        public bool finalizesubmit(int reqid)
        {
            try
            {
                var enc = _db.Encounterforms.FirstOrDefault(x => x.Requestid == reqid);
                if(enc == null)
                {
                    return false;
                }
                enc.Isfinalized = true;
                _db.Encounterforms.Update(enc);
                _db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void SendRegistrationEmailCreateRequest(string email, string note, string sessionEmail)
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
                Subject = "Request Note To Admin",
                IsBodyHtml = true,
                Body = $"Note: '{note}'"
            };

            Emaillog emailLog = new Emaillog()
            {
                Subjectname = mailMessage.Subject,
                Emailtemplate = "Sender : " + senderEmail + "Reciver :" + email + "Subject : " + mailMessage.Subject + "Message : " + "FileSent",
                Emailid = email,
                Roleid = 3,
                Physicianid = _db.Physicians.Where(r => r.Email == sessionEmail).Select(r => r.Physicianid).First(),
                Createdate = DateTime.Now,
                Sentdate = DateTime.Now,
                Isemailsent = new BitArray(1, true),
                Confirmationnumber = sessionEmail.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 19).Replace(" ", ""),
                Senttries = 1,
            };


            _db.Emaillogs.Add(emailLog);
            _db.SaveChanges();


            mailMessage.To.Add(email);

            client.Send(mailMessage);
        }


        public void RequestAdmin(RequestAdmin model, string sessionEmail)
        {
            var email = _db.Admins.ToList();

            foreach (var item in email)
            {
                try
                {
                    SendRegistrationEmailCreateRequest(item.Email, model.Note, sessionEmail);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }


        public void SendRegistrationEmailCreateRequest(string email, string registrationLink)
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
                Subject = "Create Account",
                IsBodyHtml = true,
                Body = $"Click the following link to Create Account: <a href='{registrationLink}'>{registrationLink}</a>"
            };



            mailMessage.To.Add(email);

            client.Send(mailMessage);
        }

        public bool PCreateRequest(CreateRequestModel model, string sessionEmail, string createAccountLink)
        {
            CreateRequestModel _create = new CreateRequestModel();

            var stateMain = _db.Regions.Where(r => r.Name.ToLower() == model.state.ToLower().Trim()).FirstOrDefault();

            if (stateMain == null)
            {
                return false;
            }
            else
            {
                Request req = new();
                Requestclient reqClient = new();
                User user = new User();
                Aspnetuser asp = new Aspnetuser();
                Requestnote note = new Requestnote();

                var admin = _db.Physicians.Where(r => r.Email == sessionEmail).Select(r => r).First();

                var existUser = _db.Aspnetusers.FirstOrDefault(r => r.Email == model.email);

                if (existUser == null)
                {
                    asp.Id = Guid.NewGuid().ToString();
                    asp.Username = model.firstname + "_" + model.lastname;
                    asp.Email = model.email;
                    asp.Phonenumber = model.phone;
                    asp.Createddate = DateTime.Now;
                    _db.Aspnetusers.Add(asp);
                    _db.SaveChanges();

                    user.Aspnetuserid = asp.Id;
                    user.Firstname = model.firstname;
                    user.Lastname = model.lastname;
                    user.Email = model.email;
                    user.Mobile = model.phone;
                    user.City = model.city;
                    user.State = model.state;
                    user.Street = model.street;
                    user.Zipcode = model.zipcode;
                    user.Strmonth = model.dateofbirth.Substring(5, 2);
                    user.Intdate = Convert.ToInt16(model.dateofbirth.Substring(8, 2));
                    user.Intyear = Convert.ToInt16(model.dateofbirth.Substring(0, 4));
                    user.Createdby = admin.Physicianid.ToString();
                    user.Createddate = DateTime.Now;
                    user.Regionid = stateMain.Regionid;
                    _db.Users.Add(user);
                    _db.SaveChanges();

                    try
                    {
                        SendRegistrationEmailCreateRequest(model.email, createAccountLink);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                req.Requesttypeid = (int)RequestTypeEnum.Patient;
                req.Userid = _db.Users.Where(x=>x.Email==model.email).Select(x=>x.Userid).FirstOrDefault();
                req.Firstname = admin.Firstname;
                req.Lastname = admin.Lastname;
                req.Phonenumber = admin.Mobile;
                req.Email = admin.Email;
                req.Physicianid = _db.Physicians.Where(x=>x.Email == sessionEmail).Select(x=>x.Physicianid).FirstOrDefault();
                req.Status = (int)StatusEnum.Accepted;
                req.Confirmationnumber = admin.Firstname.Substring(0, 1) + DateTime.Now.ToString().Substring(0, 19);
                req.Createddate = DateTime.Now;
                req.Isurgentemailsent = new BitArray(1);
                req.Isurgentemailsent[0] = false;
                req.Calltype = 0;   
                _db.Requests.Add(req);
                _db.SaveChanges();

                reqClient.Requestid = req.Requestid;
                reqClient.Firstname = model.firstname;
                reqClient.Lastname = model.lastname;
                reqClient.Phonenumber = model.phone;
                reqClient.Strmonth = model.dateofbirth.Substring(5, 2);
                reqClient.Intdate = Convert.ToInt16(model.dateofbirth.Substring(8, 2));
                reqClient.Intyear = Convert.ToInt16(model.dateofbirth.Substring(0, 4));
                reqClient.Street = model.street;
                reqClient.City = model.city;
                reqClient.State = model.state;
                reqClient.Zipcode = model.zipcode;
                reqClient.Regionid = stateMain.Regionid;
                reqClient.Email = model.email;

                _db.Requestclients.Add(reqClient);
                _db.SaveChanges();

                note.Requestid = req.Requestid;
                note.Adminnotes = model.admin_notes;
                note.Createdby = _db.Aspnetusers.Where(r => r.Email == sessionEmail).Select(r => r.Id).First();
                note.Createddate = DateTime.Now;
                _db.Requestnotes.Add(note);
                _db.SaveChanges();

                return true;
            }
        }

        public List<DateViewModel> GetDates()
        {
            List<DateViewModel> dates = new List<DateViewModel>();
            int startMonth = 0;
            int startYear = 0;
            int startDate = 1;
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            int nextDate = 1;
            if (today.Day > 15)
            {
                nextDate = 2;
            }
            if (today.Month - 6 < 0)
            {
                startMonth = 12 - (6 - today.Month) + 1;
                startYear = today.Year - 1;
            }
            else if (today.Month - 6 == 0)
            {
                startMonth = 1;
                startYear = today.Year;
            }
            else
            {
                startMonth = today.Month - 6;
                startYear = today.Year;
            }
            int count = 12;
            if (nextDate == 1)
            {
                count = 11;
            }
            for (int i = 1; i <= count; i++)
            {

                if (i % 2 == 0)
                {
                    startDate = 16;
                }
                else
                {
                    startDate = 1;

                }
                if (startMonth > 12)
                {
                    startMonth = 1;
                    startYear = today.Year;
                }
                DateViewModel date = new DateViewModel();
                date.StartDate = new DateOnly(startYear, startMonth, startDate);
                if (startDate != 1)
                    date.EndDate = date.StartDate.AddMonths(1).AddDays(-16);
                else
                    date.EndDate = new DateOnly(startYear, startMonth, 15);
                dates.Add(date);
                if (startDate == 16)
                    startMonth += 1;
            }
            dates.Reverse();
            return dates;
        }

        public InvoicingViewModel GetInvoicingDataonChangeOfDate(DateOnly startDate, DateOnly endDate, int? PhysicianId, int? AdminID)
        {
            InvoicingViewModel model = new InvoicingViewModel();
            var weeklyTimeSheet = _db.WeeklyTimeSheets.FirstOrDefault(u => u.ProviderId == PhysicianId && (u.StartDate == startDate && u.EndDate == endDate));
            if (weeklyTimeSheet != null)
            {
                var TimehseetsData = _weeklyTimeSheetDetailRepo.SelectWhereOrderBy(x => new Timesheet
                {
                    Date = x.Date,
                    NumberofShift = x.NumberOfShifts ?? 0,
                    NightShiftWeekend = x.IsWeekendHoliday == true ? 1 : 0,
                    NumberOfHouseCall = (x.IsWeekendHoliday == false ? x.HouseCall : 0) ?? 0,
                    HousecallNightsWeekend = (x.IsWeekendHoliday == true ? x.HouseCall : 0) ?? 0,
                    NumberOfPhoneConsults = (x.IsWeekendHoliday == false ? x.PhoneConsult : 0) ?? 0,
                    phoneConsultNightsWeekend = (x.IsWeekendHoliday == true ? x.PhoneConsult : 0) ?? 0,
                    BatchTesting = x.BatchTesting ?? 0
                }, x => x.TimeSheetId == weeklyTimeSheet.TimeSheetId, x => x.Date);
                List<Timesheet> list = new List<Timesheet>();
                foreach (Timesheet item in TimehseetsData)
                {
                    list.Add(item);
                }
                model.timesheets = list;
                model.PhysicianId = PhysicianId ?? 0;
                model.IsFinalized = weeklyTimeSheet.IsFinalized == true ? true : false;
                model.startDate = startDate;
                model.endDate = endDate;
                model.Status = weeklyTimeSheet.Status == 1 ? "Pending" : "Aprooved";
            }
            else
            {
                model.timesheets = new List<Timesheet>();
            }
            model.isAdminSide = AdminID == null ? false : true;
            return model;
        }

        public InvoicingViewModel GetUploadedDataonChangeOfDate(DateOnly startDate, DateOnly endDate, int? PhysicianId, int pageNumber, int pagesize)
        {
            WeeklyTimeSheet weeklyTimeSheet = _weeklyTimeSheetRepo.GetFirstOrDefault(u => u.ProviderId == PhysicianId && (u.StartDate == startDate && u.EndDate == endDate));
            InvoicingViewModel model = new InvoicingViewModel();
            Expression<Func<WeeklyTimeSheetDetail, bool>> whereClauseSyntax = PredicateBuilder.New<WeeklyTimeSheetDetail>();
            whereClauseSyntax = x => x.Bill != null && x.TimeSheetId == weeklyTimeSheet.TimeSheetId;
            if (weeklyTimeSheet != null)
            {
                var UploadedItems = _weeklyTimeSheetDetailRepo.GetAllWithPagination(x => new Timesheet
                {
                    Date = x.Date,
                    Items = x.Item ?? "-",
                    Amount = x.Amount ?? 0,
                    BillName = x.Bill ?? "-",
                }, whereClauseSyntax, pageNumber, pagesize, x => x.Date, true);
                List<Timesheet> list = new List<Timesheet>();
                foreach (Timesheet item in UploadedItems)
                {
                    list.Add(item);
                }
                model.timesheets = list;

                model.pager = new Pager
                {
                    TotalItems = _weeklyTimeSheetDetailRepo.GetTotalcount(whereClauseSyntax),
                    CurrentPage = pageNumber,
                    ItemsPerPage = pagesize
                };
                model.SkipCount = (pageNumber - 1) * pagesize;
                model.CurrentPage = pageNumber;
                model.TotalPages = (int)Math.Ceiling((decimal)model.pager.TotalItems / pagesize);
                model.IsFinalized = weeklyTimeSheet.IsFinalized == true ? true : false;
            }
            model.PhysicianId = PhysicianId ?? 0;
            return model;
        }

        public InvoicingViewModel getDataOfTimesheet(DateOnly startDate, DateOnly endDate, int? PhysicianId, int? AdminID)
        {
            InvoicingViewModel model = new InvoicingViewModel();
            model.startDate = startDate;
            model.endDate = endDate;
            model.differentDays = endDate.Day - startDate.Day;
            WeeklyTimeSheet weeklyTimeSheet = _weeklyTimeSheetRepo.GetFirstOrDefault(u => u.ProviderId == PhysicianId && u.StartDate == startDate && u.EndDate == endDate);
            Expression<Func<WeeklyTimeSheetDetail, bool>> whereClauseSyntax1 = PredicateBuilder.New<WeeklyTimeSheetDetail>();


            if (weeklyTimeSheet != null)
            {
                PayRate payRate = _payRateRepo.GetFirstOrDefault(u => u.PhysicianId == weeklyTimeSheet.ProviderId);
                whereClauseSyntax1 = x => x.TimeSheet!.ProviderId == PhysicianId && x.TimeSheet.StartDate == startDate && x.TimeSheet.EndDate == endDate;
                model.TimeSheetId = weeklyTimeSheet.TimeSheetId;
                var ExistingTimeshet = _weeklyTimeSheetDetailRepo.SelectWhereOrderBy(x => new Timesheet
                {
                    Date = x.Date,
                    NumberOfHouseCall = x.HouseCall ?? 0,
                    NumberOfPhoneConsults = x.PhoneConsult ?? 0,
                    Weekend = x.IsWeekendHoliday ?? false,
                    TotalHours = x.TotalHours ?? 0,
                    OnCallhours = x.NumberOfShifts ?? 0,
                    Amount = x.Amount ?? 0,
                    Items = x.Item,
                    BillName = x.Bill,
                    WeeklyTimesheetDeatilsId = x.TimeSheetDetailId,
                }, whereClauseSyntax1, x => x.Date);
                List<Timesheet> list = new List<Timesheet>();
                foreach (Timesheet item in ExistingTimeshet)
                {
                    model.shiftTotal += (item.TotalHours * payRate.Shift) ?? 0;
                    model.weekendTotal += item.Weekend == true ? (1 * payRate.NightShiftWeekend) ?? 0 : 0;
                    model.HouseCallTotal += (item.NumberOfHouseCall * payRate.HouseCall) ?? 0;
                    model.phoneconsultTotal += (item.NumberOfPhoneConsults * payRate.PhoneConsult) ?? 0;
                    list.Add(item);
                }
                model.timesheets = list;
                model.shiftPayrate = payRate.Shift ?? 0;
                model.weekendPayrate = payRate.NightShiftWeekend ?? 0;
                model.HouseCallPayrate = payRate.HouseCall ?? 0;
                model.phoneConsultPayrate = payRate.PhoneConsult ?? 0;
                model.GrandTotal = model.shiftTotal + model.weekendTotal + model.HouseCallTotal + model.phoneconsultTotal;

            }
            else
            {
                DateOnly currentDate = startDate;
                while (currentDate <= endDate)
                {
                    model.timesheets.Add(new Timesheet
                    {
                        Date = currentDate,

                    });

                    currentDate = currentDate.AddDays(1);
                }
            }
            model.startDate = startDate;
            model.endDate = endDate;
            model.PhysicianId = PhysicianId ?? 0;
            model.IsFinalized = weeklyTimeSheet == null ? false : true;
            model.isAdminSide = AdminID == null ? false : true;
            return model;

        }

        public void AprooveTimeSheet(InvoicingViewModel model, int? AdminID)
        {
            WeeklyTimeSheet weeklyTimeSheet = _weeklyTimeSheetRepo.GetFirstOrDefault(u => u.ProviderId == model.PhysicianId && u.StartDate == model.startDate && u.EndDate == model.endDate);
            if (weeklyTimeSheet != null)
            {
                weeklyTimeSheet.AdminId = AdminID;
                weeklyTimeSheet.Status = 2;
                //weeklyTimeSheet.BonusAmount = model.BonusAmount;
                weeklyTimeSheet.AdminNote = model.AdminNotes;
                _weeklyTimeSheetRepo.Update(weeklyTimeSheet);
            }
        }

        public void SubmitTimeSheet(InvoicingViewModel model, int? PhysicianId)
        {
            WeeklyTimeSheet existingWeekltTimesheet = _weeklyTimeSheetRepo.GetFirstOrDefault(u => u.ProviderId == PhysicianId && u.StartDate == model.startDate && u.EndDate == model.endDate);
            if (existingWeekltTimesheet == null)
            {
                WeeklyTimeSheet weeklyTimeSheet = new WeeklyTimeSheet();
                weeklyTimeSheet.StartDate = model.startDate;
                weeklyTimeSheet.EndDate = model.endDate;
                weeklyTimeSheet.Status = 1;
                weeklyTimeSheet.CreatedDate = DateTime.Now;
                weeklyTimeSheet.ProviderId = PhysicianId ?? 0;
                _weeklyTimeSheetRepo.Add(weeklyTimeSheet);

                foreach (var item in model.timesheets)
                {
                    BitArray deletedBit = new BitArray(new[] { false });

                    WeeklyTimeSheetDetail detail = new WeeklyTimeSheetDetail();
                    detail.Date = item.Date;
                    detail.NumberOfShifts = _shiftDetailrepo.Count(u => u.Shift.Physicianid == PhysicianId && u.Shiftdate == item.Date && u.Isdeleted == deletedBit);
                    detail.TotalHours = item.TotalHours;
                    detail.IsWeekendHoliday = item.Weekend;
                    detail.HouseCall = item.NumberOfHouseCall;
                    detail.PhoneConsult = item.NumberOfPhoneConsults;
                    detail.TimeSheetId = weeklyTimeSheet.TimeSheetId;
                    if (item.Bill != null)
                    {
                        IFormFile newFile = item.Bill;
                        detail.Bill = newFile.FileName;
                        var filePath = Path.Combine("wwwroot", "UploadedFiles", "ProviderBills", PhysicianId + "-" + item.Date + "-" + Path.GetFileName(newFile.FileName));
                        using (FileStream stream = System.IO.File.Create(filePath))
                        {
                            newFile.CopyTo(stream);
                        }
                    }
                    detail.Item = item.Items;
                    detail.Amount = item.Amount;
                    _weeklyTimeSheetDetailRepo.Add(detail);
                }
            }
            else
            {
                var exsitingTimeSheetDetail = _weeklyTimeSheetDetailRepo.GetList(u => u.TimeSheetId == existingWeekltTimesheet.TimeSheetId && u.Date >= model.startDate && u.Date <= model.endDate);
                List<WeeklyTimeSheetDetail> list = new List<WeeklyTimeSheetDetail>();

                for (int i = 0; i < model.timesheets.Count; i++)
                {
                    var currentDate = model.timesheets[i].Date;
                    WeeklyTimeSheetDetail weeklyTimeSheetDetail = exsitingTimeSheetDetail.FirstOrDefault(detail => detail.Date == currentDate)!;
                    if (weeklyTimeSheetDetail != null)
                    {
                        weeklyTimeSheetDetail.Date = model.timesheets[i].Date;
                        weeklyTimeSheetDetail.HouseCall = model.timesheets[i].NumberOfHouseCall;
                        weeklyTimeSheetDetail.PhoneConsult = model.timesheets[i].NumberOfPhoneConsults;
                        weeklyTimeSheetDetail.Item = model.timesheets[i].Items ?? null;
                        weeklyTimeSheetDetail.Amount = model.timesheets[i].Amount;
                        weeklyTimeSheetDetail.TotalHours = model.timesheets[i].TotalHours;
                        weeklyTimeSheetDetail.IsWeekendHoliday = model.timesheets[i].Weekend;
                        if (model.timesheets[i].Bill != null && model.timesheets[i].Bill!.Length > 0)
                        {
                            IFormFile newFile = model.timesheets[i].Bill!;
                            weeklyTimeSheetDetail.Bill = newFile.FileName;
                            var filePath = Path.Combine("wwwroot", "Uploaded_files", "ProviderBills", PhysicianId + "-" + model.timesheets[i].Date + "-" + Path.GetFileName(newFile.FileName));
                            FileStream stream = null;
                            try
                            {
                                stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                                newFile.CopyToAsync(stream)
;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"An error occurred: {ex.Message}");
                            }
                        }
                        list.Add(weeklyTimeSheetDetail);
                    }
                }
                foreach (WeeklyTimeSheetDetail item in list)
                {
                    _weeklyTimeSheetDetailRepo.Update(item);
                }
            }

        }

        public void DeleteBill(int id)
        {
            WeeklyTimeSheetDetail weeklyTimeSheetDetail = _weeklyTimeSheetDetailRepo.GetFirstOrDefault(u => u.TimeSheetDetailId == id);
            weeklyTimeSheetDetail.Bill = null;
            _weeklyTimeSheetDetailRepo.Update(weeklyTimeSheetDetail);

        }
        public void FinalizeTimeSheet(int id)
        {
            WeeklyTimeSheet weeklyTimeSheet = _weeklyTimeSheetRepo.GetFirstOrDefault(u => u.TimeSheetId == id);
            if (weeklyTimeSheet != null)
            {
                weeklyTimeSheet.IsFinalized = true;
                _weeklyTimeSheetRepo.Update(weeklyTimeSheet);
            }
        }

    }
}
