using System.Configuration;

namespace WebCoffeeMachine.Server
{
    public static class AppConfig
    {
        public static string ConsoleTitle { get => ConfigurationManager.AppSettings["consoleTitle"]; }
    }
}