using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MultiTenantWebApp.MultiTenancy.Middleware;

namespace MultiTenantWebApp.MultiTenancy
{
    /// <summary>
    /// A tenant name resolver which returns the tenant name injected by middleware
    /// TenantInjector (required)
    /// </summary>
    public class TenantResolver: ITenantResolver
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public TenantResolver(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        private string CurrentTenant { get; set; }

        private void SetTenant()
        {
            object currentTenant = null;
            if (httpContextAccessor.HttpContext?.Items.TryGetValue(TenantInjector.CURRENT_TENANT_KEY, out currentTenant) == true &&
                currentTenant != null)
            {
                CurrentTenant = currentTenant.ToString();
            }
        }

        public string GetCurrentTenant()
        {
            if (string.IsNullOrEmpty(CurrentTenant))
            {
                SetTenant();
            }
            return CurrentTenant;
        }
    }

    public static class TenantResolverExtensions
    {
        /// <summary>
        /// Add a scoped instance of TenantResolver to container.
        /// </summary>
        /// <returns></returns>
        public static IServiceCollection AddTenantResolver(this IServiceCollection services)
        {
            return services.AddScoped<ITenantResolver, TenantResolver>();
        }
    }
}
