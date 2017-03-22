using System.Configuration;

namespace WebCoffeeMachine.Server
{
    public static class AppConfig
    {
        public static string ConsoleTitle { get => ConfigurationManager.AppSettings["consoleTitle"]; }

        public static string ServerIp { get => ConfigurationManager.AppSettings["serverIp"]; }

        public static int ServerPort { get => int.Parse(ConfigurationManager.AppSettings["serverPort"]); }

        public static string LogPanelTitle { get => ConfigurationManager.AppSettings["logPanelTitle"]; }

        public static int MaxLogMessages { get => int.Parse(ConfigurationManager.AppSettings["maxLogMessages"]); }
    }
}