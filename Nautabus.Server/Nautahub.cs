using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nautabus.Server
{
    public class Nautahub : Hub
    {
        public override Task OnConnected()
        {
            Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(5000);
                var hub = new EventHub();
                var testMessage = new SampleMessage { Id = 55, MessageName = "test", MessageContent = "this is a test message like a Sample message from the server" };

                hub.PublishMessage("sample-messages", testMessage);
                hub.PublishMessage("string-messages", "this is the other event from the server");

            }).ConfigureAwait(false);

            //assuming your client included a clientName querystring param
            //log message somewhere : Log("Connection from " + Context.QueryString["clientName"]));
            return base.OnConnected();
        }

        public void Subscribe(string topic)
        {
            Groups.Add(Context.ConnectionId, topic);
        }
        public void Unsubscribe(string topic)
        {
            Groups.Remove(Context.ConnectionId, topic);
        }

        public void Publish(string topic, SampleMessage message)
        {
            var eh = new EventHub();
            eh.PublishMessage(topic, message);
        }


    }

    public class SampleMessage
    {
        public int Id { get; set; }
        public string MessageName { get; set; }
        public string MessageContent { get; set; }
    }

    public class EventHub
    {
        public void PublishMessage<T>(string topic, T message)
        {
            var hub = GlobalHost.ConnectionManager.GetHubContext<Nautahub>();
            ((IClientProxy)hub.Clients.Group(topic)).Invoke(topic, message).Wait();
        }
    }


}