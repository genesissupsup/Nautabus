using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Nautabus.Domain;
using Nautabus.Domain.Model;
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
            //TODO: Can we make this async and use EF async methods?
            var channel = TopicSubscription.GetChannelName(topic, subscription);
            Groups.Add(Context.ConnectionId, channel);
            using (var ctx = new Nautacontext("Nautacontext"))
            {
                //add the topic
                //TODO: should we require topics be created explicitly instead of allowing listeners to arbitrarily create them by subscribing?
                if (!ctx.Topics.Any(t => t.Name == topic))
                {
                    ctx.Topics.Add(new Topic() {Name = topic});
                    ctx.SaveChanges();
                }

                //add the topic subscription if it doesn't exist
                if (!ctx.TopicSubscriptions.Any(s => s.TopicName == topic && s.SubscriptionName == subscription))
                {
                    ctx.TopicSubscriptions.Add(new TopicSubscription()
                    {
                        SubscriptionName = subscription,
                        TopicName = topic
                    });
                    ctx.SaveChanges();
                }
                else
                {
                    //TODO: this is a bit of a hack, need to move fetching already queued messages to another hub method, and modify client to make the second call as part of it's own subscribe method
                    Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(1000);//buying some time, so client can be ready to listen
                        var fedex = new DeliveryManager();
                        fedex.SendAll(topic, subscription);
                    }).ConfigureAwait(false);
                }
            }
        }
        public void Unsubscribe(string topic, string subscription)
        {
            Groups.Remove(Context.ConnectionId, TopicSubscription.GetChannelName(topic, subscription));
        }

        public void AcknowledgeMessage(int messageId, string subscription)
        {
            using (var ctx = new Nautacontext("Nautacontext"))
            {
                var msg = ctx.SubscriptionMessages.FirstOrDefault(m => m.MessageId == messageId && m.SubscriptionName == subscription);
                if (msg != null)
                {
                    ctx.SubscriptionMessages.Remove(msg);
                    ctx.SaveChanges();
                }
            }
        }

        public void Publish(string topic, string messageContent)
        {
            var now = DateTimeOffset.Now;
            using (var ctx = new Nautacontext("Nautacontext"))
            {
                var message = new Message() { TopicName = topic, MessageContent = messageContent, CreatedDate = now };
                ctx.Messages.Add(message);
                var subscriptions = ctx.TopicSubscriptions
                    .Where(s => s.TopicName.Equals(topic));

                var newSubMessages = new List<SubscriptionMessage>();
                foreach (var sub in subscriptions)
                {
                    newSubMessages.Add(new SubscriptionMessage()
                    {
                        Message = message,
                        TopicSubscription = sub
                    });
                }
                ctx.SubscriptionMessages.AddRange(newSubMessages);
                ctx.SaveChanges();

                var fedex = new DeliveryManager();
                foreach (var subscription in subscriptions)
                {
                    fedex.Send(subscription.ChannelName,subscription.TopicName, message.Id, message.MessageContent);
                }
            }
        }
    }

    public class DeliveryManager
    {

        public IHubContext HubContext => GlobalHost.ConnectionManager.GetHubContext<Nautahub>();

        public void Send(string channel, string topic, int messageId, string messageContent)
        {
            ((IClientProxy)HubContext.Clients.Group(channel)).Invoke(topic, messageId, messageContent).Wait();
        }

        public void SendAll(string topic, string subscription)
        {
            using (var ctx = new Nautacontext("Nautacontext"))
            {
                var subscriptionMessagesToDeliver = ctx.SubscriptionMessages
                    .Include(sm => sm.TopicSubscription)
                    .Include(sm => sm.Message)
                    .Where(sm => sm.TopicName == topic && sm.SubscriptionName == subscription)
                    .ToList();
                foreach (var sm in subscriptionMessagesToDeliver)
                {
                    Send(sm.TopicSubscription.ChannelName, sm.TopicName, sm.MessageId, sm.Message.MessageContent);
                }
            }
        }
    }
}