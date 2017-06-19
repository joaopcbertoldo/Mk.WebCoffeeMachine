using Microsoft.Practices.Unity;
using Mkafeina.Domain;
using Mkafeina.Domain.Dashboard;
using Mkafeina.Server.Domain.CoffeeMachineProxy;
using Mkafeina.Server.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Mkafeina.Server.Domain
{
	public class CommandInterpreter : AbstractCommandInterpreter
	{
		public const string
			COMMAND_F5 = "F4",
			COMMAND_F4 = "F5"
			;

		public override void HandleCommand(ConsoleKeyInfo key)
		{
			Task.Factory.StartNew(() =>
			{
				switch (key.Key)
				{
					case ConsoleKey.F5:
						{
							var appconfig = AppDomain.CurrentDomain.UnityContainer().Resolve<AppConfig>();
							appconfig.ReloadConfigs();
						}
						Dashboard.Sgt.LogAsync("App.config has been reloaded.");
						break;

					case ConsoleKey.F4:
						{
							var mainCookBook = AppDomain.CurrentDomain.UnityContainer().Resolve<MainCookBook>();
							mainCookBook.ReloadRecipesFromAppConfig(wait: true);
							CMProxyHub.Sgt.ReloadRecipesOnProxies();
						}
						Dashboard.Sgt.LogAsync("Recipes have been reloaded.");
						break;

					case ConsoleKey.RightArrow:
						CMProxyHub.Sgt.NextCM();
						break;

					case ConsoleKey.LeftArrow:
						CMProxyHub.Sgt.PreviousCM();
						break;

					case ConsoleKey.D:
						CMProxyHub.Sgt.DisableSelectedCM();
						break;


					default:
						return;
				}
			});
		}
	}
}