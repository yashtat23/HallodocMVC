using DataAccess.CustomModel;
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

        bool CreateAccount(CreateAccountModel model);

        void SendRegistrationEmailCreateRequest(string email, string registrationLink);

        LoginResponseViewModel PatientLogin(LoginVm model);
        bool AddPatientInfo(PatientInfoModel patientInfoModel);

        bool AddFamilyReq(FamilyReqModel familyReqModel, string createaccountLink);

        bool AddConciergeReq(ConciergeReqModel conciergeReqModel, string createAccountLink);

        bool AddBusinessReq(BusinessReqModel businessReqModel, string createAccountLink);

        Task<bool> IsEmailExists(string email);

        //MedicalHistoryList GetMedicalHistory(int userid);

        MedicalHistoryList GetMedicalHistory(string email);

        //List<MedicalHistory> GetMedicalHistory(User user);
        DocumentModel GetAllDocById(int requestId);

        bool UploadDocuments(List<IFormFile> files, int reqId);

        List<PatientInfoModel> subinformation(PatientInfoModel patientInfoModel);

        void Editing(string email, User model);
        Profile GetProfile(int userid);
        bool EditProfile(Profile profile);

        bool StoreData(PatientInfoModel patientRequestModel);   

        PatientInfoModel FetchData(string email);

        bool SomeElseReq(FamilyReqModel model, string createAccountLink, string loginid);
    }
}
