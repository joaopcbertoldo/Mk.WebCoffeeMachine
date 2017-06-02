using Microsoft.Owin.Hosting;
using Microsoft.Practices.Unity;
using Mkafeina.Domain;
using Mkafeina.Domain.Dashboard;
using Mkafeina.Domain.Dashboard.Panels;
using System;

namespace Mkafeina.Server
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnityContainer().RegisterType<AbstractCommandInterpreter, CommandInterpreter>();
			AppDomain.CurrentDomain.UnityContainer().RegisterType<AbstractPanelLineBuilder, PanelLineBuilder>();
			AppDomain.CurrentDomain.UnityContainer().RegisterInstance<AbstractAppConfig>(AppConfig.Sgt);
			AppDomain.CurrentDomain.UnityContainer().RegisterInstance<AbstractDashboard>(Dashboard.Sgt);

			AppConfig.Sgt.ReloadConfigs();
			//CookBook.Sgt.LoadRecipes(wait: true);

			Dashboard.Sgt.Title = AppConfig.Sgt.ServerName;
			Dashboard.Sgt.CreatePanels(AppConfig.Sgt.PanelsConfigs);
			Dashboard.Sgt.AddFixedLinesToPanels(AppConfig.Sgt.PanelsLinesCollections);

			AppConfig.Sgt.ConfigChangeEvent += Dashboard.Sgt.UpdateEventHandlerOfPanel("configs");

			var serverAddress = AppConfig.Sgt.ServerAddress;
			StartOptions options = new StartOptions();
			options.Urls.Add(serverAddress);

			using (WebApp.Start<Startup>(options))
			{
				Dashboard.Sgt.LogAsync($"Server's web api is on at <<{serverAddress}>>.");
				while (true) { }
			}
		}
	}
}