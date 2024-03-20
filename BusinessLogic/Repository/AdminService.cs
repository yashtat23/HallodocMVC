using DataAccess.DataContext;
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

namespace BusinessLogic.Repository
{
    public class AdminService : IAdminService
    {

        private readonly ApplicationDbContext _db;

        public AdminService(ApplicationDbContext db)
        {
            _db = db;
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

        public List<AdminDashTableModel> GetRequestsByStatus(int tabNo)
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
                            reqId = r.Requestid
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
            var requeststatuslog = _db.Requeststatuslogs.Where(x => x.Requestid == ReqId).FirstOrDefault();
            ViewNotesViewModel model = new ViewNotesViewModel();
            if (model == null)
            {
                model.TransferNotes = null;
                model.PhysicianNotes = null;
                model.AdminNotes = null;
            }

            if (requestNotes != null)
            {
                model.PhysicianNotes = requestNotes.Physiciannotes;
                model.AdminNotes = requestNotes.Adminnotes;
            }
            if (requeststatuslog != null)
            {
                model.TransferNotes = requeststatuslog.Notes;
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
        public List<Physician> GetPhysician(int regionId)
        {
            var physician = _db.Physicians.Where(i => i.Regionid == regionId).ToList();
            return physician;
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
                ReqClientId=requestClientId,
                Firstname=request.Firstname+" "+request.Lastname,
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

        public async Task SubmitRequest(CreateRequestViewModel model)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    Aspnetuser aspnetuser = await _db.Aspnetusers.Where(x => x.Email == model.Email).FirstOrDefaultAsync();
                    User user = new User();
                    if (aspnetuser == null)
                    {
                        Aspnetuser aspnetuser1 = new Aspnetuser()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Username = model.FirstName + " " + model.LastName,
                            Email = model.Email,
                            Phonenumber = model.PhoneNumber,
                            Createddate = DateTime.Now

                        };

                        await _db.Aspnetusers.AddAsync(aspnetuser1);

                        user.Aspnetuserid = aspnetuser1.Id;
                        user.Userid = 9;
                        user.Firstname = model.FirstName;
                        user.Lastname = model.LastName;
                        user.Email = model.Email;
                        user.Mobile = model.PhoneNumber;
                        user.Zipcode = model.ZipCode;
                        user.State = model.State;
                        user.City = model.City;
                        user.Street = model.Street;
                        user.Intdate = model.DOB.Day;
                        user.Intyear = model.DOB.Year;
                        user.Strmonth = model.DOB.ToString("MMM");
                        user.Createddate = DateTime.Now;
                        user.Createdby = aspnetuser.Username;

                        await _db.Users.AddAsync(user);
                    }
                    else
                    {
                        user = _db.Users.Where(a => a.Email == model.Email).FirstOrDefault();
                    }

                    var regiondata = _db.Regions.Where(x => x.Regionid == user.Regionid).FirstOrDefault();
                    var requestcount = _db.Requests.Where(a => a.Createddate.Date == DateTime.Now.Date && a.Createddate.Month == DateTime.Now.Month && a.Createddate.Year == DateTime.Now.Year && a.Userid == user.Userid).ToList();
                    Request request = new Request()
                    {
                        Userid = user.Userid,
                        Requesttypeid = 1,
                        Firstname = model.FirstName,
                        Lastname = model.LastName,
                        Email = model.Email,
                        Phonenumber = model.PhoneNumber,
                        Status = (int)StatusEnum.Unassigned,
                        Createddate = DateTime.Now,
                        Isurgentemailsent = new BitArray(1),
                        Confirmationnumber = regiondata.Abbreviation + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Month.ToString().PadLeft(2, '0')
                                             + DateTime.Now.Year.ToString().Substring(2) + model.LastName.Substring(0, 2) + model.FirstName.Substring(0, 2) +
                                             (requestcount.Count() + 1).ToString().PadLeft(4, '0'),
                    };
                    await _db.Requests.AddAsync(request);
                    await _db.SaveChangesAsync();

                    Requestclient requestclient = new Requestclient()
                    {

                        Firstname = model.FirstName,
                        Lastname = model.LastName,
                        Email = model.Email,
                        Phonenumber = model.PhoneNumber,
                        Intdate = model.DOB.Day,
                        Intyear = model.DOB.Year,
                        Strmonth = model.DOB.ToString("MMM"),
                        State = model.State,
                        Street = model.Street,
                        City = model.City,
                        Zipcode = model.ZipCode,
                        Regionid = regiondata.Regionid,
                    };

                    Requestnote requestNote = new Requestnote()
                    {
                        Requestid = request.Requestid,
                        Adminnotes = model.Notes,
                        Createdby = "Admin",
                        Createddate = DateTime.Now,
                    };

                    await _db.Requestnotes.AddAsync(requestNote);
                    request.Requestclients.Add(requestclient);
                    await _db.Requestclients.AddAsync(requestclient);
                    await _db.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }


            }
        }

        public AdminProfile ProfileInfo(int adminId)
        {
            Admin? obj = _db.Admins.FirstOrDefault(x => x.Adminid == adminId);

            var region = _db.Regions.FirstOrDefault(x => x.Regionid == obj.Regionid).Name;
            var regionList = _db.Regions.ToList();

            AdminProfile profile = new()
            {
                UserName = obj.Firstname + obj.Lastname,
                AdminId = adminId.ToString(),
                //AdminPassword=obj.,
                Status = obj.Status,
                Role = obj.Roleid.ToString() ?? "",
                FirstName = obj.Firstname,
                LastName = obj.Lastname,
                AdminPhone = obj.Mobile,
                Email = obj.Email,
                ConfirmEmail = obj.Email,
                Address1 = obj.Address1,
                Address2 = obj.Address2,
                City = region,
                State = region,
                Zip = obj.Zip,
                BillingPhone = obj.Altphone,
                RegionList = regionList,
            };

            return profile;
        }

    }
}
