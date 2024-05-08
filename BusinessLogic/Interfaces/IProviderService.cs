using DataAccess.CustomModel;
using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IProviderService
    {
        DashboardModel GetRequestsByStatus(int tabNo, int CurrentPage, int phyid);
        public StatusCountModel GetStatusCount(int phyid);

        DashboardModel GetRequestByRegion(FilterModel filterModel, int phyid);

        void acceptCase(int requestId,string loginUserId);

        bool TransferRequest(TransferRequest model);
        void CallType(int requestId, short callType);
        void housecall(int requestId);
        bool PSubmitEncounterForm(EncounterFormModel model);
        Task<ConcludeCareViewModel> ConcludeCare(int request_id);
        Task UploadDocuments(ConcludeCareViewModel model, int request_id);
        Task ConcludeCase(ConcludeCareViewModel model, string email);
        int GetPhysicianId(string userid);

        MonthWiseScheduling PhysicianMonthlySchedule(string date, int status, string aspnetuserid);
        bool concludecaresubmit(int ReqId, string ProviderNote);
        bool finalizesubmit(int reqid);

        void SendRegistrationEmailCreateRequest(string email, string note, string sessionEmail);
        void RequestAdmin(RequestAdmin model, string sessionEmail);

        bool PCreateRequest(CreateRequestModel model, string sessionEmail, string createAccountLink);

        List<DateViewModel> GetDates();

        InvoicingViewModel GetInvoicingDataonChangeOfDate(DateOnly startDate, DateOnly endDate, int? PhysicianId, int? AdminID);

        InvoicingViewModel GetUploadedDataonChangeOfDate(DateOnly startDate, DateOnly endDate, int? PhysicianId, int pageNumber, int pagesize);

        InvoicingViewModel getDataOfTimesheet(DateOnly startDate, DateOnly endDate, int? PhysicianId, int? AdminID);

        void AprooveTimeSheet(InvoicingViewModel model, int? AdminID);

        void SubmitTimeSheet(InvoicingViewModel model, int? PhysicianId);

        void DeleteBill(int id);

        void FinalizeTimeSheet(int id);

    }
}
