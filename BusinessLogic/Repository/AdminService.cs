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

namespace BusinessLogic.Repository
{
    public class AdminService : IAdminService
    {

        private readonly ApplicationDbContext _db;

        public AdminService(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool AdminLogin(AdminLogin adminLogin)
        {
            return _db.Aspnetusers.Any(x=>x.Email == adminLogin.Email && x.Passwordhash==adminLogin.Password);
        }

        public List<AdminDashTableModel> GetRequestsByStatus()
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
                        };

            var result = query.ToList();

            return result;
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
    }
}
