using System;
using System.IO;
using System.Net;
using System.Threading;

namespace HttpSenderTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var ipv4 = "192.168.0.101";
            Console.Title = "HTTP SENDER TEST";
            Console.SetWindowSize(150, 15);
            Thread.Sleep(3000);
            string baseAddress1 = "http://localhost:8081/testapi";
            string baseAddress3 = $"http://{ipv4}:8081/testapi";
            string baseAddress4 = $"http://{Environment.MachineName}:8081";
            SendRequest(baseAddress1);
            SendRequest(baseAddress3);
            SendRequest(baseAddress4);
            Console.ReadKey();
        }

        private static void SendRequest(string requestUriString)
        {
            try {
                Console.WriteLine($"------------------------------- START {requestUriString} -----------------------------------------");
                WebRequest request = WebRequest.Create(requestUriString);
                // Set the Method property of the request to GET.
                request.Method = "GET";
                // Get the response.
                WebResponse response = request.GetResponse();
                // Get the stream containing content returned by the server.
                var dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                // Display the content.
                Console.WriteLine($"RESPONSE FROM SERVER");
                Console.WriteLine(responseFromServer);
                // Clean up the streams.
                reader.Close();
                dataStream.Close();
                response.Close();
                Console.WriteLine($"------------------------------- OK END {requestUriString} -----------------------------------------");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            } catch (Exception ex) {
                Console.WriteLine(ex);
                Console.WriteLine($"------------------------------- EXCEPTION END {requestUriString} -----------------------------------------");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }
        }
    }
}