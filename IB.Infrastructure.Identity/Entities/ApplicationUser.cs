using Microsoft.AspNetCore.Identity;

namespace IB.Infrastructure.Identity.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
        public string IdNumber { get; set; }
    }
}
