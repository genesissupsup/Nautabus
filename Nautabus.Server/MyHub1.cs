using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Nautabus.Server
{
    public class MyHub : Hub
    {
        public override Task OnConnected()
        {

            Clients.All.addMessage("new connection",Context.QueryString["clientName"]);
            return base.OnConnected();
        }

        public void Send(string name, string message)
        {
            Clients.All.addMessage(name, message);
        }
    }
}