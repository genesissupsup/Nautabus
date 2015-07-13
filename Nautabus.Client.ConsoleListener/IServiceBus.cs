using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Nautabus.Client.ConsoleListener
{
    interface IServiceBus
    {
        Task PublishAsync<T>(string topic, T message) where T : ISerializable;

        Task SubscribeAsync<T>(string topicName, string subscriptionName, Action<T> onMessageReceived,
            Action<Exception> onError = null) where T : ISerializable;
    }
}
