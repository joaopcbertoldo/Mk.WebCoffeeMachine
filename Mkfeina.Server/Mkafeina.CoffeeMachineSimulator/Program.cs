using Microsoft.Owin.Hosting;
using System;
using System.Threading;

namespace Mkfeina.CoffeeMachineSimulator
{
    public class Program
    {
        public static Action<ConsoleKeyInfo> KeyEvent;

        private static void Main(string[] args)
        {

            AppConfig.LoadAppConfig();
            Dashboard.Load();

            var simulatorAddress = $"http://{AppConfig.SimulatorIp}:{AppConfig.SimulatorPort}";
            StartOptions options = new StartOptions();
            options.Urls.Add(simulatorAddress);

            using (WebApp.Start<Startup>(options)) {
                Dashboard.LogAsync($"Coffee machine simulator's api is up!");
                Dashboard.StartCommandInterpreter();
            }
        }
    }
}