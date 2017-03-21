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
            Console.SetWindowSize(150, 30);
            Console.SetWindowPosition(0, 0);

            String strHostName = string.Empty;
            strHostName = Dns.GetHostName();
            Console.WriteLine("Local Machine's Host Name: " + strHostName);

            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] ips = ipEntry.AddressList;

#if true
            Console.WriteLine("Select the IP");
            foreach(var i in ips)
                Console.WriteLine(i);
            var chosen = Console.ReadKey().KeyChar.ToString();
            var ipIndex = int.Parse(chosen);
            var ip = ips[ipIndex];
#else
            var ip = ips[1];
#endif

            Console.WriteLine();
            Console.WriteLine("IP: " + ip);

            string baseAddress1 = "http://localhost:8081";
            string baseAddress3 = $"http://{ip}:8081";
            string baseAddress4 = $"http://{Environment.MachineName}:8081";

            StartOptions options = new StartOptions();
            options.Urls.Add(baseAddress1);
            options.Urls.Add(baseAddress3);
            options.Urls.Add(baseAddress4);
            while (true) {
                using (WebApp.Start<Startup>(options)) {
                    TryClient(baseAddress1);
                    TryClient(baseAddress3);
                    TryClient(baseAddress4);
                    var line = Console.ReadLine();
                } 
            }
        }

        private static void TryClient(string baseAddress)
        {
            try {
                Console.WriteLine($"------------------------------- TRY CLIENT {baseAddress} -----------------------------------------");
                HttpClient client = new HttpClient();
                var response = client.GetAsync(baseAddress + "/testapi").Result;
                Console.WriteLine(response);
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                Console.WriteLine($"------------------------------- OK END {baseAddress} -----------------------------------------");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            } catch (Exception ex) {
                Console.WriteLine(ex);
                Console.WriteLine($"------------------------------- EXCEPTION END {baseAddress} -----------------------------------------");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }
        }
    }
}