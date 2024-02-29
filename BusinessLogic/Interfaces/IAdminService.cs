using DataAccess.CustomModel;
using DataAccess.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IAdminService
    {
        //bool AdminLogin(AdminLogin adminLogin);
        Aspnetuser GetAspnetuser(string email);

        List<AdminDashTableModel> GetRequestsByStatus(int status);

        StatusCountModel GetStatusCount();

        ViewCaseViewModel ViewCaseViewModel(int Requestclientid, int RequestTypeId);

        ViewNotesViewModel ViewNotes(int ReqId);

        bool UpdateAdminNotes(string additionalNotes,int reqId);
    }
}
