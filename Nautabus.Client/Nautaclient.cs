﻿using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nautabus.Client
{
    public class Nautaclient
    {

        public string HostUrl { get;  }
        private static IHubProxy Nautaproxy { get; set; }
        public bool IsConnected { get; set; }

        private JsonSerializerSettings JsonSettings { get; } =
            new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        
        
       

        public Nautaclient(string nameOrConnectionString)
        {
            var cstring = ConfigurationManager.ConnectionStrings[nameOrConnectionString];
            if (cstring != null)
            {
                nameOrConnectionString = cstring.ConnectionString;
            }

            var parts = nameOrConnectionString
                .Split(';')
                .Select(p => p.Split('='))
                .Select(t => new { key = t[0], value = t[1] }).ToList();

            var host = parts.FirstOrDefault(p => p.key.Equals("hostUrl"));
            if (host == null)
            {
                throw new TypeInitializationException(this.GetType().FullName, new ArgumentNullException(nameof(nameOrConnectionString)));
            }

            HostUrl = host.value;

            //foreach (var nvp in parts)
            //{
            //    foreach (var pair in nvp)
            //    {
            //        var p = nvp.Split('=');
            //        pairs.Add(new KeyValuePair<string, string>(p[0], p[1]));
            //    }
            //}
            //if (pairs.Any(p => p.Key.Equals("hostUrl")))
            //{
            //    HostUrl = pairs.First(p => p.Key == "hostUrl").Value;
            //}
            //else
            //{
            //    throw new TypeInitializationException(this.GetType().FullName,
            //        new ArgumentNullException(nameof(nameOrConnectionString)));
            //}

        }

        public async Task ConnectAsync()
        {
            var hubConnection = new HubConnection(HostUrl);
            Nautaproxy = hubConnection.CreateHubProxy("Nautahub");
            await hubConnection.Start();
            IsConnected = true;
        }

        public async Task PublishAsync<T>(string topic, T message)
        {
            if (!IsConnected)
            {
                await ConnectAsync();
            }
            var jsonValue = JsonConvert.SerializeObject(message, JsonSettings);
            await Nautaproxy.Invoke("Publish", topic, jsonValue);
        }

      

        public async Task<IDisposable> SubscribeAsync<T>(string topic, string subscription, Action<T> callbackAction)
        {
            if (!IsConnected)
            {
                await ConnectAsync();
            }
            await Nautaproxy.Invoke("Subscribe", topic, subscription);
            return Nautaproxy.On<string>(topic, msg =>
            {
                //NOTE! When T is dynamic, camelCase will not be mapped to PascalCase
                var obj = JsonConvert.DeserializeObject<T>(msg, JsonSettings);
                callbackAction(obj);
            });


        }

    }
}