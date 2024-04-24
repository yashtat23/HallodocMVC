using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DataModels;
using Microsoft.AspNetCore.Http;

namespace DataAccess.Customemodel
{
    public class PatientModel
    {
        public int patientid { get; set; }

        public string Firstname { get; set;}
        public string Lastname { get; set;}
        public string Email { get; set;}
        public int? Age { get; set;}
        public string phonenuber { get; set;}
        public string gender { get; set;}
        public string Dieases { get; set;}
        public string Doctor { get; set;}

        public string Specialist { get; set;}

    }

    public class PatientModelList
    {
        public List<PatientModel> patientModelList { get; set; }

    }

    public class Patientform
    {

        public int? patientid { get; set;}
        [Required(ErrorMessage ="First Name is Required")]
        public string Fname { get; set;}
        [Required(ErrorMessage = "Last Name is Required")]
        public string Lname { get; set;}
        [Required(ErrorMessage ="Email is Required")]
        public string email { get; set;}
        [Required(ErrorMessage ="Age is Required")]
        public int? Age { get; set;}
        public string Gender { get; set;}
        [Required(ErrorMessage ="Phone Number is required")]
        public string phonenumber { get; set;}

        public string? Disease { get; set;}
        public string? Specialist { get; set;}

        public List<Doctor> doctorList { get; set; }

    }


}
