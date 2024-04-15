using System.ComponentModel.DataAnnotations;
using DataAccess.DataModels;
using Microsoft.AspNetCore.Http;
using DataAccess.Enums;

namespace DataAccess.CustomModel
{
    public class LoginVm
    {
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }

    public class LoginResponseViewModel
    {
        public ResponseStatus Status { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
    }

    public class forgotpassword
    {
        [Required(ErrorMessage = "This Is required")]
        public string forgotemail { get; set; }
    }

    public class CreateAccountModel
    {
        [Required(ErrorMessage = "Email is required")]
        //[RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter a valid email address.")]
        public string? email { get; set; }

        [Required(ErrorMessage = "Password is required")]
      //  [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
      //ErrorMessage = "8 characters long (one uppercase, one lowercase letter, one digit, and one special character.")]
        public string? password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        //[Compare("password", ErrorMessage = "Password Missmatch")]
        public string? confirmPassword { get; set; }
    }

}
