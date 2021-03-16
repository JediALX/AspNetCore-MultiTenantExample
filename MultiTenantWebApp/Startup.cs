using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using MultiTenantWebApp.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MultiTenantWebApp.MultiTenancy;
using MultiTenantWebApp.MultiTenancy.Middleware;

namespace MultiTenantWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add tenant resolver (don't forget to also add tenant injector middleware to the pipeline
            services.AddTenantResolver();

            // Add connection string helper, so we always know which connection string to use based on current tenant
            services.AddTenantConnectionStringHelper();

            // Replace default "AddDbContext" with something similar to the following.
            services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
            {
                var connectionStringHelper = serviceProvider.GetRequiredService<IConnectionStringHelper>();
                var connectionString = connectionStringHelper.GetConnectionString()
                    ?? Configuration.GetConnectionString("DefaultConnection");
                options.UseSqlite(connectionString);
            });

            // Add database updater, used during startup (see Program.cs)
            services.AddScoped<IDatabaseUpdater, DatabaseUpdater>();

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Don't forget to add tenant injector middleware to the pipeline
            app.UseTenantInjector();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
