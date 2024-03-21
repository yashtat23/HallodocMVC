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
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Globalization;

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
                u.Intyear = patientInfoModel.Dateofbirth.Year;
                u.Intdate = patientInfoModel.Dateofbirth.Day;
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
            info.Intyear = patientInfoModel.Dateofbirth.Year;
            info.Intdate = patientInfoModel.Dateofbirth.Day;
            info.Strmonth = patientInfoModel.Dateofbirth.ToString("MMM");
            info.Regionid = 1;
            

            _db.Requestclients.Add(info)
;
            _db.SaveChanges();

            var regionData = _db.Regions.Where(x => x.Regionid == u.Regionid).FirstOrDefault();

            var count = (from req in _db.Requests where req.Userid == u.Userid && req.Createddate.Date == DateTime.Now.Date select req).Count();

            request.Confirmationnumber = $"{regionData.Abbreviation.Substring(0, 2)}{request.Createddate.Day.ToString().PadLeft(2, '0')}{request.Createddate.Month.ToString().PadLeft(2, '0')}{(request.Createddate.Year % 100).ToString().PadLeft(2, '0')}{u.Lastname.ToUpper().Substring(0, 2)}{u.Firstname.ToUpper().Substring(0, 2)}{count.ToString().PadLeft(4, '0')}";

            _db.Requests.Update(request);
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


        public MedicalHistoryList GetMedicalHistory(int userid)
        {
            var user = _db.Users.FirstOrDefault(x => x.Userid == userid);


            var medicalhistory = (from request in _db.Requests
                                  join requestfile in _db.Requestwisefiles
                                  on request.Requestid equals requestfile.Requestid
                                  where request.Email == user.Email && request.Email != null
                                  group requestfile by request.Requestid into groupedFiles
                                  select new MedicalHistory
                                  {

                                      reqId = groupedFiles.Select(x => x.Request.Requestid).FirstOrDefault(),
                                      createdDate = groupedFiles.Select(x => x.Request.Createddate).FirstOrDefault(),
                                      currentStatus = groupedFiles.Select(x => x.Request.Status).FirstOrDefault(),
                                      document = groupedFiles.Select(x => x.Filename.ToString()).ToList(),
                                      ConfirmationNumber = groupedFiles.Select(x => x.Request.Confirmationnumber).FirstOrDefault(),
                                  }).ToList();

            MedicalHistoryList medicalHistoryList = new()
            {
                medicalHistoriesList = medicalhistory,
                id = userid,
                firstName = user.Firstname,
                lastName = user.Lastname
            };

            return medicalHistoryList;
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

        public Profile GetProfile(int userid)
        {
            var user = _db.Users.FirstOrDefault(x => x.Userid == userid);
            Profile profile = new()
            {
                FirstName = user.Firstname,
                LastName = user.Lastname,
                Email = user.Email,
                PhoneNo = user.Mobile,
                State = user.State,
                City = user.City,
                Street = user.Street,
                ZipCode = user.Zipcode,
                //DateOfBirth = new DateTime(Convert.ToInt32(user.Intyear), DateTime.ParseExact(user.Strmonth, "MMM", CultureInfo.InvariantCulture).Month, Convert.ToInt32(user.Intdate)),
                //isMobileCheck = user.Ismobile[0] ? 1 : 0,

            };
            return profile;
        }

        public bool EditProfile(Profile profile)
        {

            try
            {
                var existingUser = _db.Users.Where(x => x.Userid == profile.userId).FirstOrDefault();
                if (existingUser != null)
                {

                    existingUser.Firstname = profile.FirstName;
                    existingUser.Lastname = profile.LastName;
                    existingUser.Mobile = profile.PhoneNo;
                    existingUser.Street = profile.Street;
                    existingUser.City = profile.City;
                    existingUser.State = profile.State;
                    existingUser.Zipcode = profile.ZipCode;
                    existingUser.Intdate = profile.DateOfBirth.Day;
                    existingUser.Strmonth = profile.DateOfBirth.ToString("MMM");
                    existingUser.Intyear = profile.DateOfBirth.Year;

                    //existingUser.Ismobile[0] = profile.isMobileCheck == 1 ? true : false;
                    _db.Users.Update(existingUser);
                    _db.SaveChanges();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex) { return false; }
        }

        //me & someoneelse page
        public PatientInfoModel FetchData(int userid)
        {
            User? user = _db.Users.FirstOrDefault(i => i.Userid == userid);
            var BirthDay = Convert.ToInt32(user.Intdate);
            var BirthMonth = Convert.ToInt32(user.Strmonth);
            var BirthYear = Convert.ToInt32(user.Intyear);
            var userdata = new PatientInfoModel()
            {
                firstname = user.Firstname,
                lastname = user.Lastname,
                email = user.Email,
                phonenumber = user.Mobile,
            };
            return userdata;
        }
        public void StoreData(PatientInfoModel patientRequestModel, int reqTypeid, int userid)

        {
            User? user = _db.Users.FirstOrDefault(i => i.Userid == userid);

            Request reqdata = new Request()                             //Request
            {
                Userid = userid,
                Requesttypeid = reqTypeid,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Email = user.Email,
                Phonenumber = user.Mobile,
                Status = 1,
                Createddate = DateTime.Now,
                Isurgentemailsent = new BitArray(1),
            };

            _db.Requests.Add(reqdata);
            _db.SaveChanges();


            int reqid = reqdata.Requestid;



            if (patientRequestModel.File != null)
            {
                foreach (IFormFile file in patientRequestModel.File)
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
                            Requestid = reqdata.Requestid,
                            Createddate = DateTime.Now
                        };

                        _db.Requestwisefiles.Add(requestwisefile);
                        _db.SaveChanges();
                    };
                }
            }


            var reqclientdata = new Requestclient()                 //request client
            {
                Requestid = reqid,
                Firstname = patientRequestModel.firstname,
                Lastname = patientRequestModel.lastname,
                Email = patientRequestModel.email,
                Notiemail = patientRequestModel.email,
                Phonenumber = patientRequestModel.phonenumber,
                Address = patientRequestModel.street + " " + patientRequestModel.city + " " + patientRequestModel.state + " " + patientRequestModel.zipcode,
                Notimobile = patientRequestModel.phonenumber,
                State = patientRequestModel.state,
                Street = patientRequestModel.street,
                City = patientRequestModel.city,
            };
            _db.Requestclients.Add(reqclientdata);
            _db.SaveChanges();

        }

        public void ReqforSomeoneElse(FamilyReqModel familyFriendRequestForm, int userid)
        {
            string? aspid = _db.Aspnetusers.Where(Au => Au.Email == familyFriendRequestForm.patientEmail).Select(Au => Au.Id).FirstOrDefault();


            var user = _db.Users.FirstOrDefault(x => x.Userid == userid);




            Request data = new Request()                             //Request
            {

                Userid = userid,
                Requesttypeid = 2,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Email = user.Email,
                Phonenumber = user.Mobile,
                //Relationname = familyFriendRequestForm.Relation,
                Status = 1,
                Createddate = DateTime.Now,
                Isurgentemailsent = new BitArray(1),
            };

            _db.Requests.Add(data);
            _db.SaveChanges();


            int reqid = data.Requestid;

            var reqClient = new Requestclient()             //RequestClient
            {
                Requestid = reqid,
                Firstname = familyFriendRequestForm.firstName,
                Lastname = familyFriendRequestForm.lastName,
                Email = familyFriendRequestForm.email,
                Phonenumber = familyFriendRequestForm.phoneNo,
                Notes = familyFriendRequestForm.symptoms,
                Address = familyFriendRequestForm.roomNo + " " + familyFriendRequestForm.street + " " + familyFriendRequestForm.city + " " + familyFriendRequestForm.state + " " + familyFriendRequestForm.zipCode,
                //Intyear = familyFriendRequestForm.patientDob.,
                //Intdate = familyFriendRequestForm.DateOfBirth.Day,
                //Strmonth = familyFriendRequestForm.DateOfBirth.Month.ToString(),
                Street = familyFriendRequestForm.street,
                City = familyFriendRequestForm.city,
                State = familyFriendRequestForm.state,
                Zipcode = familyFriendRequestForm.zipCode,

            };
            _db.Requestclients.Add(reqClient);
            _db.SaveChanges();



            if (familyFriendRequestForm.File != null && familyFriendRequestForm.File.Length > 0)
            {
                //get file name
                var fileName = Path.GetFileName(familyFriendRequestForm.File.FileName);

                //define path
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles", fileName);

                // Copy the file to the desired location
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    familyFriendRequestForm.File.CopyTo(stream);
                }

                Requestwisefile requestwisefile = new()
                {
                    Filename = fileName,
                    Requestid = data.Requestid,
                    Createddate = DateTime.Now
                };

                _db.Requestwisefiles.Add(requestwisefile);
                _db.SaveChanges();
            };


        }
    }
}
