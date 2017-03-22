namespace WebCoffeeMachine.Domain
{
    /*
    public class AppConsole
    {
        private static object _syncObj = new object();
        private static AppConsole _singleton;
        private AppConsole() { }
        public static AppConsole Singleton {
            get {
                if (_singleton == null) {
                    lock (_syncObj) {
                        if (_singleton == null)
                            _singleton = new AppConsole();
                    }
                }
            }
        }
        public void StartConsole(Simulator simulator)
        {
            Console.Title = AppConfig.ConsoleTitle;

            var simulatorPanel = new List<string>();
            simulatorPanel.Add(AppConfig.ConsoleTitle);
            simulatorPanel.Add($"IP: {AppConfig.SimulatorIp}");
            simulatorPanel.Add($"PORT: {AppConfig.SimulatorPort}");
            __panels.Add("simulator", simulatorPanel);

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
                WritePanel(__panels["log"]);
                Console.SetCursorPosition(0, 0);
            });
        }

        private static void WritePanel(List<string> panel)
        {
            Console.WriteLine(MakeDivisorLine(panel[0]));
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
            logPanel.Insert(1, ToLogMessage(message));
            if (logPanel.Count > AppConfig.MaxLogMessages)
                logPanel.RemoveAt(logPanel.Count - 1);
            RefreshServerConsole();
        }

        private static string MakeDivisorLine(string title)
            => new string('#', (Console.WindowWidth - title.Length) / 2 - 1) + $" {title} " + new string('#', (Console.WindowWidth - title.Length) / 2 - 1);

        private static int __logCounter = 0;

        private static string ToLogMessage(string message)
            => $"{++__logCounter} :: {DateTime.Now} :: {message}";
    }
*/
}