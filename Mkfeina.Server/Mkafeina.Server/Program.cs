using Microsoft.Owin.Hosting;
using Mkfeina.Domain;
using System;

namespace Mkfeina.Server
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			CookBook.Singleton.LoadRecipes();
			//Thread.Sleep(8000);
			ServerConsole.StartConsole();

			var serverAddress = $"http://{AppConfig.ServerIp}:{AppConfig.ServerPort}";
			StartOptions options = new StartOptions();
			options.Urls.Add(serverAddress);

			using (WebApp.Start<Startup>(options))
			{
				ServerConsole.Log($"Server's web api is on at <<{AppConfig.ServerIp}:{AppConfig.ServerPort}>>.");
				while (true)
					Console.ReadKey(intercept: true);
			}
		}
	}
}