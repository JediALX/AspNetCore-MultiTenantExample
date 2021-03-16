using System;
namespace MultiTenantWebApp.MultiTenancy
{
    public interface IConnectionStringHelper
    {
        string GetConnectionString();
    }
}
