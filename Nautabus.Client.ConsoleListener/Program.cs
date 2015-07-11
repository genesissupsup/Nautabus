using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace Nautabus.Client.ConsoleListener
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectToHub().Wait();
            Console.ReadLine();
        }

        public static async Task ConnectToHub()
        {
            var hubConnection = new HubConnection("http://localhost:9000/",new Dictionary<string, string> { {"clientName","ConsoleListener"}});
            IHubProxy myHubProxy = hubConnection.CreateHubProxy("MyHub");
            myHubProxy.On<string,string>("addMessage", (name,message )=> Console.WriteLine("{0} {1}", name, message));
            await hubConnection.Start();
        }


    }
}
