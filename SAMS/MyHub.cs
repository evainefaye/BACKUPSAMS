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

        //*** FUNCTIONS RUN ON MONITOR ***

        // Check if user is authenticated, prompt for login if not
        public void CheckAuthenticated()
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                string userId = Context.User.Identity.Name.GetUserId();
                string connectionId = Context.ConnectionId;
                Clients.Caller.userId = userId;
                Clients.Caller.connectionId = connectionId;
                SignalRDB.lookupMonitorUsers(userId, connectionId);
            }
            else
            {
                Clients.Caller.getUserId();
            }
        }

        public void AuthenticatedUser(string userId)
        {
            string connectionId = Context.ConnectionId;
            Clients.Caller.userId = userId;
            Clients.Caller.connectionId = connectionId;
            SignalRDB.lookupMonitorUsers(userId, connectionId);
        }



        // Remove client record from Database when disconnected
        public override Task OnDisconnected(bool stopCalled)
        {
            string connectionId = Context.ConnectionId;
            if (SignalRDB.RemoveSashaSessionRecord(connectionId))
            {
                Clients.Group(groupNames.Monitor).removeSashaSession(connectionId);
            }
            SignalRDB.UpdateChatHelper(connectionId);
            return base.OnDisconnected(stopCalled);
        }

    }
}