using System.ComponentModel.DataAnnotations;

namespace CarpetStore.ViewModels
{
    public class RegisterViewModel
    {
        public RegisterViewModel() { }
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} needs to be at least {2} characters long", MinimumLength = 6)]
        public string Password { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password must match")]
        public string ConfirmPassword { get; set; }

        //[Required]
        //[Display(Name = "Role Name")]
        //public string RoleName { get; set; }
    }
}
