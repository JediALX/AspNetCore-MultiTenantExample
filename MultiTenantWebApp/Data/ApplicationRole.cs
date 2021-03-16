using Microsoft.AspNetCore.Identity;

namespace MultiTenantWebApp.Data
{
    public class ApplicationRole: IdentityRole<int>
    {
        public ApplicationRole()
        {
        }
    }
}
