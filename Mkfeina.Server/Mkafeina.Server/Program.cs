using Microsoft.Owin.Hosting;
using Microsoft.Practices.Unity;
using Mkafeina.Domain;
using Mkafeina.Domain.Dashboard;
using Mkafeina.Domain.Dashboard.Panels;
using Mkafeina.Server.Domain;
using Mkafeina.Server.Domain.CoffeeMachineProxy;
using Mkafeina.Server.Domain.Entities;
using System;
using System.Runtime.InteropServices;

namespace Mkafeina.Server
{
	internal class Program
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
			AppDomain.CurrentDomain.UnityContainer().RegisterInstance<AbstractDashboard>(Dashboard.Sgt);

			var appconfig = new AppConfig();
			AppDomain.CurrentDomain.UnityContainer().RegisterInstance<AbstractAppConfig>(appconfig);

			var mainCookbook = new MainCookBook();
			AppDomain.CurrentDomain.UnityContainer().RegisterInstance<MainCookBook>(mainCookbook);

			Dashboard.Sgt.Title = appconfig.ServerName;
			Dashboard.Sgt.CreateFixedPanels(appconfig.FixedPanelsConfigs);
			Dashboard.Sgt.AddFixedLinesToFixedPanels(appconfig.AllPanelsFixedLines);

			appconfig.ConfigChangeEvent += Dashboard.Sgt.UpdateEventHandlerOfPanel(AppConfig.CONFIGS);
			CMProxyHub.Sgt.ChangeEvent += Dashboard.Sgt.UpdateEventHandlerOfPanel("server");

			Dashboard.Sgt.LogAsync($"Server's dashboard is ready.");

			

			var serverAddress = appconfig.ServerAddress;
			StartOptions options = new StartOptions();
			options.Urls.Add(serverAddress);
			options.Urls.Add("localhost:80");

			using (WebApp.Start<Startup>(options))
			{

				Dashboard.Sgt.LogAsync($"Server's web api is on at <<{serverAddress}>>.");
				Dashboard.Sgt.LogAsync($"Server's nice address is <<{appconfig.ServerNiceAddress}>>.");
				while (true) { }
			}
		}
	}
}