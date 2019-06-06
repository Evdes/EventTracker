using EventTracker.DAL.SqlData;
using EventTracker.Models.UserProfiles;
using EventTracker.Services.EmailSender;
using EventTracker.Services.Repos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventTracker.Web.Extensions
{
    public static class StartupServiceExtensions
    {
        public static void ConfigureIdentity (this IServiceCollection services)
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
        }

        public static void ConfigureDbContext(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<EventTrackerDbContext>(
                options => options.UseSqlServer(config.GetConnectionString("EventTrackerDevDb")));
        }

        public static void ConfigureCustomServices(this IServiceCollection services)
        {
            services.AddScoped<IEventRepo, EventSqlRepo>();
            services.AddTransient<IEmailSender, EmailSender>();
        }
    }
}
