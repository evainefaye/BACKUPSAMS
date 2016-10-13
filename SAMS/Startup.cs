using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Cors;
using Microsoft.AspNet.SignalR;
using System.Data.Entity;


[assembly: OwinStartup(typeof(SAMS.Startup))]

namespace SAMS
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HubConfiguration hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableDetailedErrors = true;
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR(hubConfiguration);
            GlobalHost.Configuration.MaxIncomingWebSocketMessageSize = null;
            /* Set the following tables to an initialized state at startup

                sashaSessions (empty)
                chatSessions (empty)
                chatHelpers (connectionStatus to notConnected)
            Database.InitializeTables();
            */
        }
    }
}
