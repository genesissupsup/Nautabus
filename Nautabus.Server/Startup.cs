using System.Data.Entity;
using System.Web.Http;
using Microsoft.Owin.Cors;
using Nautabus.Domain;
using Nautabus.Domain.Migrations;
using Owin;

namespace Nautabus.Server
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            InitDatabase();
            var httpConfiguration = new HttpConfiguration();
            WebApiConfig.Register(httpConfiguration);
            app.UseWebApi(httpConfiguration);
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }

        public void InitDatabase()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<Nautacontext, Configuration>(true));
            using (var ctx = new Nautacontext("Nautacontext"))
            {
                ctx.Database.Initialize(false);
            }
        }
    }
}