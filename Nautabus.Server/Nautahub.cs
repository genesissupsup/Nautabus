using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nautabus.Server
{
    public class Nautahub : Hub
    {
      
        //public override Task OnConnected()
        //{
        //    //assuming your client included a clientName querystring param
        //    //log message somewhere : Log("Connection from " + Context.QueryString["clientName"]));
        //    return base.OnConnected();
        //}

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