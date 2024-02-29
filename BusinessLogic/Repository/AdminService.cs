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
            var aspNetUser = _db.Aspnetusers.FirstOrDefault(x => x.Email == email);
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
                            Requestclientid=rc.Requestclientid,
                            reqId = r.Requestid
                        };

            var result = query.ToList();

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

            return result;
        }

        public bool UpdateAdminNotes(string additionalNotes,int reqId)
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
            ViewCaseViewModel viewCaseViewModel = new()
            {
                Requestclientid=obj.Requestclientid,
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
                RequestTypeId = RequestTypeId
            };
            return viewCaseViewModel;
        }

        public ViewNotesViewModel ViewNotes(int ReqId)
        {
            var data = _db.Requeststatuslogs.Where(a => a.Requestid == ReqId).FirstOrDefault();
            var data1 = _db.Requestnotes.Where(a => a.Requestid == ReqId).FirstOrDefault();

            var list = _db.Requeststatuslogs.Where(x => x.Requestid == ReqId).ToList();
            ViewNotesViewModel model = new ViewNotesViewModel();
            if (model == null)
            {


                model.TransferNotes = null;
                model.PhysicianNotes = null;
                model.AdminNotes = null;

            }

            model.TransferNotes = list;
            if (data1 != null)
            {
                model.PhysicianNotes = data1.Physiciannotes;
                model.AdminNotes = data1.Adminnotes;
            }





            return model;

        }
    }
}
