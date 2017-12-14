using Owin;
using System.Web.Http;

namespace HospitalSimulatorService
{
    public class Startup
    {
        /// <summary>
        /// WebAPI configuration using OWIN
        /// </summary>
        /// <param name="appBuilder"></param>
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure self-host WebAPI
            var config = new HttpConfiguration();
            config.EnableCors();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults:new {id = RouteParameter.Optional}
            );

            appBuilder.UseWebApi(config);
        }
    }
}
