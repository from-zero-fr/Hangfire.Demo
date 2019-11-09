using Hangfire.Demo.Client.Filters;
using Owin;
using System.Web.Http;

namespace Hangfire.Demo.Client
{
    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Hangfire
            GlobalConfiguration.Configuration.UseSqlServerStorage("HangfireDemo");
            appBuilder.UseHangfireDashboard("/hangfire", new DashboardOptions()
            {
                Authorization = new[] { new HangfireDashboardAuthorizeFilter() }
            });

            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            appBuilder.Use<LoggingMiddleware>();
            appBuilder.UseWebApi(config);
        }
    }
}
