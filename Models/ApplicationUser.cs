using Microsoft.AspNetCore.Identity;

namespace CarpetStore.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; }
        public DateTime? AccountCreated { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
