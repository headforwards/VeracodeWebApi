using System.Linq;
using System.Web;
using System.Web.Http;

namespace Headforwards.Axa.Ppp.WebApi
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
