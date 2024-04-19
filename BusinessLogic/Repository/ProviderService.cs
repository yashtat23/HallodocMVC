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

namespace BusinessLogic.Repository
{
    public class ProviderService : IProviderService
    {

        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _http;
        private readonly IJwtService _jwtService;
        private IWebHostEnvironment _environment;

        public ProviderService(ApplicationDbContext db, IHttpContextAccessor http, IJwtService jwtService, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _http = http;
            _jwtService = jwtService;
            _environment = hostEnvironment;
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

    }
}
