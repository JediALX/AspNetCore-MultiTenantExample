using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MultiTenantWebApp.Data;

namespace MultiTenantWebApp.MultiTenancy
{
    /// <summary>
    /// Enables caller to apply migrations to all databases defined in appsettings.json
    /// </summary>
    public class DatabaseUpdater : IDatabaseUpdater
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<DatabaseUpdater> logger;

        public DatabaseUpdater(IConfiguration configuration, ILogger<DatabaseUpdater> logger)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.logger = logger;
        }

        public async Task UpdateAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var connectionStringsSection = configuration.GetSection("ConnectionStrings");
                foreach (var connectionString in connectionStringsSection.GetChildren())
                {
                    var databaseName = GetDatabaseName(connectionString.Value);

                    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                    optionsBuilder.UseSqlite(connectionString.Value);
                    using var ctx = new ApplicationDbContext(optionsBuilder.Options);
                    if ((await ctx.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return;

                        logger?.LogInformation("Database {0} has pending migrations and will be updated.", databaseName);
                        await ctx.Database.MigrateAsync(cancellationToken);
                        logger?.LogInformation("Database {0} has been updated.", databaseName);
                    }
                    else
                    {
                        logger?.LogInformation("Database {0} is already up to date.", databaseName);
                    }
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.LogError(ex, "Caould not check or update databases");
                }
                else
                {
                    throw;
                }
            }
        }

        private static string GetDatabaseName(string connectionString)
        {
            var csb = new SqliteConnectionStringBuilder(connectionString);
            return csb.DataSource;
        }
    }
}
