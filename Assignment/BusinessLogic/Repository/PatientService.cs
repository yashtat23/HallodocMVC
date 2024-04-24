using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DataModels;
using DataAccess.DataContext;
using DataAccess.Customemodel;
using BusinessLogic.Interface;

namespace BusinessLogic.Repository
{
    public class PatientService :IPatientService
    {

        private readonly ApplicationDbContext _db;

        public PatientService(ApplicationDbContext db)
        {
            _db = db;
        }

        public PatientModelList patientinfo()
        {
            
            var patient = (from p in _db.Patients
                          join d in _db.Doctors on p.Doctorid equals d.Doctorid
                          where p.Isdeleted == null
                          select new PatientModel
                          {
                              patientid = p.Id,
                              Firstname = p.Firstname,
                              Lastname = p.Lastname,
                              Email = p.Email,
                              Age = p.Age,
                              phonenuber = p.Phoneno,
                              gender = p.Gender,
                              Dieases = p.Disease,
                              Specialist = d.Specialist,
                          }).ToList();

            PatientModelList patientModelList = new()
            {
                patientModelList = patient
            };

            return patientModelList;
                
        }

        public List<Doctor> fetchdoctor()
        {
           var doctor = _db.Doctors.ToList();
            return doctor;
        }

        public bool SubmitPatientform(Patientform patientform)
        {
            try
            {
                Patient patient = new Patient();
                patient.Firstname = patientform.Fname;
                patient.Lastname = patientform.Lname;
                patient.Email = patientform.email;
                patient.Age = patientform.Age;
                patient.Phoneno = patientform.phonenumber;
                patient.Gender = "Male";
                patient.Disease = patientform.Disease;
                patient.Doctorid = 1;



                //patient.Createddate = DateTime.Now(DateTime.Now);
                _db.Patients.Add(patient);
                _db.SaveChanges();
                return true;
            }
            catch {
                return false;
            }

        }

        public bool DeletePatient(int patientid)
        {
            var patient = _db.Patients.Where(x=>x.Id == patientid).FirstOrDefault();
            try
            {
                patient.Isdeleted = true;
                _db.Patients.Update(patient);
                _db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Patientform GetPatient(int patientid)
        {
            var patient = _db.Patients.FirstOrDefault(x=>x.Id == patientid);
            Patientform obj = new()
            {
                Fname = patient.Firstname,
                Lname = patient.Lastname,
                email = patient.Email,
                Age = patient.Age,
                phonenumber = patient.Phoneno,
                Disease = patient.Disease,
            };
            return obj;
        }

        public bool UpdatePatient(Patientform patientform,int patientid)
        {
            var patient = _db.Patients.Where(x => x.Id == patientid).FirstOrDefault();
            try
            {
                patient.Firstname = patientform.Fname;
                patient.Lastname = patientform.Lname;
                patient.Email = patientform.email;
                patient.Age = patientform.Age;
                patient.Gender = patientform.Gender;
                patient.Phoneno = patientform.phonenumber;
                patient.Disease = patientform.Disease;
                _db.Patients.Update(patient);
                _db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
