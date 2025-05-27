using Microsoft.AspNetCore.Identity;

namespace CarpetStore.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; }
    }
}
