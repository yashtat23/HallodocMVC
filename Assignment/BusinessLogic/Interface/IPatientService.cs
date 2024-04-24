using DataAccess.Customemodel;
using DataAccess.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interface
{
    public interface IPatientService
    {
        PatientModelList patientinfo();
        bool SubmitPatientform(Patientform patientform);
        bool DeletePatient(int patientid);
        bool UpdatePatient(Patientform patientform,int patientid);
        Patientform GetPatient(int patientid);

        List<Doctor> fetchdoctor();
    }
}
