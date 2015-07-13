using System;
using System.Net.Http;
using Microsoft.Owin.Hosting;

namespace Nautabus.Server.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            //sets the data directory to the same folder where the exe lives - needed to attach user instance database files
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);

            

            string baseAddress = "http://localhost:9000/";

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.WriteLine("Server running on {0}", baseAddress);
                // Create HttpCient and make a request to api/values 
                HttpClient client = new HttpClient();

                var response = client.GetAsync(baseAddress + "home").Result;

                Console.WriteLine(response);
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                Console.ReadLine();

            }
           
        }
    }
}
