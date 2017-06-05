using Microsoft.Practices.Unity;
using Mkafeina.Domain;
using Mkafeina.Domain.Dashboard;
using Mkafeina.Server.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Mkafeina.Server.Domain
{
	public class CommandInterpreter : AbstractCommandInterpreter
	{
		public const string
			COMMAND_F5 = "F4",
			COMMAND_F4 = "F5";

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
						break;

					case ConsoleKey.F4:
						{
							var mainCookBook = AppDomain.CurrentDomain.UnityContainer().Resolve<CookBook>();
							var appconfig = AppDomain.CurrentDomain.UnityContainer().Resolve<AppConfig>();
#warning fazer reload nos cookbooks de cada proxy
							appconfig.LoadRecipesOnCookBookAsync(mainCookBook);
						}
						break;

					default:
						return;
				}
			});
		}
	}
}