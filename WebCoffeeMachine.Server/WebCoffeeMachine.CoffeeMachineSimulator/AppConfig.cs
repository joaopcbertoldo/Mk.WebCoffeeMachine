using System.Configuration;

namespace WebCoffeeMachine.CoffeeMachineSimulator
{
    public class AppConfig
    {
        public static string SimulatorTitle { get => ConfigurationManager.AppSettings["simulatorTitle"]; }

        public static string SimulatorIp { get => ConfigurationManager.AppSettings["simulatorIp"]; }

        public static int SimulatorPort { get => int.Parse(ConfigurationManager.AppSettings["simulatorPort"]); }

        public static string LogPanelTitle { get => ConfigurationManager.AppSettings["logPanelTitle"]; }

        public static int MaxLogMessages { get => int.Parse(ConfigurationManager.AppSettings["maxLogMessages"]); }

        public static int Increment { get => int.Parse(ConfigurationManager.AppSettings["increment"]); }

        public static int IngredientAdditionBaseDelayMs { get => int.Parse(ConfigurationManager.AppSettings["ingredientAdditionBaseDelayMs"]); }

    }
}