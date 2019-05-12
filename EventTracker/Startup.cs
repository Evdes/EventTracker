
using EventTracker.Services.Repos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using EventTracker.DAL.SqlData;
using EventTracker.Services.Repos.SqlData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Authentication.Cookies;
using EventTracker.Models.UserProfiles;
using Microsoft.AspNetCore.Identity;
using EventTracker.Services.EmailSender;
using EventTracker.DAL;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity.UI.Pages;
using System.IO;

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

            services.AddIdentity<UserProfile, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<EventTrackerDbContext>();
            services.AddDbContext<EventTrackerDbContext>(
                options => options.UseSqlServer(_config.GetConnectionString("EventTrackerDevDb")));
            services.AddScoped<IEventRepo, EventSqlData>();
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
                                IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "text/html";

                        await context.Response.WriteAsync("<html lang=\"en\"><body>\r\n");
                        await context.Response.WriteAsync("ERROR!<br><br>\r\n");

                        var exceptionHandlerPathFeature =
                            context.Features.Get<IExceptionHandlerPathFeature>();

                        // Use exceptionHandlerPathFeature to process the exception (for example, 
                        // logging), but do NOT expose sensitive error information directly to 
                        // the client.

                        if (exceptionHandlerPathFeature?.Error is FileNotFoundException)
                        {
                            await context.Response.WriteAsync("File error thrown!<br><br>\r\n");
                        }

                        await context.Response.WriteAsync("<a href=\"/\">Home</a><br>\r\n");
                        await context.Response.WriteAsync("</body></html>\r\n");
                        await context.Response.WriteAsync(new string(' ', 512)); // IE padding
                    });
                });
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
