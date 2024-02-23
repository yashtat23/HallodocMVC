using DataAccess.CustomModels;
using DataAccess.DataModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccess.CustomModels.MedicalHistory;

namespace BusinessLogic.Interfaces
{
    public interface IPatientService
    {
        void AddPatientInfo(PatientInfoModel patientInfoModel);

        void AddFamilyReq(FamilyReqModel familyReqModel);

        void AddConciergeReq(ConciergeReqModel conciergeReqModel);

        void AddBusinessReq(BusinessReqModel businessReqModel);

        Task<bool> IsEmailExists(string email);

        List<MedicalHistory> GetMedicalHistory(User user);
        IQueryable<Requestwisefile>? GetAllDocById(Int64 requestId);

        void AddFile(IFormFile file);

        List<PatientInfoModel> subinformation(PatientInfoModel patientInfoModel);

        //public userProfile getUserData(int userid);
    }
}
