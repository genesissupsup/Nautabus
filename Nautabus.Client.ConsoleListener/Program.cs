using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
            subscriberId = args.Any() ? args[0] : Guid.NewGuid().ToString();
            DirectTestNautaclient();
            IntermediaryTest();
            Console.ReadLine();
        }

        static void IntermediaryTest()
        {
            var testMessage = new SampleMessage { Id = 55, MessageName = "test", MessageContent = "this is a test message of Sample message type from client" };

            var nbus = new NautabusServiceBus();
            nbus.SubscribeAsync(topicA, subscriberId + "intermediary", GetTypedMessageCallbackAction()).Wait();
            //this interface requires the object be ISerializable, and ironically enough, System.String isn't ISerializable
            //nbus.SubscribeAsync(topicB, subscriberId, GetSimpleStringCallbackAction()).Wait();
            nbus.PublishAsync(topicA, testMessage).Wait();
        }

        static void DirectTestNautaclient()
        {
           
            var testMessage = new SampleMessage { Id = 55, MessageName = "test", MessageContent = "this is a test message of Sample message type from client" };
            var client = new Nautaclient("Nautaconnection");
            client.ConnectAsync().Wait();
            client.SubscribeAsync(topicA, subscriberId, GetTypedMessageCallbackAction()).Wait();
            client.SubscribeAsync(topicB, subscriberId, GetSimpleStringCallbackAction()).Wait();

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
