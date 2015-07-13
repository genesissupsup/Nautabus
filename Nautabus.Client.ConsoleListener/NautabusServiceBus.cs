using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Nautabus.Client.ConsoleListener
{
    public class NautabusServiceBus: IServiceBus
    {
        public Nautaclient Client { get; set; }

        public NautabusServiceBus()
        {
            Client = new Nautaclient("Nautaconnection");
        }

        public async Task PublishAsync<T>(string topic, T message) where T : ISerializable
        {
            await Client.PublishAsync(topic, message);
        }

        public Task SubscribeAsync<T>(string topicName, string subscriptionName, Action<T> onMessageReceived, Action<Exception> onError = null) where T : ISerializable
        {
            
            return Client.SubscribeAsync(topicName, subscriptionName, onMessageReceived);
        }
    }
}
