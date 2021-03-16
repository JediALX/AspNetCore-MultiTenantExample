using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiTenantWebApp.MultiTenancy
{
    public interface IDatabaseUpdater
    {
        public Task UpdateAllAsync(CancellationToken cancellationToken = default);
    }
}
