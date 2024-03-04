using System;
using BusinessLogic.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.CustomModel;
using DataAccess.DataModels;
using DataAccess.DataContext;
using DataAccess.CustomModels;
using System.Collections;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Repository
{
    public class PatientService : IPatientService
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _http;

        public PatientService(ApplicationDbContext db,IHttpContextAccessor http)
        {
            _db = db;
            _http = http;
        }

        public void AddPatientInfo(PatientInfoModel patientInfoModel)
        {


            var aspnetuser = _db.Aspnetusers.Where(m => m.Email == patientInfoModel.email).FirstOrDefault();
            User u = new User();
            if (aspnetuser == null)
            {
                Aspnetuser aspnetuser1 = new Aspnetuser();
                aspnetuser1.Id = Guid.NewGuid().ToString();
                aspnetuser1.Passwordhash = patientInfoModel.password;
                aspnetuser1.Email = patientInfoModel.email;
                string username = patientInfoModel.firstname + patientInfoModel.lastname;
                aspnetuser1.Username = username;
                aspnetuser1.Phonenumber = patientInfoModel.phonenumber;
                aspnetuser1.Createddate = DateTime.Now;
                aspnetuser1.Modifieddate = DateTime.Now;
                _db.Aspnetusers.Add(aspnetuser1);



                u.Aspnetuserid = aspnetuser1.Id;
                u.Firstname = patientInfoModel.firstname;
                u.Lastname = patientInfoModel.lastname;
                u.Email = patientInfoModel.email;
                u.Mobile = patientInfoModel.phonenumber;
                u.Street = patientInfoModel.street;
                u.City = patientInfoModel.city;
                u.State = patientInfoModel.state;
                u.Zipcode = patientInfoModel.zipcode;
                u.Createdby = patientInfoModel.firstname + patientInfoModel.lastname;
                u.Intyear = int.Parse(patientInfoModel.Dateofbirth.ToString("yyyy"));
                u.Intdate = int.Parse(patientInfoModel.Dateofbirth.ToString("dd"));
                u.Strmonth = patientInfoModel.Dateofbirth.ToString("MMM");
                u.Createddate = DateTime.Now;
                u.Modifieddate = DateTime.Now;
                u.Status = 1;
                u.Regionid = 1;

                _db.Users.Add(u);
                _db.SaveChanges();
            }
            else
            {
                u = _db.Users.Where(m => m.Email == patientInfoModel.email).FirstOrDefault();
            }


            Request request = new Request();
            request.Requesttypeid = 2;
            request.Status = 1;
            request.Createddate = DateTime.Now;
            request.Isurgentemailsent = new BitArray(1);
            request.Firstname = patientInfoModel.firstname;
            request.Lastname = patientInfoModel.lastname;
            request.Phonenumber = patientInfoModel.phonenumber;
            request.Email = patientInfoModel.email;



            _db.Requests.Add(request);
            _db.SaveChanges();

            Requestclient info = new Requestclient();
            info.Request = request;
            info.Requestid = request.Requestid;
            info.Notes = patientInfoModel.symptoms;
            info.Firstname = patientInfoModel.firstname;
            info.Lastname = patientInfoModel.lastname;
            info.Phonenumber = patientInfoModel.phonenumber;
            info.Email = patientInfoModel.email;
            info.Street = patientInfoModel.street;
            info.City = patientInfoModel.city;
            info.State = patientInfoModel.state;
            info.Zipcode = patientInfoModel.zipcode;
            info.Intyear = int.Parse(patientInfoModel.Dateofbirth.ToString("yyyy"));
            info.Intdate = int.Parse(patientInfoModel.Dateofbirth.ToString("dd"));
            info.Strmonth = patientInfoModel.Dateofbirth.ToString("MMM");
            info.Regionid = 1;

            _db.Requestclients.Add(info)
;
            _db.SaveChanges();



            if (patientInfoModel.File != null)
            {
                foreach (IFormFile file in patientInfoModel.File)
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
                            Requestid = request.Requestid,
                            Createddate = DateTime.Now
                        };

                        _db.Requestwisefiles.Add(requestwisefile);
                        _db.SaveChanges();
                    };
                }
            }
        }


        //public Task<bool> IsEmailExists(string email)
        //{
        //    bool isExist = _db.Aspnetusers.Any(x => x.Email == email);
        //    if (isExist)
        //    {
        //        return Task.FromResult(true);
        //    }
        //    return Task.FromResult(false);
        //}

        public void AddFamilyReq(FamilyReqModel familyReqModel)
        {
            Request request = new Request();
            request.Requesttypeid = 3;
            request.Status = 1;
            request.Createddate = DateTime.Now;
            request.Isurgentemailsent = new BitArray(1);
            request.Firstname = familyReqModel.firstName;
            request.Lastname = familyReqModel.lastName;
            request.Phonenumber = familyReqModel.phoneNo;
            request.Email = familyReqModel.email;
            //request.Relationname = familyReqModel.relation;

            _db.Requests.Add(request);
            _db.SaveChanges();

            Requestclient info = new Requestclient();
            info.Requestid = request.Requestid;
            info.Notes = familyReqModel.symptoms;
            info.Firstname = familyReqModel.patientFirstName;
            info.Lastname = familyReqModel.patientLastName;
            info.Phonenumber = familyReqModel.patientPhoneNo;
            info.Email = familyReqModel.patientEmail;
            info.Street = familyReqModel.street;
            info.City = familyReqModel.city;
            info.State = familyReqModel.state;
            info.Zipcode = familyReqModel.zipCode;


            _db.Requestclients.Add(info);
            _db.SaveChanges();

            if (familyReqModel.File != null && familyReqModel.File.Length > 0)
            {
                //get file name
                var fileName = Path.GetFileName(familyReqModel.File.FileName);

                //define path
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles", fileName);

                // Copy the file to the desired location
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    familyReqModel.File.CopyTo(stream);
                }

                Requestwisefile requestwisefile = new()
                {
                    Filename = fileName,
                    Requestid = request.Requestid,
                    Createddate = DateTime.Now
                };

                _db.Requestwisefiles.Add(requestwisefile);
                _db.SaveChanges();
            };

        }

        public void AddConciergeReq(ConciergeReqModel conciergeReqModel)
        {
            Request request = new Request();
            request.Requesttypeid = 4;
            request.Status = 1;
            request.Createddate = DateTime.Now;
            request.Isurgentemailsent = new BitArray(1);
            request.Firstname = conciergeReqModel.firstName;
            request.Lastname = conciergeReqModel.lastName;
            request.Phonenumber = conciergeReqModel.phoneNo;
            request.Email = conciergeReqModel.email;
            request.Relationname = "Concierge";

            _db.Requests.Add(request);
            _db.SaveChanges();

            Requestclient info = new Requestclient();
            info.Requestid = request.Requestid;
            info.Notes = conciergeReqModel.symptoms;
            info.Firstname = conciergeReqModel.patientFirstName;
            info.Lastname = conciergeReqModel.patientLastName;
            info.Phonenumber = conciergeReqModel.patientPhoneNo;
            info.Email = conciergeReqModel.patientEmail;


            _db.Requestclients.Add(info);
            _db.SaveChanges();

            Concierge concierge = new Concierge();
            concierge.Conciergename = conciergeReqModel.firstName + " " + conciergeReqModel.lastName;
            concierge.Createddate = DateTime.Now;
            concierge.Regionid = 1;
            concierge.Street = conciergeReqModel.street;
            concierge.City = conciergeReqModel.city;
            concierge.State = conciergeReqModel.state;
            concierge.Zipcode = conciergeReqModel.zipCode;

            _db.Concierges.Add(concierge);
            _db.SaveChanges();

            Requestconcierge reqCon = new Requestconcierge();
            reqCon.Requestid = request.Requestid;
            reqCon.Conciergeid = concierge.Conciergeid;

            _db.Requestconcierges.Add(reqCon);
            _db.SaveChanges();

        }

        public void AddBusinessReq(BusinessReqModel businessReqModel)
        {
            Request request = new Request();
            request.Requesttypeid = 1;
            request.Status = 1;
            request.Createddate = DateTime.Now;
            request.Isurgentemailsent = new BitArray(1);
            request.Firstname = businessReqModel.firstName;
            request.Lastname = businessReqModel.lastName;
            request.Phonenumber = businessReqModel.phoneNo;
            request.Email = businessReqModel.email;
            request.Relationname = "Business";

            _db.Requests.Add(request);
            _db.SaveChanges();

            Requestclient info = new Requestclient();
            info.Requestid = request.Requestid;
            info.Notes = businessReqModel.symptoms;
            info.Firstname = businessReqModel.patientFirstName;
            info.Lastname = businessReqModel.patientLastName;
            info.Phonenumber = businessReqModel.patientPhoneNo;
            info.Email = businessReqModel.patientEmail;

            _db.Requestclients.Add(info)
;
            _db.SaveChanges();

            Business business = new Business();
            business.Createddate = DateTime.Now;
            business.Name = businessReqModel.businessName;
            business.Phonenumber = businessReqModel.phoneNo;
            business.City = businessReqModel.city;
            business.Zipcode = businessReqModel.zipCode;

            _db.Businesses.Add(business);
            _db.SaveChanges();

            Requestbusiness requestbusiness = new Requestbusiness();
            requestbusiness.Businessid = business.Businessid;
            requestbusiness.Requestid = request.Requestid;

            _db.Requestbusinesses.Add(requestbusiness);
            _db.SaveChanges();

        }


        public Task<bool> IsEmailExists(string email)
        {
            bool isExist = _db.Aspnetusers.Any(x => x.Email == email);
            if (isExist)
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public List<MedicalHistory> GetMedicalHistory(User user)
        {
            var medicalhistory = (from request in _db.Requests
                                  join requestfile in _db.Requestwisefiles
                                  on request.Requestid equals requestfile.Requestid
                                  where request.Email == user.Email && request.Email != null
                                  group requestfile by request.Requestid into groupedFiles
                                  select new MedicalHistory
                                  {
                                      FirstName = user.Firstname,
                                      LastName = user.Lastname,
                                      PhoneNo = user.Mobile,
                                      dateOfBirth = user.Strmonth + user.Intdate + user.Intyear,
                                      Email = user.Email,
                                      Street = user.Street,
                                      City = user.City,
                                      State = user.State,
                                      ZipCode = user.Zipcode,
                                      reqId = groupedFiles.Select(x => x.Request.Requestid).FirstOrDefault(),
                                      createdDate = groupedFiles.Select(x => x.Request.Createddate).FirstOrDefault(),
                                      currentStatus = groupedFiles.Select(x => x.Request.Status).FirstOrDefault().ToString(),
                                      document = groupedFiles.Select(x => x.Filename.ToString()).ToList()
                                  }).ToList();
            return medicalhistory;
        }

        public IQueryable<Requestwisefile>? GetAllDocById(Int64 requestId)
        {
            var data = from request in _db.Requestwisefiles
                       where request.Requestid == requestId
                       select request;
            return data;
        }

        public void AddFile(IFormFile file)
        {
            var fileName = Path.GetFileName(file.FileName);

            //define path
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles", fileName);

            // Copy the file to the desired location
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            var u = _http.HttpContext.Session.GetInt32("rid");
            Requestwisefile requestwisefile = new()
            {
                Filename = fileName,
                Requestid = (int)u,
                Createddate = DateTime.Now
            };
            _db.Requestwisefiles.Add(requestwisefile);
            _db.SaveChanges();
        }

        public List<PatientInfoModel> subinformation(PatientInfoModel patientInfoModel)
        {
            var y = (from Request in _db.Requests
                     where patientInfoModel != null
                     select new PatientInfoModel
                     {
                         firstname = patientInfoModel.firstname,
                         lastname = patientInfoModel.lastname,
                         email = patientInfoModel.email,
                         phonenumber = patientInfoModel.phonenumber,
                     }).ToList();
        return y;
        }

        public void Editing(string email, User model)
        {
            var userdata = _db.Users.Where(x => x.Email == email).FirstOrDefault();

            if (userdata.Email == model.Email)
            {
                userdata.Firstname = model.Firstname;
                userdata.Lastname = model.Lastname;
                userdata.Mobile = model.Mobile;
                userdata.Email = model.Email;
                userdata.Street = model.Street;
                userdata.City = model.City;
                userdata.State = model.State;
                userdata.Zipcode = model.Zipcode;
                userdata.Modifieddate = DateTime.Now;

                _db.Users.Update(userdata);
                _db.SaveChanges();
            }

        }
    }
}
