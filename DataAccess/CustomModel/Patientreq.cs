using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DataAccess.CustomModels
{
    public class PatientInfoModel
    {
        public string symptoms { get; set; }

        //[Required(ErrorMessage = "First Name is required")]
        public string firstname { get; set; }

        public string lastname { get; set; }

        //[Required(ErrorMessage = "Date of Birth is required")]
        public DateTime Dateofbirth { get; set; }

        //[Required(ErrorMessage = "Email is required")]
        public string email { get; set; }

        //[Required(ErrorMessage = "Phone Number is required")]
        public string phonenumber { get; set; }

        //[Required(ErrorMessage = "Street is required")]
        public string street { get; set; }

        //[Required(ErrorMessage = "City is required")]
        public string city { get; set; }

        //[Required(ErrorMessage = "State is required")]
        public string state { get; set; }

        //[Required(ErrorMessage = "Zipcode is required")]
        public string zipcode { get; set; }

        //[Required(ErrorMessage = "Room Number is required")]
        public string roomno { get; set; }

        public List<IFormFile>? File { get; set; }

    }

    public class FamilyReqModel
    {
        
        public string firstName { get; set; }

        public string lastName { get; set; }

        public string email { get; set; }

        public string phoneNo { get; set; }

        //public string relation { get; set; }

        public string symptoms { get; set; }

        public string patientFirstName { get; set; }

        public string patientLastName { get; set; }

        public DateTime patientDob { get; set; }
       
        public string patientEmail { get; set; }
        
        public string patientPhoneNo { get; set; }
        
        public string street { get; set; }
     
        public string city { get; set; }
      
        public string state { get; set; }

        public string zipCode { get; set; }

        //[Required(ErrorMessage = "Room Number is required")]
        public int roomNo { get; set; }

        public IFormFile File { get; set;}

    }

    public class ConciergeReqModel
    {
        
        public string firstName { get; set; }
        
        public string lastName { get; set; }
        
        public string email { get; set; }
        
        public string phoneNo { get; set; }
       
        public string hotelName { get; set; }

        public string symptoms { get; set; }
        
        public string patientFirstName { get; set; }
     
        public string patientLastName { get; set; }
       
        public DateTime patientDob { get; set; }
    
        public string patientEmail { get; set; }
        
        public string patientPhoneNo { get; set; }
        
        public string street { get; set; }

        public string city { get; set; }
       
        public string state { get; set; }

        public string zipCode { get; set; }

        public int roomNo { get; set; }
    }

    public class BusinessReqModel
    {
        public string firstName { get; set; }

       
        public string lastName { get; set; }

        public string email { get; set; }

       
        public string phoneNo { get; set; }

        
        public string businessName { get; set; }

       
        public string caseNo { get; set; }

        public string symptoms { get; set; }

        
        public string patientFirstName { get; set; }

        
        public string patientLastName { get; set; }

       
        public DateTime patientDob { get; set; }

        
        public string patientEmail { get; set; }

        
        public string patientPhoneNo { get; set; }

       
        public string street { get; set; }

       
        public string city { get; set; }

        public string state { get; set; }
     
        public string zipCode { get; set; }

        public int roomNo { get; set; }

    }

     //Dashboard

    public class PatientDashboard
    {
        public DateTime createdDate { get; set; }
        public string currentStatus { get; set; }
        public string document { get; set; }
    }

    public class PatientDashboardInfo
    {
        public List<PatientDashboard> patientDashboardItems { get; set; }
    }

    public class MedicalHistory
    {
        public int redId { get; set; }
        public DateTime createdDate { get; set; }
        public string currentStatus { get; set; }
        public List<string> document { get; set; }
    }
    public class MedicalHistoryList
    {
        public List<MedicalHistory> medicalHistoriesList { get; set; }
    }
}
