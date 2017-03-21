using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCoffeeMachine.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerConsole.StartConsole();

            

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
}
