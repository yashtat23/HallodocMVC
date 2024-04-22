using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DataModels;
using Microsoft.AspNetCore.Http;

namespace DataAccess.CustomModels
{
    public class PatientInfoModel
    {
        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage ="This Type of Symptoms is not valid")]
        public string? symptoms { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "First name is required and must be properly formatted.")]
        public string? firstname { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "Last name is required and must be properly formatted.")]
        public string? lastname { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        public DateOnly Dateofbirth { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter a valid email address like a@g.com")]
        public string email { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        public string phonenumber { get; set; }

        [Required(ErrorMessage = "Street is required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "Please enter a valid street")]
        public string? street { get; set; }

        [Required(ErrorMessage = "City is required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "Please enter a valid city")]
        public string? city { get; set; }

        [Required(ErrorMessage = "State is required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "Please enter a valid State")]
        public string state { get; set; }

        [Required(ErrorMessage = "Zipcode is required")]
        [RegularExpression(@"^\d{6}(?:[-\s]\d{4})?$", ErrorMessage = "Please enter a valid ZipCode")]
        public string? zipcode { get; set; }

        //[Required(ErrorMessage = "Room Number is required")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage ="This Type of Room No Is not Valid")]
        public string? roomno { get; set; }

        [Required(ErrorMessage ="Password is required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
      ErrorMessage = "8 characters long (one uppercase, one lowercase letter, one digit, and one special character.")]
        public string? password { get; set; }

        [Compare("password",ErrorMessage ="Password Missmatch")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
      ErrorMessage = "8 characters long (one uppercase, one lowercase letter, one digit, and one special character.")]
        public string? confirmPassword { get; set; }

        public List<IFormFile>? File { get; set; }

    }

    public class FamilyReqModel
    {
        [Required(ErrorMessage = "First Name is required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "First name is required and must be properly formatted.")]
        public string? firstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "Last name is required and must be properly formatted.")]
        public string? lastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter a valid email address like a@g.com")]
        public string? email { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        public string? phoneNo { get; set; }

        //public string relation { get; set; }
        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "This Type of Symptoms is not valid")]
        public string? symptoms { get; set; }

        [Required(ErrorMessage = "Patient Firstname is required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "First name is required and must be properly formatted.")]
        public string patientFirstName { get; set; }

        [Required(ErrorMessage = "Patient Lastname is required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "Last name is required and must be properly formatted.")]
        public string? patientLastName { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        public DateOnly patientDob { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter a valid email address like a@g.com")]
        public string? patientEmail { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        public string? patientPhoneNo { get; set; }

        [Required(ErrorMessage = "Street is required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "Please enter a valid street")]
        public string? street { get; set; }

        [Required(ErrorMessage = "City is required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "Please enter a valid city")]
        public string? city { get; set; }

        [Required(ErrorMessage = "State is required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "Please enter a valid State")]
        public string? state { get; set; }

        [Required(ErrorMessage = "Zipcode is required")]
        [RegularExpression(@"^\d{6}(?:[-\s]\d{4})?$", ErrorMessage = "Please enter a valid ZipCode")]
        public string? zipCode { get; set; }

        [RegularExpression(@"^[0-9]*$", ErrorMessage = "This Type of Room No Is not Valid")]
        public int? roomNo { get; set; }

        public IFormFile? File { get; set; }

        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "Realtion is required and must be properly formatted.")]
        public string? relation { get; set; }

    }

    public class ConciergeReqModel
    {
        [Required(ErrorMessage = "Please Enter Your First Name")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "First name is required and must be properly formatted.")]
        public string firstName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "Last name is required and must be properly formatted.")]
        public string? lastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter a valid email address like a@g.com")]
        public string? email { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        public string? phoneNo { get; set; }

        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "Hotel name is required and must be properly formatted.")]
        public string? hotelName { get; set; }

        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "This Type of Symptoms is not valid")]
        public string? symptoms { get; set; }

        [Required(ErrorMessage = "Patient Firstname is required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "First name is required and must be properly formatted.")]
        public string? patientFirstName { get; set; }
        [Required(ErrorMessage = "Patient Lastname is required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "Last name is required and must be properly formatted.")]
        public string? patientLastName { get; set; }
        [Required(ErrorMessage = "Date of Birth is required")]
        public DateTime patientDob { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter a valid email address like a@g.com")]
        public string patientEmail { get; set; }
        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        public string? patientPhoneNo { get; set; }

        [Required(ErrorMessage = "Street is required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "Please enter a valid street")]
        public string? street { get; set; }

        [Required(ErrorMessage = "city is required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "Please enter a valid city")]
        public string? city { get; set; }
        [Required(ErrorMessage = "State is required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "Please enter a valid State")]
        public string? state { get; set; }

        [RegularExpression(@"^\d{6}(?:[-\s]\d{4})?$", ErrorMessage = "Please enter a valid ZipCode")]
        public string? zipCode { get; set; }
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "This Type of Room No Is not Valid")]
        public int? roomNo { get; set; }
    }

    public class BusinessReqModel
    {
        [Required(ErrorMessage = "Please Enter Your Firstname")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "First name is required and must be properly formatted.")]
        public string firstName { get; set; }

        [Required(ErrorMessage = "Please Enter Your Lastname")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "First name is required and must be properly formatted.")]
        public string lastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter a valid email address like a@g.com")]
        public string email { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        public string? phoneNo { get; set; }

        public string? businessName { get; set; }

        public string? caseNo { get; set; }

        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "This Type of Symptoms is not valid")]
        public string? symptoms { get; set; }

        [Required(ErrorMessage = "Please Enter Your Firstname")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "First name is required and must be properly formatted.")]
        public string? patientFirstName { get; set; }

        [Required(ErrorMessage = "Please Enter Your Lastname")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "Last name is required and must be properly formatted.")]
        public string? patientLastName { get; set; }

        [Required(ErrorMessage = "Please Enter Your Date of Birth")]
        public DateTime patientDob { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter a valid email address like a@g.com")]
        public string? patientEmail { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        public string? patientPhoneNo { get; set; }

        [Required(ErrorMessage = "Street is required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "Please enter a valid street")]
        public string? street { get; set; }

        [Required(ErrorMessage = "city is required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "Please enter a valid city")]
        public string? city { get; set; }
        [Required(ErrorMessage = "State is required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "Please enter a valid State")]
        public string? state { get; set; }
        [RegularExpression(@"^\d{6}(?:[-\s]\d{4})?$", ErrorMessage = "Please enter a valid ZipCode")]
        public string? zipCode { get; set; }
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "This Type of Room No Is not Valid")]
        public int? roomNo { get; set; }

    }

    //Dashboard

    public class MedicalHistory
    {

        public int reqId { get; set; }
        public DateTime createdDate { get; set; }
        public int currentStatus { get; set; }
        public List<string> document { get; set; }
        public int? IntDate { get; set; }
        public string StrMonth { get; set; }
        public int? IntYear { get; set; }
        public string ConfirmationNumber { get; set; }

        public int docCount { get; set; }

    }
    public class MedicalHistoryList
    {
        public List<MedicalHistory>? medicalHistoriesList { get; set; }
        public int? id { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
    }

    public class Profile
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DateOfBirth { get; set; }

        public string? PhoneNo { get; set; }

        public string? Street { get; set; }

        public string? City { get; set; }

        public string? ZipCode { get; set; }

        public string? State { get; set; }

        public string? Email { get; set; }
        public int? userId { get; set; }
        public int? IntDate { get; set; }
        public int? IntYear { get; set; }
        public string? StrMonth { get; set; }
        public int? isMobileCheck { get; set; }
    }

    public class subinformation
    {
        public List<PatientInfoModel> subinformationModels { get; set; }
    }

    public class DocumentModel
    {
        public List<Requestwisefile>? files { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public int? ReqId { get; set; }

        public List<IFormFile>? uploadedFiles { get; set; }
        public string? confirmationnumber { get; set; }
    }

}