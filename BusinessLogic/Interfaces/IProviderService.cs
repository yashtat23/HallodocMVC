using DataAccess.CustomModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IProviderService
    {
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
    }
}
