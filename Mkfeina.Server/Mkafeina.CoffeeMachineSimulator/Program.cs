using Microsoft.Practices.Unity;
using Mkfeina.CoffeeMachineSimulator;
using Mkfeina.Domain;
using Mkfeina.Domain.Panels;
using System;
using static Mkfeina.Domain.Extentions;

namespace Mkfeina.Simulator
{
	public class Program
	{
		public static Action<ConsoleKeyInfo> KeyEvent;

		private static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnityContainer().RegisterType<CommandInterpreter, SimulatorCommandInterpreter>();
			AppDomain.CurrentDomain.UnityContainer().RegisterType<PanelLineBuilder, SimulatorPanelLineBuilder>();
			AppDomain.CurrentDomain.UnityContainer().RegisterInstance<AppConfig>(SimulatorAppConfig.Singleton);

			SimulatorAppConfig.Singleton.ReloadConfigs();
			CookBook.Singleton.LoadRecipes(wait: true);

			SimulatorDashboard.Singleton.Title = SimulatorAppConfig.Singleton.SimulatorUniqueName;
			SimulatorDashboard.Singleton.CreatePanels(SimulatorAppConfig.Singleton.PanelsConfigs);
			SimulatorDashboard.Singleton.AddFixedLinesToPanels(SimulatorAppConfig.Singleton.PanelsLinesCollections);

			FakeCoffeMachine.Singleton.StatusChangeEvent += SimulatorDashboard.Singleton.UpdateEventHandler("status");
			CookBookExtension.SelectedRecipeChangeEvent += SimulatorDashboard.Singleton.UpdateEventHandler("status");
			SimulatorAppConfig.Singleton.ConfigChangeEvent += SimulatorDashboard.Singleton.UpdateEventHandler("configs");

			FakeCoffeMachine.Singleton.TurnOn();

			while (true) { }
		}
	}
}