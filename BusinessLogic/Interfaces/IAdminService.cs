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

       DashboardModel GetRequestsByStatus(int status,int CurrentPage);

        DashboardModel GetRequestByRegion(int regionId, int tabNo);

        StatusCountModel GetStatusCount();

        ViewCaseViewModel ViewCaseViewModel(int Requestclientid, int RequestTypeId);

        ViewNotesViewModel ViewNotes(int ReqId);

        bool UpdateAdminNotes(string additionalNotes,int reqId);

        CancelCaseModel CancelCase(int reqId);

        bool SubmitCancelCase(CancelCaseModel cancelCaseModel);

        public List<Region> GetRegion();

        public JsonArray GetPhysician(int regionId);

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

        void TransferReqPostData(AssignCaseModel assignCaseModel, int requestId);

        bool Clearcase(int requestId);

        SendAgreement Agreement(int requestId);
        void SendAgreementEmail(SendAgreement model, string link);
        //void Resetreq(string Email);
        //Task EmailSendar(string email, string subject, string message);

        bool ReviewAgree(ReviewAgreement Agreement);
        CancelAngreement CancelAgreement(int requestClientId);
        bool CancelAgreement(CancelAngreement cancel);

        CloseCase closeCase(int reqId);

        bool EditCloseCase(CloseCase closeCase);

        bool ChangeCloseCase(CloseCase closeCase);

        EncounterFormModel EncounterForm(int reqId);

        bool SubmitEncounterForm(EncounterFormModel model);

        bool CreateRequest(CreateRequestModel model, string sessionEmail);

        //AdminProfile ProfileInfo(int adminId);
        //MyProfileModel MyProfile(string sessionEmail);
        bool VerifyState(string state);

        List<ProviderModel> GetProvider();
        bool StopNotification(int phyId);

        public ProviderModel providerContact(int PhysicianId);

        void providerContactEmail(int phyIdMain, string msg);

        EditPhysicianAccount EditPhysician(int PhysicianId);

        bool EditSavePhysician(EditPhysicianAccount editPhysicianAccount);

        List<Physicianlocation> GetPhysicianlocations();

        MyProfileModel MyProfile(string sessionEmail);

        bool ResetPassword(string tokenEmail, string resetPassword);
        bool SubmitAdminInfo(MyProfileModel model, string tokenEmail);
        bool SubmitBillingInfo(MyProfileModel model, string tokenEmail);

        List<AccountAccess> AccountAccess();
        bool DeleteRole(int roleId);
        List<AdminDashTableModel> Expert(int tabNo);
        CreateAccess FetchRole(short selectedValue);
        bool CreateRole(List<int> menuIds, string roleName, short accountType);

        bool RoleExists(string roleName, short accountType);
        CreateAdminAccount RegionList();

        CreateProviderAccount GetProviderList();
        bool CreateAdminAccount(CreateAdminAccount obj, string email);
        //void SendRegistrationproviderContactEmail(string provider, string msg, string sessionEmail, int phyIdMain);

        void InsertFileAfterRename(IFormFile file, string path, string updateName);
        void CreateProviderAccount(CreateProviderAccount model);
    }   
}
