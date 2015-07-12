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
        static IHubProxy Nautaproxy { get; set; }

        static void Main(string[] args)
        {
            ConnectToHub().Wait();

            var testMessage = new SampleMessage {Id = 55, MessageName = "test", MessageContent = "this is a test message of Sample message type from client"};

            Nautaproxy.Invoke("Publish", "sample-messages", testMessage);


            Console.ReadLine();
        }

        public static async Task ConnectToHub()
        {
            var hubConnection = new HubConnection("http://localhost:3411/", new Dictionary<string, string> { { "clientName", "ConsoleListener" } });
            Nautaproxy = hubConnection.CreateHubProxy("Nautahub");

            
            Nautaproxy.On<SampleMessage>("sample-messages", smsg =>
            {
                Console.WriteLine("message received for event 'sample-messages'");
                Console.WriteLine("message id = " + smsg.Id);
                Console.WriteLine("message name = " + smsg.MessageName);
                Console.WriteLine("message content = " + smsg.MessageContent);
                Console.WriteLine("---------------------------");
            });

            Nautaproxy.On<string>("string-messages", s =>
            {
                Console.WriteLine("message received for event 'string-messages'");
                Console.WriteLine("message = " + s);
                Console.WriteLine("---------------------------");
            });
            await hubConnection.Start();


            await Nautaproxy.Invoke("subscribe", "sample-messages");
            
        }


    }


    public class SampleMessage
    {
        public int Id { get; set; }
        public string MessageName { get; set; }
        public string MessageContent { get; set; }
    }
}
