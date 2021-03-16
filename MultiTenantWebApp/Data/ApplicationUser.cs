using Microsoft.AspNetCore.Identity;

namespace MultiTenantWebApp.Data
{
    public class ApplicationUser: IdentityUser<int>
    {
        public ApplicationUser()
        {
        }
    }
}
