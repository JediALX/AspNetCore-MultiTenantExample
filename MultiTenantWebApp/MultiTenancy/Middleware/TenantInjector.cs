using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace MultiTenantWebApp.MultiTenancy.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    /// <summary>
    /// Tenant Injector Middleware - determines tenant based on host name, then saves it
    /// in CURRENT_TENANT in HttpContext.Items
    /// </summary>
    public class TenantInjector
    {
        private readonly RequestDelegate _next;
        public const string CURRENT_TENANT_KEY = "CURRENT_TENANT";

        public TenantInjector(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var tenant = httpContext.Request.Host.Host;
            httpContext.Items.Add(CURRENT_TENANT_KEY, tenant);

            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class TenantInjectorExtensions
    {
        /// <summary>
        /// Add TenantInjector middleware to the HTTP request pipeline.
        /// </summary>
        public static IApplicationBuilder UseTenantInjector(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TenantInjector>();
        }
    }
}
