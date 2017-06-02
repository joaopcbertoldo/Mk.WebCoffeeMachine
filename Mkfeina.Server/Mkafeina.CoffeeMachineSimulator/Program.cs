using Microsoft.Practices.Unity;
using Mkafeina.Domain;
using Mkafeina.Domain.Dashboard;
using Mkafeina.Domain.Dashboard.Panels;
using System;
using static Mkafeina.Domain.Extentions;

namespace Mkafeina.Simulator
{
	public class Program
	{
		public static Action<ConsoleKeyInfo> KeyEvent;

		private static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnityContainer().RegisterType<AbstractCommandInterpreter, SimulatorCommandInterpreter>();
			AppDomain.CurrentDomain.UnityContainer().RegisterType<AbstractPanelLineBuilder, SimulatorPanelLineBuilder>();
			AppDomain.CurrentDomain.UnityContainer().RegisterInstance<AbstractAppConfig>(SimulatorAppConfig.Singleton);

			//SimulatorAppConfig.Singleton.ReloadConfigs();
			//CookBook.Sgt.LoadRecipes(wait: true);

			//SimulatorDashboard.Singleton.Title = SimulatorAppConfig.Singleton.SimulatorUniqueName;
			//SimulatorDashboard.Singleton.CreatePanels(SimulatorAppConfig.Singleton.PanelsConfigs);
			//SimulatorDashboard.Singleton.AddFixedLinesToPanels(SimulatorAppConfig.Singleton.PanelsLinesCollections);

			//FakeCoffeMachine.Singleton.StatusChangeEvent += SimulatorDashboard.Singleton.UpdateEventHandler("status");
			//CookBookExtension.SelectedRecipeChangeEvent += SimulatorDashboard.Singleton.UpdateEventHandler("status");
			//SimulatorAppConfig.Singleton.ConfigChangeEvent += SimulatorDashboard.Singleton.UpdateEventHandler("configs");

			//FakeCoffeMachine.Singleton.TurnOn();

			while (true) { }
		}
	}
}