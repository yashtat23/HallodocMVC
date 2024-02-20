using BusinessLogic.Repository;
using DataAccess.CustomModel;
using DataAccess.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IAuth
    {
        public User Login(LoginVm loginVm);
        
        //sending email
        public Task EmailSendar(string email, string subject, string message);

        public void Resetreq(forgotpassword forgotpassword);

    }
}
