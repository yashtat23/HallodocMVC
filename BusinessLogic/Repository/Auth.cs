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
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
            var obj = _context.Aspnetusers.ToList();
            User user = new User();
            user = null;

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

            string mail = "tatva.dotnet.yashvariya@outlook.com";
            string password = "Itzvariya@23";

            SmtpClient client = new SmtpClient("smtp.office365.com")
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
                Subject = "Set up your Account",
                IsBodyHtml = true,

                Body = message
            };

            mailMessage.To.Add(email);
            client.Send(mailMessage);
            return Task.CompletedTask;
        }

        public void Resetreq(forgotpassword forgotpassword)
        {
            var receiver = forgotpassword.forgotemail;
            var subject = "Create Account";
            var message = "Tap on link for Create Account Hello this is dummy project: https://localhost:7130/Patient/patientresetpass";
            EmailSendar(receiver, subject, message);
        }
    }


}
