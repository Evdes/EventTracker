using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using EventTracker.DAL.SqlData;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Rewrite;
using EventTracker.Models.UserProfiles;
using Microsoft.AspNetCore.Identity;
using EventTracker.BLL.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;
using System.IO;
using System.Net;

namespace EventTracker
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureIdentity();
            services.ConfigureDbContext(_config);
            services.ConfigureCustomServices();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
                                IHostingEnvironment env,
                                EventTrackerDbContext ctx,
                                UserManager<UserProfile> userManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                var Seeder = new Seeder(ctx, userManager);
                Seeder.Seed();
            }
            else
            {
                app.UseExceptionHandler("/Error/500");
                app.UseHsts();
            }
            
            app.UseRewriter(new RewriteOptions()
                                .AddRedirectToHttpsPermanent());
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseStatusCodePagesWithReExecute("/error/{0}");
            app.UseMvc(ConfigureRoutes);
        }

        private void ConfigureRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute("default", "{controller=events}/{action=UpcomingEvents}/{id?}");
        }
    }
}
