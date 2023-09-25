using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Business.Models.Account
{
    public class AccountLoginModel // login (uygulamaya giriş) view'ı için herhangi bir entity üzerinden oluşturmadığımız model
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

        public string ReturnUrl { get; set; }
    }
}
