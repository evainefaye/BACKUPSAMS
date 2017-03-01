using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;


namespace SAMS
{
    public class SignalRDB
    {

        // Initialize Database Status on Startup of Application
        public static void InitializeDatabase()
        {
            using (SignalR db = new SignalR())
            {
/*
                // Remove any records in sashaSessions
                sashaSession sashaSession = new sashaSession();
                var sashaSessionRecords = db.sashaSessions;
                db.sashaSessions.RemoveRange(sashaSessionRecords);
                db.SaveChanges();
*/
                // Update any open chat Sessions to show Auto Closed
                chatSession chatSession = new chatSession();
                foreach (var chatSessionRecord in db.chatSessions.Where(c => c.completeDate == "").ToList())
                {
                    chatSessionRecord.completeDate = "Auto Closed";
                }
                db.SaveChanges();

                // Update lookupMonitorUsers
                foreach (var lookupMonitorUserRecord in db.lookupMonitorUsers.ToList())
                {
                    lookupMonitorUserRecord.onlineStatus = "offline";
                    lookupMonitorUserRecord.connectionId = "";
                    lookupMonitorUserRecord.currentChats = 0;
                    lookupMonitorUserRecord.lastChatTime = System.DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ssZ");
                }
                db.SaveChanges();
            }
        }


        // Removes Given Connection from the sashaSessions Table
        public static bool RemoveSashaSessionRecord(string connectionId)
        {
            using (SignalR db = new SignalR())
            {
                sashaSession sashaSession = new sashaSession();
                var sashaSessionRecord = db.sashaSessions.Where(s => s.connectionId == connectionId).SingleOrDefault();
                if (sashaSessionRecord != null)
                {
                    db.sashaSessions.Remove(sashaSessionRecord);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }


        // Check lookupMonitorUser on disconnect and update fields if needed
        public static void UpdateChatHelper(string connectionId)
        {
            using (SignalR db = new SignalR())
            {
                lookupMonitorUser lookupMonitorUser = new lookupMonitorUser();
                var lookupMonitorUserRecord = db.lookupMonitorUsers.Where(c => c.connectionId == connectionId).SingleOrDefault();
                if (lookupMonitorUserRecord != null)
                {
                    lookupMonitorUserRecord.connectionId = "";
                    lookupMonitorUserRecord.currentChats = 0;
                    lookupMonitorUserRecord.onlineStatus = "Offline";
                }
                db.Entry(lookupMonitorUserRecord).CurrentValues.SetValues(lookupMonitorUserRecord);
                db.SaveChanges();
                chatSession chatSession = new chatSession();
                foreach (var chatSessionRecord in db.chatSessions.Where(c => c.helperConnectionId == connectionId && c.completeDate == "").ToList())
                {
                    chatSessionRecord.completeDate = System.DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ssZ");
                }
                db.SaveChanges();
            }
        }


        // Check monitorUser for a valid login and current connection
        public static void lookupMonitorUsers(string userId, string connectionId)
        {
            using (SignalR db = new SignalR())
            {
                lookupMonitorUser monitorUser = new lookupMonitorUser();
                var monitorUserRecord = db.lookupMonitorUsers.Where(c => c.userId == userId).SingleOrDefault();
                dynamic results = new System.Dynamic.ExpandoObject();
                if (monitorUserRecord != null)
                {
                    if (monitorUserRecord.connectionId != "" && monitorUserRecord.connectionId != connectionId)
                    {
                        results.resultType = "error";
                        results.resultMessage = "LIMIT OF ONE CONNECTION PER LOGIN: " + userId;
                        results.resultDuration = 8000;
                    }
                    else
                    {
                        results.resultType = "success";
                        results.resultMessage = "";
                        results.maximumChats = monitorUserRecord.maximumChats;
                        results.connectionId = connectionId;
                        int maximumChats = monitorUserRecord.maximumChats;
                        monitorUserRecord.connectionId = connectionId;
                        db.Entry(monitorUserRecord).CurrentValues.SetValues(monitorUserRecord);
                        db.SaveChanges();
                        db.Configuration.LazyLoadingEnabled = false;
                        var lookupMonitorUsersRecord = db.lookupMonitorUsers.Where(c => c.userId == userId).SingleOrDefault();
                        results.monitorUser = lookupMonitorUsersRecord;
                        var userRecord = db.users.Where(c => c.userId == userId).SingleOrDefault();
                        results.user = userRecord;
                    }
                }
                else
                {
                    results.resultType = "error";
                    results.resultMessage = "NO ACCESS FOR THIS LOGIN: " + userId;
                    results.resultDuration = 8000;
                }
                var myHub = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
                var json = JsonConvert.SerializeObject(results);
                myHub.Clients.Client(connectionId).monitorConnectionResults(json);

                var sashaSessions = from c in db.sashaSessions join u in db.users on c.userId equals u.userId select new { connectionId = c.connectionId, userId = c.userId, userName = u.userName, sessionStartTime = c.sessionStartTime, flowName = c.flowName, stepName = c.stepName, stepNameStartTime = c.stepNameStartTime };
                json = JsonConvert.SerializeObject(sashaSessions);
                myHub.Clients.Client(connectionId).getSessions(json);
            }
        }

    }
}