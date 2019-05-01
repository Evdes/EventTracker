
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
            services.AddScoped<IUserProfileRepo, UserProfileSqlData>();
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
                                IHostingEnvironment env, 
                                EventTrackerDbContext context,
                                UserManager<UserProfile> userManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                //******Only use for dev purpose! Will seed database!*******////
                //var seeder = new Seeder(context, userManager);
                //seeder.SeedRoles();
                //seeder.SeedUsers().Wait();
            }

            app.UseRewriter(new RewriteOptions()
                                .AddRedirectToHttpsPermanent());
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(ConfigureRoutes);

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }

        private void ConfigureRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute("default", "{controller=events}/{action=AllUpcomingEvents}/{id?}");
        }
    }
}
