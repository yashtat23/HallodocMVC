﻿using DataAccess.CustomModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IAdminService
    {
        bool AdminLogin(AdminLogin adminLogin);

        List<AdminDashTableModel> GetRequestsByStatus();
    }
}
