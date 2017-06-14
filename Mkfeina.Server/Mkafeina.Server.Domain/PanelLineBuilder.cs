using Microsoft.Practices.Unity;
using Mkafeina.Domain;
using Mkafeina.Domain.Dashboard.Panels;
using Mkafeina.Server.Domain.CoffeeMachineProxy;
using Mkafeina.Server.Domain.Entities;
using System;
using System.Linq;

namespace Mkafeina.Server.Domain
{
	public class PanelLineBuilder : AbstractPanelLineBuilder
	{
		public override string BuildOrUpdate(string lineName, object caller = null)
		{
			switch (lineName)
			{
				#region Server Panel Lines

				case AppConfig.SERVER_ADDRESS:
					var address = caller == null ? AppDomain.CurrentDomain.UnityContainer().Resolve<AppConfig>().ServerAddress :
												   ((AppConfig)caller).ServerAddress;
					return $"Server address : {address}";

				case AppConfig.SERVER_NICE_ADDRESS:
					var niceAddress = caller == null ? AppDomain.CurrentDomain.UnityContainer().Resolve<AppConfig>().ServerAddress :
												   ((AppConfig)caller).ServerAddress;
					return $"Server nice address : {niceAddress}";

				case CMProxyHub.SELECTED:
					return caller == null ? "Selected coffee machine: ----" : $"Selected coffee machine: {((CMProxyHub)caller).SelectedCM}";

				#endregion Status Panel Lines

				#region Commands Panel Lines

				case CommandInterpreter.COMMAND_F5:
					return "F5 : reload app.configs";

				case CommandInterpreter.COMMAND_F4:
					return "F4 : reload recipes in the cookbooks";

				case CMProxyHub.DISABLE_SELECTED:
					return "D : disable selected coffee machine";

				case CMProxyHub.NEXT:
					return "RIGHT ARROW : next coffee machine";

				case CMProxyHub.PREVIOUS:
					return "LEFT ARROW : previous coffee machine";

				#endregion Commands Panel Lines

				#region Coffee Machines Panel

				case CMProxy.CMPROXY_STATE:
					return $"Current state: {((CMProxy) caller).CurrentState.GetType().Name}";

				case CMProxy.CMPROXY_RECIPES:
					return caller == null ? "Recipes: -" : $"Recipes: {string.Join(",", ((CMProxy)caller).Info.AllRecipesNames)}";

				case CMProxyInfo.ENABLED:
					return caller == null ? "---" : (((CMProxy)caller).Info.Enabled ? "Enabled" : "Disabled");

				case CMProxyInfo.MAKING_COFFEE:
					return caller == null ? "---" : (((CMProxy)caller).Info.MakingCoffee ? "Making coffee" : "ZzZzZzZ");

				#endregion Coffee Machines Panel

				default:
					if (Ingredient.GetAllExistingIngredientsNames().Contains(lineName) )
						return CMIngredientLine(lineName, caller);
					else
						return $"line <<{lineName}>> was not implemented!";
			}
		}

		private string CMIngredientLine(string lineName, object caller)
		{
			var value = ((CMProxy)caller)?.Info.GetLevel(lineName);
			return value == null ? $"{lineName} : -" : $"{lineName}: {value}%";
		}
	}
}