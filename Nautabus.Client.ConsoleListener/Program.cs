using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Nautabus.Client.ConsoleListener
{
    class Program
    {
        static string subscriberId;
        static string topicA = "typed-messages";
        static string topicB = "string-messages";
        static void Main(string[] args)
        {
            //Directly talking to server via Nautaclient library
            subscriberId = args.Any() ? args[0] : promptForSubscriptionName();
            Console.WriteLine("Subscribing as "+ subscriberId);
            Console.WriteLine("sending test message");

            //uncomment to test conneting directly with nautaclient api
            //DirectTestNautaclient();

            //uncomment to test connecting via an IServiceBus instance instead
            CommonInterfaceTest();
            Console.WriteLine();
            Console.WriteLine("listening, press any key to exit...");
            Console.ReadLine();
        }

        static string promptForSubscriptionName()
        {
            Console.WriteLine("Please enter a subscription name for this listener");
            return Console.ReadLine();
        }

        static void CommonInterfaceTest()
        {
            var testMessage = new SampleMessage { Id = new Random().Next(0, 1000), MessageName = "test", MessageContent = "message from client Interface based listener " + subscriberId};

            var nbus = new NautabusServiceBus();
            nbus.SubscribeAsync(topicA, subscriberId, GetTypedMessageCallbackAction()).Wait();
            //this interface requires the object be ISerializable, and ironically enough, System.String isn't ISerializable
            //nbus.SubscribeAsync(topicB, subscriberId, GetSimpleStringCallbackAction()).Wait();
            nbus.PublishAsync(topicA, testMessage).Wait();
        }

        static void DirectTestNautaclient()
        {
           
            var testMessage = new SampleMessage { Id = new Random().Next(0,1000), MessageName = "test", MessageContent = "message from client listener " + subscriberId };
            var client = new Nautaclient("Nautaconnection");
            client.ConnectAsync().Wait();
            client.SubscribeAsync(topicA, subscriberId, GetTypedMessageCallbackAction()).Wait();
            client.SubscribeAsync(topicB, subscriberId, GetSimpleStringCallbackAction()).Wait();

            //send one message to the topic
            client.PublishAsync(topicA, testMessage).Wait();
            
        }

        private static Action<dynamic> GetSimpleStringCallbackAction()
        {
            return s =>
            {
                Console.WriteLine("message received for event '"+ topicB +"'");
                Console.WriteLine("message = " + s.data);//NOTE! When using dynamic, camelCase is preserved
                Console.WriteLine("---------------------------");
            };
        }

        private static Action<SampleMessage> GetTypedMessageCallbackAction()
        {
            return smsg =>
            {
                Console.WriteLine("message received for event '"+ topicA +"'");
                Console.WriteLine("message id = " + smsg.Id);
                Console.WriteLine("message name = " + smsg.MessageName);
                Console.WriteLine("message content = " + smsg.MessageContent);
                Console.WriteLine("---------------------------");
            };
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
}
