using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCoffeeMachine.CoffeeMachineSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            SimulatorDashboard.TurnOn(Simulator.Singleton);

            var simulatorAddress = $"http://{AppConfig.SimulatorIp}:{AppConfig.SimulatorPort}";
            StartOptions options = new StartOptions();
            options.Urls.Add(simulatorAddress);

            using (WebApp.Start<Startup>(options)) {
                SimulatorDashboard.Log($"Coffee machine simulator's api is up at <<{AppConfig.SimulatorIp}:{AppConfig.SimulatorPort}>>.");
                while (true)
                    Simulator.Singleton.HandleCommand(Console.ReadKey(intercept: true));
            }
        }
    }
}
