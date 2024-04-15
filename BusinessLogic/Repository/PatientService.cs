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
using BusinessLogic.Services;
using DataAccess.Enums;
using DataAccess.Enum;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;

namespace BusinessLogic.Repository
{
    public class PatientService : IPatientService
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _http;
        private readonly IJwtService _jwtService;

        public PatientService(ApplicationDbContext db,IHttpContextAccessor http,IJwtService jwtService)
        {
            _db = db;
            _http = http;
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

        public bool CreateAccount(CreateAccountModel model)
        {
            try
            {
                Aspnetuser asp = new();
                User user = new();
                var existUser = _db.Aspnetusers.FirstOrDefault(r => r.Email == model.email);

                if (existUser == null)
                {
                    asp.Id = Guid.NewGuid().ToString();
                    asp.Email = model.email;
                    asp.Username = model.email.Split('@')[0];
                    asp.Passwordhash = GenerateSHA256(model.password);
                    asp.Createddate = DateTime.Now;
                    _db.Aspnetusers.Add(asp);

                    user.Aspnetuserid = asp.Id;
                    user.Email = model.email;
                    user.Firstname = asp.Username;
                    user.Createdby = asp.Id;
                    user.Createddate = DateTime.Now;
                    _db.Users.Add(user);
                    _db.SaveChanges();
                }
                else
                {
                    existUser.Passwordhash = GenerateSHA256(model.password);
                    _db.Aspnetusers.Update(existUser);
                    _db.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        public void SendRegistrationEmailCreateRequest(string email, string registrationLink)
        {
            string senderEmail = "mailto:tatva.dotnet.yashvariya@outlook.com";
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

        public LoginResponseViewModel PatientLogin(LoginVm model)
        {
            var user = _db.Aspnetusers.Include(u=>u.Aspnetuserroles).Where(u => u.Email == model.Email).FirstOrDefault();

            if (user == null)
                return new LoginResponseViewModel() { Status = ResponseStatus.Failed, Message = "User Not Found" };
            if (user.Passwordhash == null)
                return new LoginResponseViewModel() { Status = ResponseStatus.Failed, Message = "There is no Password with this Account" };
            if (user.Passwordhash == model.Password)
            {
                var jwtToken = _jwtService.GetJwtToken(user);

                return new LoginResponseViewModel() { Status = ResponseStatus.Success, Token = jwtToken };
            }

            return new LoginResponseViewModel() { Status = ResponseStatus.Failed, Message = "Password does not match" };
        }

        public bool AddPatientInfo(PatientInfoModel patientInfoModel)
        {

            var stateMain = _db.Regions.Where(r => r.Name.ToLower() == patientInfoModel.state.ToLower().Trim()).FirstOrDefault();
            if (stateMain == null)
            {
                return false;
            }

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

                Aspnetuserrole role = new Aspnetuserrole();
                role.Userid = aspnetuser1.Id;
                role.Roleid = 2;
                _db.Aspnetuserroles.Add(role);

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
                u.Status = (int)StatusEnum.Unassigned;
                u.Regionid = stateMain.Regionid;

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
            request.Userid = u.Userid;

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
            info.Regionid = stateMain.Regionid;


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
            return true;
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

        public bool AddFamilyReq(FamilyReqModel familyReqModel, string createaccountLink)
        {
            var statemain = _db.Regions.Where(x => x.Name.ToLower() == familyReqModel.state.ToLower().Trim()).FirstOrDefault();
            if (statemain == null)
            {
                return false;
            }
            User user = new User();
            Aspnetuser asp = new Aspnetuser();
            var existUser = _db.Aspnetusers.FirstOrDefault(r => r.Email == familyReqModel.patientEmail);

            if (existUser == null)
            {
                asp.Id = Guid.NewGuid().ToString();
                asp.Username = familyReqModel.patientFirstName + "_" + familyReqModel.patientLastName;
                asp.Email = familyReqModel.patientEmail;
                asp.Phonenumber = familyReqModel.patientPhoneNo;
                asp.Createddate = DateTime.Now;
                _db.Aspnetusers.Add(asp);
                _db.SaveChanges();

                user.Aspnetuserid = asp.Id;
                user.Firstname = familyReqModel.patientFirstName;
                user.Lastname = familyReqModel.patientFirstName;
                user.Email = familyReqModel.patientEmail;
                user.Mobile = familyReqModel.patientPhoneNo;
                user.City = familyReqModel.city;
                user.State = familyReqModel.state;
                user.Street = familyReqModel.street;
                user.Zipcode = familyReqModel.zipCode;
                user.Intyear = int.Parse(familyReqModel.patientDob.ToString("yyyy"));
                user.Intdate = int.Parse(familyReqModel.patientDob.ToString("dd"));
                user.Strmonth = familyReqModel.patientDob.ToString("MMM");
                user.Createdby = asp.Id;
                user.Createddate = DateTime.Now;
                user.Regionid = statemain.Regionid;
                _db.Users.Add(user);
                _db.SaveChanges();

                try
                {
                    SendRegistrationEmailCreateRequest(familyReqModel.patientEmail, createaccountLink);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            else
            {
                user = _db.Users.Where(m => m.Email == familyReqModel.patientEmail).FirstOrDefault();
            }
            Request request = new Request();
            request.Requesttypeid = (int)RequestTypeEnum.Family;
            request.Status = (int)StatusEnum.Unassigned;
            request.Createddate = DateTime.Now;
            request.Isurgentemailsent = new BitArray(1, false);
            request.Firstname = familyReqModel.firstName;
            request.Lastname = familyReqModel.lastName;
            request.Phonenumber = familyReqModel.phoneNo;
            request.Email = familyReqModel.email;
            request.Relationname = familyReqModel.relation;
            request.Userid = user.Userid;

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
            info.Regionid = statemain.Regionid;

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
            return true;
        }

        public bool AddConciergeReq(ConciergeReqModel conciergeReqModel, string createAccountLink)
        {
            var stateMain = _db.Regions.Where(x => x.Name.ToLower() == conciergeReqModel.state.ToLower().Trim()).FirstOrDefault();

            if (stateMain == null)
            {
                return false;
            }
            User user = new User();
            Aspnetuser asp = new Aspnetuser();
            var existUser = _db.Aspnetusers.FirstOrDefault(r => r.Email == conciergeReqModel.patientEmail);

            if (existUser == null)
            {
                asp.Id = Guid.NewGuid().ToString();
                asp.Username = conciergeReqModel.patientFirstName + "_" + conciergeReqModel.patientLastName;
                asp.Email = conciergeReqModel.patientEmail;
                asp.Phonenumber = conciergeReqModel.patientPhoneNo;
                asp.Createddate = DateTime.Now;
                _db.Aspnetusers.Add(asp);
                _db.SaveChanges();

                user.Aspnetuserid = asp.Id;
                user.Firstname = conciergeReqModel.patientFirstName;
                user.Lastname = conciergeReqModel.patientLastName;
                user.Email = conciergeReqModel.patientEmail;
                user.Mobile = conciergeReqModel.patientPhoneNo;
                //user.City = conciergeReqModel.city;
                //user.State = conciergeReqModel.state;
                //user.Street = conciergeReqModel.street;
                //user.Zipcode = conciergeReqModel.zipCode;
                user.Intyear = int.Parse(conciergeReqModel.patientDob.ToString("yyyy"));
                user.Intdate = int.Parse(conciergeReqModel.patientDob.ToString("dd"));
                user.Strmonth = conciergeReqModel.patientDob.ToString("MMM");
                user.Createdby = asp.Id;
                user.Createddate = DateTime.Now;
                user.Regionid = stateMain.Regionid;
                _db.Users.Add(user);
                _db.SaveChanges();




                try
                {
                    SendRegistrationEmailCreateRequest(conciergeReqModel.patientEmail, createAccountLink);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            else
            {
                user = _db.Users.Where(m => m.Email == conciergeReqModel.patientEmail).FirstOrDefault();
            }

            Request request = new Request();
            request.Requesttypeid = (int)RequestTypeEnum.Concierge;
            request.Status = (int)StatusEnum.Unassigned;
            request.Createddate = DateTime.Now;
            request.Isurgentemailsent = new BitArray(1, false);
            request.Firstname = conciergeReqModel.firstName;
            request.Lastname = conciergeReqModel.lastName;
            request.Phonenumber = conciergeReqModel.phoneNo;
            request.Email = conciergeReqModel.email;
            request.Relationname = "Concierge";
            request.Userid = user.Userid;

            _db.Requests.Add(request);
            _db.SaveChanges();

            Requestclient info = new Requestclient();
            info.Requestid = request.Requestid;
            info.Notes = conciergeReqModel.symptoms;
            info.Firstname = conciergeReqModel.patientFirstName;
            info.Lastname = conciergeReqModel.patientLastName;
            info.Phonenumber = conciergeReqModel.patientPhoneNo;
            info.Email = conciergeReqModel.patientEmail;
            info.Regionid = stateMain.Regionid;


            _db.Requestclients.Add(info);
            _db.SaveChanges();

            Concierge concierge = new Concierge();
            concierge.Conciergename = conciergeReqModel.firstName + " " + conciergeReqModel.lastName;
            concierge.Createddate = DateTime.Now;
            concierge.Regionid = stateMain.Regionid;
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

            return true;
        }

        public bool AddBusinessReq(BusinessReqModel businessReqModel, string createAccountLink)
        {
            var stateMain = _db.Regions.Where(x => x.Name.ToLower() == businessReqModel.state.ToLower().Trim()).FirstOrDefault();

            if (stateMain == null)
            {
                return false;
            }
            User user = new User();
            Aspnetuser asp = new Aspnetuser();
            var existUser = _db.Aspnetusers.FirstOrDefault(r => r.Email == businessReqModel.patientEmail);

            if (existUser == null)
            {
                asp.Id = Guid.NewGuid().ToString();
                asp.Username = businessReqModel.patientFirstName + "_" + businessReqModel.patientLastName;
                asp.Email = businessReqModel.patientEmail;
                asp.Phonenumber = businessReqModel.patientPhoneNo;
                asp.Createddate = DateTime.Now;
                _db.Aspnetusers.Add(asp);
                _db.SaveChanges();

                user.Aspnetuserid = asp.Id;
                user.Firstname = businessReqModel.patientFirstName;
                user.Lastname = businessReqModel.patientLastName;
                user.Email = businessReqModel.patientEmail;
                user.Mobile = businessReqModel.patientPhoneNo;
                user.City = businessReqModel.city;
                user.State = businessReqModel.state;
                user.Street = businessReqModel.street;
                user.Zipcode = businessReqModel.zipCode;
                user.Intyear = int.Parse(businessReqModel.patientDob.ToString("yyyy"));
                user.Intdate = int.Parse(businessReqModel.patientDob.ToString("dd"));
                user.Strmonth = businessReqModel.patientDob.ToString("MMM");
                user.Createdby = asp.Id;
                user.Createddate = DateTime.Now;
                user.Regionid = stateMain.Regionid;
                _db.Users.Add(user);
                _db.SaveChanges();




                try
                {
                    SendRegistrationEmailCreateRequest(businessReqModel.patientEmail, createAccountLink);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            else
            {
                user = _db.Users.Where(m => m.Email == businessReqModel.patientEmail).FirstOrDefault();
            }
            Request request = new Request();
            request.Requesttypeid = (int)RequestTypeEnum.Business;
            request.Status = (int)StatusEnum.Unassigned;
            request.Createddate = DateTime.Now;
            request.Isurgentemailsent = new BitArray(1, false);
            request.Firstname = businessReqModel.firstName;
            request.Lastname = businessReqModel.lastName;
            request.Phonenumber = businessReqModel.phoneNo;
            request.Email = businessReqModel.email;
            request.Relationname = "Business";
            request.Userid = user.Userid;

            _db.Requests.Add(request);
            _db.SaveChanges();

            Requestclient info = new Requestclient();
            info.Requestid = request.Requestid;
            info.Notes = businessReqModel.symptoms;
            info.Firstname = businessReqModel.patientFirstName;
            info.Lastname = businessReqModel.patientLastName;
            info.Phonenumber = businessReqModel.patientPhoneNo;
            info.Email = businessReqModel.patientEmail;
            info.Regionid = stateMain.Regionid;
            info.Street = businessReqModel.street;
            info.City = businessReqModel.city;
            info.State = businessReqModel.state;
            info.Zipcode = businessReqModel.zipCode;
            info.Regionid = stateMain.Regionid;
            _db.Requestclients.Add(info);
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
            return true;
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
            if (user.Intdate == null && user.Intyear == null && user.Strmonth == "")
            {
                Profile obj = new()
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
                return obj;
            }
            else
            {
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
                    DateOfBirth = new DateTime(Convert.ToInt32(user.Intyear), DateTime.ParseExact(user.Strmonth, "MMM", CultureInfo.InvariantCulture).Month, Convert.ToInt32(user.Intdate)),
                    //isMobileCheck = user.Ismobile[0] ? 1 : 0,

                };
                return profile;
            }
           
            
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

                    //existingUser.Ismobile[1] = profile.isMobileCheck == 1 ? true : false;
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
        public PatientInfoModel FetchData(string email)
        {
            User? user = _db.Users.FirstOrDefault(i => i.Email == email);
            var BirthDay = Convert.ToInt32(user.Intdate);
            var BirthMonth = user.Strmonth;
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
