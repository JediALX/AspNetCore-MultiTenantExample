namespace MultiTenantWebApp.MultiTenancy
{
    /// <summary>
    /// Interface to tenant name resolvers.
    /// </summary>
    public interface ITenantResolver
    {
        string GetCurrentTenant();
    }
}
