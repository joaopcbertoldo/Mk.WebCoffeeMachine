using Microsoft.Practices.Unity;
using Mkafeina.Domain;
using Mkafeina.Domain.Dashboard.Panels;
using Mkafeina.Server.Domain.CoffeeMachineProxy;
using Mkafeina.Server.Domain.Entities;
using System;

namespace Mkafeina.Server.Domain
{
	public class PanelLineBuilder : AbstractPanelLineBuilder
	{
		public override string BuildOrUpdate(string lineName, object caller = null)
		{
			switch (lineName)
			{
				#region Status Panel Lines

				case AppConfig.SERVER_ADDRESS:
					var address = caller == null ? AppDomain.CurrentDomain.UnityContainer().Resolve<AppConfig>().ServerAddress :
												   ((AppConfig)caller).ServerAddress;
					return $"Server address : {address}";

				case AppConfig.SERVER_NICE_ADDRESS:
					var niceAddress = caller == null ? AppDomain.CurrentDomain.UnityContainer().Resolve<AppConfig>().ServerAddress :
												   ((AppConfig)caller).ServerAddress;
					return $"Server nice address : {niceAddress}";

				#endregion Status Panel Lines

				#region Commands Panel Lines

				case CommandInterpreter.COMMAND_F5:
					return "F5 : reload app.configs, dashboard";

				case CommandInterpreter.COMMAND_F4:
					return "F4 : reload recipes in the cookbook";

				#endregion Commands Panel Lines

				#region Coffee Machines Panel

				case "states":
					return $"Current state: { "not implemented"}";

				case CookBook.RECIPES:
					return caller == null ? "Recipes: -" : $"Recipes: {string.Join(",", ((CMProxyInfo)caller).AllRecipesNames)}";

				case CMProxyInfo.ENABLED:
					return caller == null ? "---" : (((CMProxyInfo)caller).Enabled ? "Enabled" : "Disabled");

				case CMProxyInfo.MAKING_COFFEE:
					return caller == null ? "---" : (((CMProxyInfo)caller).MakingCoffee ? "Making coffee" : "ZzZzZzZ");

				#endregion Coffee Machines Panel

				default:
#warning mudar aqui para pegar o caso de não ser ingrediente
					return CMIngredientLine(lineName, caller);// ?? $"line <<{lineName}>> was not implemented!";
			}
		}

		private string CMIngredientLine(string lineName, object caller)
		{
			var value = ((CMProxyInfo)caller)?.GetLevel(lineName);
			return value == null ? $"{lineName} : -" : $"{lineName}: {value}%";
		}
	}
}