using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCoffeeMachine.Domain;

namespace WebCoffeeMachine.CoffeeMachineSimulator
{
    public static class SimulatorDashboard
    {
        private static long __logCounter = 0;
        private static Simulator __simulator;
        private static List<string> __logPanel;
        private static List<string> __simulatorPanel;
        private static object __syncObj;

        public static void TurnOn(Simulator simulator)
        {
            __simulator = simulator;

            __syncObj = new object();

            Console.Title = AppConfig.SimulatorTitle;

            __simulatorPanel = __simulator.ToPanel();

            __logPanel = new List<string>();
            __logPanel.Add(AppConfig.LogPanelTitle);

            RefreshSimulatorStatus();
            Log("Dashboard started");
        }

        public static void RefreshSimulatorStatus()
        {
            Task.Factory.StartNew(() => {
                __simulatorPanel = __simulator.ToPanel();
                RefreshPanels();
            });
        }

        public static void Log(string message)
        {
            Task.Factory.StartNew(() => {
                __logPanel.Insert(1, message.ToLogMessage(++__logCounter));
                RefreshPanels();
            });

            Task.Factory.StartNew(() => {
                if (__logPanel.Count < AppConfig.MaxLogMessages + 15)
                    return;
                var count = __logPanel.Count;
                __logPanel.RemoveRange(count - 11, 10);
            });
        }

        private static void RefreshPanels()
        {
            lock (__syncObj) {
                Console.Clear();
                WritePanel(__simulatorPanel);
                WritePanel(__logPanel, AppConfig.MaxLogMessages);
                Console.SetCursorPosition(0, 0);
            }
        }

        private static void WritePanel(List<string> panel, int? numberOfLines = null)
        {
            Console.WriteLine(panel[0].ToDivisorLine());
            foreach (var line in panel.Skip(1).Take(numberOfLines != null ? numberOfLines.Value : panel.Count - 1))
                Console.WriteLine(line);
            Console.WriteLine();

            //panel.Skip(1).ToList().ForEach(line => {
            //    Console.Write(line);
            //    Console.Write(new string(' ', Console.BufferWidth > line.Length ? Console.BufferWidth - line.Length : 0));
            //    });

            //foreach (var line in panel.Skip(1)) {
            //    if (line.Length < Console.WindowWidth)
            //        Console.SetCursorPosition((Console.WindowWidth - line.Length) / 2, Console.CursorTop);
            //    Console.WriteLine(line);
            //}
            Console.WriteLine();
        }
    }
}