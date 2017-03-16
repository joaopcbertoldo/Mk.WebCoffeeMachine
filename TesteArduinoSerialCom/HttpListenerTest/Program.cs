using Microsoft.Owin.Hosting;
using System;
using System.Net;
using System.Net.Http;

namespace HttpListenerTest
{
    public class Program
    {
        private static void Main()
        {
            Console.Title = "HTTP LISTENER TEST";
            Console.SetWindowSize(150, 15);
            Console.SetWindowPosition(0, 0);

            String strHostName = string.Empty;
            // Getting Ip address of local machine...
            // First get the host name of local machine.
            strHostName = Dns.GetHostName();
            Console.WriteLine("Local Machine's Host Name: " + strHostName);
            // Then using host name, get the IP address list..
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;

            for (int i = 0; i < addr.Length; i++) {
                Console.WriteLine($"IP Address {i}: {addr[i].ToString()} ");
            }
            var ipv4 = addr[1];

            string baseAddress1 = "http://localhost:8081";
            //string baseAddress3 = $"http://192.168.0.101:8081";
            string baseAddress3 = $"http://{ipv4}:8081";
            string baseAddress4 = $"http://{Environment.MachineName}:8081";

            StartOptions options = new StartOptions();
            options.Urls.Add(baseAddress1);
            options.Urls.Add(baseAddress3);
            options.Urls.Add(baseAddress4);
            // Start OWIN host
            //WebApp.Start<Startup>(baseAddress1);
            //WebApp.Start<Startup>(baseAddress2);
            //WebApp.Start<Startup>(baseAddress3);
            //WebApp.Start<Startup>(baseAddress4);
            using (WebApp.Start<Startup>(options)) {
                TryClient(baseAddress1);
                TryClient(baseAddress3);
                TryClient(baseAddress4);
                var line = Console.ReadLine();
            }
        }

        private static void TryClient(string baseAddress)
        {
            try {
                HttpClient client = new HttpClient();
                var response = client.GetAsync(baseAddress + "/testapi").Result;
                Console.WriteLine(response);
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            } catch (Exception ex) {
                Console.WriteLine(ex);
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }
        }
    }
}