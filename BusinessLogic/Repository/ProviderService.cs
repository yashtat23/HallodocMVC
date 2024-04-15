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

namespace BusinessLogic.Repository
{
    public class ProviderService : IProviderService
    {

        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _http;
        private readonly IJwtService _jwtService;

        public ProviderService(ApplicationDbContext db, IHttpContextAccessor http, IJwtService jwtService)
        {
            _db = db;
            _http = http;
            _jwtService = jwtService;
        }


        public void acceptCase(int requestId, string loginUserId)
        {
            //string? aspId = HttpContext.Session.GetString("UserId");
            var req = _db.Requests.FirstOrDefault(x => x.Requestid == requestId);
            
            //int phyId = _db.Physicians.Where(x => x.Aspnetuserid == aspId).Select(i => i.Physicianid).FirstOrDefault();

            Request? req_data = _db.Requests.Where(i => i.Requestid == requestId).FirstOrDefault();
            var reqStatLog = _db.Requeststatuslogs.Where(i => i.Requestid == requestId).FirstOrDefault();

            int phyId = _db.Physicians.Where(x => x.Aspnetuserid == loginUserId).Select(x=>x.Physicianid).FirstOrDefault();
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

    }
}
