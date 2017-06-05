using Microsoft.Owin.Hosting;
using Microsoft.Practices.Unity;
using Mkafeina.Domain;
using Mkafeina.Domain.Dashboard;
using Mkafeina.Domain.Dashboard.Panels;
using Mkafeina.Server.Domain;
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

		public static Action<ConsoleKeyInfo> KeyEvent;

		private static void Main(string[] args)
		{
			ShowWindow(ThisConsole, 3); //  maximize window

			AppDomain.CurrentDomain.UnityContainer().RegisterType<AbstractCommandInterpreter, CommandInterpreter>();

			AppDomain.CurrentDomain.UnityContainer().RegisterType<AbstractPanelLineBuilder, PanelLineBuilder>();

			var appconfig = new AppConfig();
			appconfig.ReloadConfigs();
			AppDomain.CurrentDomain.UnityContainer().RegisterInstance<AbstractAppConfig>(appconfig);

			AppDomain.CurrentDomain.UnityContainer().RegisterInstance<AbstractDashboard>(Dashboard.Sgt);
			var mainCookbook = CookBook.CreateCookbook();
			appconfig.LoadRecipesOnCookBookAsync(mainCookbook, wait: true);
			AppDomain.CurrentDomain.UnityContainer().RegisterInstance<CookBook>(mainCookbook);

			Dashboard.Sgt.Title = appconfig.ServerName;
			Dashboard.Sgt.CreateFixedPanels(appconfig.FixedPanelsConfigs);
			Dashboard.Sgt.AddFixedLinesToFixedPanels(appconfig.AllPanelsFixedLines);

			appconfig.ConfigChangeEvent += Dashboard.Sgt.UpdateEventHandlerOfPanel(AppConfig.CONFIGS);

			var serverAddress = appconfig.ServerAddress;
			StartOptions options = new StartOptions();
			options.Urls.Add(serverAddress);

			using (WebApp.Start<Startup>(options))
			{
				Dashboard.Sgt.LogAsync($"Server's web api is on at <<{serverAddress}>>.");
				while (true)
				{
					var key = Console.ReadKey(intercept: true);
					KeyEvent?.Invoke(key);
				}
			}
		}
	}
}