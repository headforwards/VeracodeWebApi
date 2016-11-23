using System.Web.Http;
using System.Web.Http.Cors;

namespace Headforwards.Axa.Ppp.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            // veracode latest scan results route
            config.Routes.MapHttpRoute(
                name: "LatestVeracodeApi",
                routeTemplate: "api/{applicationId}/scans/latest",
                defaults: new { controller = "LatestVeracodeScan" }
            );

            // veracode all and specific scan route
            config.Routes.MapHttpRoute(
                name: "VeracodeApi",
                routeTemplate: "api/{applicationId}/scans/{scanId}",
                defaults: new { controller = "VeracodeScans", scanId = RouteParameter.Optional }
            );

            // default route
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // enable CORS for entire api
            var cors = new EnableCorsAttribute("*", "accept, content-type, origin, Authorization", "GET,POST");
            cors.SupportsCredentials = true;
            config.EnableCors(cors);
        }
    }
}
