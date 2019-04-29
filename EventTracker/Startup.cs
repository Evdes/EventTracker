
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

namespace EventTracker
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddScoped<EventTrackerDbContext>();
            services.AddDbContext<EventTrackerDbContext>(
                options => options.UseSqlServer(_config.GetConnectionString("EventTrackerDevDb")));
            services.AddScoped<IEventRepo, EventSqlData>();
            services.AddScoped<IUserProfileRepo, UserProfileSqlData>();
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

            app.UseStaticFiles();
            app.UseMvc(ConfigureRoutes);
            
            
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }

        private void ConfigureRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute("default", "{controller=events}/{action=AllUpcomingEvents}/{id?}");
        }
    }
}
