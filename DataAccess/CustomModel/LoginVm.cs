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
    }
