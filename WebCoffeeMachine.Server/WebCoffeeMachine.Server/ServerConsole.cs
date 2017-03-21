using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WebCoffeeMachine.Server
{
    public static class ServerConsole
    {
        [DllImport("user32.dll")]
        public static extern bool SetWindow(IntPtr windowHandle, int showCommand);

        public static void StartConsole()
        {
            Console.Title = AppConfig.ConsoleTitle;
            var p = Process.GetCurrentProcess();
            SetWindow(p.MainWindowHandle, 3); //SW_MAXIMIZE = 3

            String strHostName = string.Empty;
            strHostName = Dns.GetHostName();
            Console.WriteLine("Local Machine's Host Name: " + strHostName);

            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] ips = ipEntry.AddressList;

#if true
            Console.WriteLine("Select the IP");
            foreach (var i in ips)
                Console.WriteLine(i);
            var chosen = Console.ReadKey().KeyChar.ToString();
            var ipIndex = int.Parse(chosen);
            var ip = ips[ipIndex];
#else
            var ip = ips[1];
#endif

            Console.WriteLine();
            Console.WriteLine("IP: " + ip);
        }

        private static string serverPanel = "";

        private static Dictionary<string, string> coffeeMachinePanels = new Dictionary<string, string>();

        private static List<string> logPanel = new List<string>();

        public static void UpdateCoffeMachinePanel(string coffeeMachineId, string newPanel)
        {
        }

        public static void Log(string message)
        {
        }
    }
}