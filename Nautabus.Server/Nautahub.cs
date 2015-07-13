using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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

        static string topicA = "typed-messages";
        static string topicB = "string-messages";
        public override Task OnConnected()
        {
            Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(5000);
                var hub = new EventHub();
                var testMessage = new SampleMessage { Id = 55, MessageName = "test", MessageContent = "this is a test message like a Sample message from the server" };

                hub.PublishMessage(topicA, testMessage);
                hub.PublishMessage(topicB, new {Data = "this is the other event from the server"});

            }).ConfigureAwait(false);

            //assuming your client included a clientName querystring param
            //log message somewhere : Log("Connection from " + Context.QueryString["clientName"]));
            return base.OnConnected();
        }

        public void Subscribe(string topic, string subscription)
        {
            Groups.Add(Context.ConnectionId, topic);
        }
        public void Unsubscribe(string topic, string subscription)
        {

            Groups.Remove(Context.ConnectionId, topic);
        }

        public void Publish(string topic, string message)
        {
            var eh = new EventHub();
            eh.PublishMessage(topic, message);
        }
    }


    public class SampleMessage : ISerializable
    {
        public SampleMessage() { }

        public SampleMessage(SerializationInfo info, StreamingContext context)
        {
            Id = info.GetInt32("Id");
            MessageName = info.GetString("MessageName");
            MessageContent = info.GetString("MessageContent");
        }
        public int Id { get; set; }
        public string MessageName { get; set; }
        public string MessageContent { get; set; }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("MessageName", MessageName);
            info.AddValue("MessageContent", MessageContent);
        }
    }

    public class EventHub
    {

        public void PublishMessage(string topic, string message)
        {
            var hub = GlobalHost.ConnectionManager.GetHubContext<Nautahub>();
            //Notice: now we're sending to a specified group, not all clients!
            ((IClientProxy)hub.Clients.Group(topic)).Invoke(topic, message).Wait();
        }

        public void PublishMessage<T>(string topic, T message)
        {
            var jsonValue = JsonConvert.SerializeObject(message, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            PublishMessage(topic, jsonValue);
        }
    }
}