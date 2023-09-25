using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Business.Models.Account
{
    public class AccountRegisterModel // login (uygulamaya giriş) view'ı için herhangi bir entity üzerinden oluşturmadığımız model
    {
        [Required(ErrorMessage = "{0} is required!")]
        [MinLength(3, ErrorMessage = "{0} must be minimum {1} characters!")]
        [MaxLength(50, ErrorMessage = "{0} must be maximum {1} characters!")]
        [DisplayName("User Name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "{0} is required!")]
        [MinLength(3, ErrorMessage = "{0} must be minimum {1} characters!")]
        [MaxLength(10, ErrorMessage = "{0} must be maximum {1} characters!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "{0} is required!")]
        [MinLength(3, ErrorMessage = "{0} must be minimum {1} characters!")]
        [MaxLength(10, ErrorMessage = "{0} must be maximum {1} characters!")]
        [Compare("Password", ErrorMessage = "{0} and {1} must be same!")]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
