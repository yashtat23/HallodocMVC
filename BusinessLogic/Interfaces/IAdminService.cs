using DataAccess.CustomModel;
using DataAccess.DataModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
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

        CancelCaseModel CancelCase(int reqId);

        bool SubmitCancelCase(CancelCaseModel cancelCaseModel);

        public List<Region> GetRegion();

        public List<Physician> GetPhysician(int regionId);

        void AssignCasePostData(AssignCaseModel assignCaseModel, int requestId);

        BlockCaseModel BlockCase(int reqId);
        bool SubmitBlockCase(BlockCaseModel blockCaseModel);

        ViewUploadModel GetAllDocById(int requestId);

        bool UploadFiles(List<IFormFile> files, int reqId);

        bool DeleteFileById(int reqFileId);

        bool DeleteAllFiles(List<string> filename, int reqId);

        Order FetchOrder(int reqId);

        JsonArray FetchVendors(int selectedValue);

        Healthprofessional VendorDetails(int selectedValue);
        Task SendOrderDetails(Order order);
    }
}
