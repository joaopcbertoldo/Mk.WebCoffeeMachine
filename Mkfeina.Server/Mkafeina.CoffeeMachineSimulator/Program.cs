using Microsoft.Practices.Unity;
using Mkafeina.Domain;
using Mkafeina.Domain.Dashboard;
using Mkafeina.Domain.Dashboard.Panels;
using System;
using System.Runtime.InteropServices;
using static Mkafeina.Domain.Extentions;

namespace Mkafeina.Simulator
{
	public class Program
	{
		#region Maximize Window Stuff

		[DllImport("kernel32.dll", ExactSpelling = true)]
		private static extern IntPtr GetConsoleWindow();

		private static IntPtr ThisConsole = GetConsoleWindow();

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		#endregion Maximize Window Stuff

		private static void Main(string[] args)
		{
			ShowWindow(ThisConsole, 3); //  maximize window
			AppDomain.CurrentDomain.UnityContainer().RegisterType<AbstractCommandInterpreter, CommandInterpreter>();

			AppDomain.CurrentDomain.UnityContainer().RegisterType<AbstractPanelLineBuilder, PanelLineBuilder>();

			var appconfig = new AppConfig();
			appconfig.ReloadConfigs();
			AppDomain.CurrentDomain.UnityContainer().RegisterInstance<AbstractAppConfig>(appconfig);

			AppDomain.CurrentDomain.UnityContainer().RegisterInstance<AbstractDashboard>(Dashboard.Sgt);

			Dashboard.Sgt.Title = appconfig.SimulatorUniqueName;
			Dashboard.Sgt.CreateFixedPanels(appconfig.FixedPanelsConfigs);
			Dashboard.Sgt.AddFixedLinesToFixedPanels(appconfig.AllPanelsFixedLines);

			appconfig.ConfigChangeEvent += Dashboard.Sgt.UpdateEventHandlerOfPanel(AppConfig.CONFIGS);
			FakeCoffeMachine.Sgt.Signals.ChangeEvent += Dashboard.Sgt.UpdateEventHandlerOfPanel(AppConfig.FAKE_COFFEE_MACHINE);

			FakeCoffeMachine.Sgt.TurnOn();

			while (true) { }
		}
	}
}