using Common;
using Microsoft.AspNetCore.Identity;

namespace DataAccess.Data
{
    public class ApplicationUser: IdentityUser
    {
        public string Name { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string RefreshToken { get; set; }
    }
}
