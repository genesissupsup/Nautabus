using System.Web.Http;
using Microsoft.Owin.Cors;
using Owin;

namespace Nautabus.Server
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
           

            var httpConfiguration = new HttpConfiguration();
            WebApiConfig.Register(httpConfiguration);
            app.UseWebApi(httpConfiguration);
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }
}