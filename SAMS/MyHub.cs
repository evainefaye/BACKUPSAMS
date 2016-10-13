using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using System.Web;


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
    }
}