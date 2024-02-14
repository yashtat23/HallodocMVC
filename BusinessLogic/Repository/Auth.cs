using BusinessLogic.Interfaces;
using System;
using System.Collections.Generic;

using BusinessLogic.Interfaces;
using DataAccess.CustomModel;
using Microsoft.EntityFrameworkCore;
using DataAccess.DataContext;


namespace BusinessLogic.Repository
{
    public class Auth : IAuth
    {
        private readonly ApplicationDbContext _context;
        public Auth(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool ValidateLogin(LoginVm loginVm)
        {
            return _context.Aspnetusers.Any(Au => Au.Email == loginVm.Email && Au.Passwordhash == loginVm.Password);
        }
    }


}
