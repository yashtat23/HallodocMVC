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
        bool ValidateLogin(LoginVm loginVm);

        public interface IAuthentication
        {
            public bool ValidateLogin(LoginVm loginVm);
        }
    }
}
