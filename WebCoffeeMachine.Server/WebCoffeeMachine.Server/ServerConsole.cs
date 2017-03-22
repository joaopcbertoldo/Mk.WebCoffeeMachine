using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WebCoffeeMachine.Domain;
using WebCoffeeMachine.Server.Domain;

namespace WebCoffeeMachine.Server
{
    public class ServerConsole : Domain.IObserver<CoffeeMachineProxy>
    {
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        private static IntPtr ThisConsole = GetConsoleWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public ServerConsole Singleton { get => _singleton; }

        private static ServerConsole _singleton = new ServerConsole();

        private ServerConsole()
        {
        }

        public static void StartConsole()
        {
            Console.Title = AppConfig.ConsoleTitle;
            ShowWindow(ThisConsole, 3);

            var serverPanel = new List<string>();
            serverPanel.Add(AppConfig.ConsoleTitle);
            serverPanel.Add($"IP: {AppConfig.ServerIp}");
            serverPanel.Add($"PORT: {AppConfig.ServerPort}");
            __panels.Add("server", serverPanel);

            var logPanel = new List<string>();
            logPanel.Add(AppConfig.LogPanelTitle);
            __panels.Add("log", logPanel);

            RefreshServerConsole();
        }

        private static Dictionary<string, List<string>> __panels = new Dictionary<string, List<string>>();

        private static void RefreshServerConsole()
        {
            Task.Factory.StartNew(() => {
                Console.Clear();
                WritePanel(__panels["server"]);
                __panels.Where(p => p.Key != "server" && p.Key != "log")
                        .Select(p => p.Value)
                        .ToList()
                        .ForEach(p => WritePanel(p));
                WritePanel(__panels["log"]);
                Console.SetCursorPosition(0, 0);
            });
        }

        private static void WritePanel(List<string> panel)
        {
            Console.WriteLine(panel[0].ToDivisorLine());
            foreach (var line in panel.Skip(1)) {
                if (line.Length < Console.WindowWidth)
                    Console.SetCursorPosition((Console.WindowWidth - line.Length) / 2, Console.CursorTop);
                Console.WriteLine(line);
            }
            Console.WriteLine();
        }

        public static void Log(string message)
        {
            var logPanel = __panels["log"];
            logPanel.Insert(1, message.ToLogMessage(++__logCounter));
            if (logPanel.Count > AppConfig.MaxLogMessages)
                logPanel.RemoveAt(logPanel.Count - 1);
            RefreshServerConsole();
        }

        private static long __logCounter = 0;

        private static string ToLogMessage(string message)
            => $"{++__logCounter} :: {DateTime.Now} :: {message}";

        public void Notify(CoffeeMachineProxy notifier)
        {
            if (!__panels.ContainsKey(notifier.UniqueName))
                __panels.Add(notifier.UniqueName, null);
            __panels[notifier.UniqueName] = notifier.ToPanel();

            RefreshServerConsole();
        }
    }
}