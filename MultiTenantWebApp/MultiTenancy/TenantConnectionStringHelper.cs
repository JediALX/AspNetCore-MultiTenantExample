using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MultiTenantWebApp.MultiTenancy
{
    public class TenantConnectionStringHelper : IConnectionStringHelper
    {
        private readonly ITenantResolver tenantResolver;
        private readonly IConfiguration configuration;

        public TenantConnectionStringHelper(ITenantResolver tenantResolver, IConfiguration configuration)
        {
            this.tenantResolver = tenantResolver ?? throw new ArgumentNullException(nameof(tenantResolver));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string GetConnectionString()
        {
            var defaultConnectionStringName = "DefaultConnection";
            var actualConnectionStringName = defaultConnectionStringName;
            var currentTenant = tenantResolver.GetCurrentTenant();
            if (!string.IsNullOrEmpty(currentTenant))
                actualConnectionStringName = currentTenant;

            return configuration.GetConnectionString(actualConnectionStringName)
                ?? configuration.GetConnectionString(defaultConnectionStringName);
        }
    }

    public static class TenantConnectionStringHelperExtensions
    {
        /// <summary>
        /// Add a scoped instance of TenantConnectionStringHelper to container.
        /// </summary>
        /// <returns></returns>
        public static IServiceCollection AddTenantConnectionStringHelper(this IServiceCollection services)
        {
            return services.AddScoped<IConnectionStringHelper, TenantConnectionStringHelper>();
        }
    }
}
