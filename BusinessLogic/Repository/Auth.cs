using BusinessLogic.Interfaces;
using System;
using System.Collections.Generic;

using DataAccess.CustomModel;
using Microsoft.EntityFrameworkCore;
using DataAccess.DataContext;
using DataAccess.CustomModels;
using DataAccess.DataModels;
using System.Net.Mail;
using System.Net;

namespace BusinessLogic.Repository
{
    public class Auth : IAuth
    {
        private readonly ApplicationDbContext _context;
        public Auth(ApplicationDbContext context)
        {
            _context = context;
        }

        public User Login(LoginVm loginvm)
        {

            //bool isExist = _db.Aspnetusers.Any(x => x.Email == loginModel.Email && x.Passwordhash == loginModel.Password);
            //if (isExist)
            //{
            //    var user = _db.Users.FirstOrDefault(x => x.Aspnetuserid ==  )
            //}
            //return user;
            var obj = _context.Aspnetusers.ToList();
            User user = new User();

            //string hashPassword = GenerateSHA256(patient.Password);
            //match the email and pw with database entry
            foreach (var item in obj)
            {
                if (item.Email == loginvm.Email && item.Passwordhash == loginvm.Password)
                {
                    user = _context.Users.FirstOrDefault(u => u.Aspnetuserid == item.Id);
                    return user;
                }
            }
            return user;
        }

        public Task EmailSendar(string email, string subject, string message)
        {

            var mail = "yashvariya2024@gmail.com";
            var password = "mwgg tnsg guhv xcrv";

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, password)
            };
            return client.SendMailAsync(new MailMessage(from: mail, to: email, subject, message));
        }
        
        public void Resetreq(forgotpassword forgotpassword)
        {
            var receiver = forgotpassword.forgotemail;
            var subject = "Create Account";
            var message = "Tap on link for Create Account: https://localhost:44311/Patient/patientreg";

            EmailSendar(receiver, subject, message);
        }



    }


}
