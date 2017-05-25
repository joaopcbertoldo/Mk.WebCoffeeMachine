using Microsoft.Owin.Hosting;
using Microsoft.Practices.Unity;
using Mkafeina.Domain;
using System;

namespace Mkafeina.Server
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnityContainer().RegisterType<Mkafeina.Domain.CommandInterpreter, CommandInterpreter>();
			AppDomain.CurrentDomain.UnityContainer().RegisterType<Mkafeina.Domain.Panels.PanelLineBuilder, PanelLineBuilder>();
			AppDomain.CurrentDomain.UnityContainer().RegisterInstance<Mkafeina.Domain.AppConfig>(AppConfig.Sgt);

			AppConfig.Sgt.ReloadConfigs();
			CookBook.Sgt.LoadRecipes(wait: true);

			Dashboard.Sgt.Title = AppConfig.Sgt.ServerName;
			Dashboard.Sgt.CreatePanels(AppConfig.Sgt.PanelsConfigs);
			Dashboard.Sgt.AddFixedLinesToPanels(AppConfig.Sgt.PanelsLinesCollections);

			AppConfig.Sgt.ConfigChangeEvent += Dashboard.Sgt.UpdateEventHandler("configs");

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