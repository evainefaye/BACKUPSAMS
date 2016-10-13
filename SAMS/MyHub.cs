using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using System.Web;
using System;
using System.Data;
using System.Linq;
using Newtonsoft.Json;

namespace SAMS
{
    public static class groupNames
    {
        public const string Monitor = "Monitor";
        public const string Sasha = "Sasha";
    }

    public class MyHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }

        /* When a client disconnects attempts to remove its record from the SashaSessions Database and calls for an update of sasha sessions on all monitor clients */
        public override Task OnConnected()
        {
            string connectionId = Context.ConnectionId;
            using (SignalR db = new SignalR())
            {
                user user = new user();
                user.userId = connectionId;
                user.userName = connectionId;
                db.users.Add(user);
                db.SaveChanges();
            }
            return base.OnConnected();
        }

    }
}