using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CarpetStore.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DisplayName("Remember Me?")]
        public bool RememberMe { get; set; }
    }
}
