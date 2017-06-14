using Microsoft.Practices.Unity;
using Mkafeina.CoffeeMachineSimulator;
using Mkafeina.Domain;
using Mkafeina.Domain.Dashboard.Panels;
using Mkafeina.Server.Domain.CoffeeMachineProxy;
using Mkafeina.Server.Domain.Entities;
using System;

namespace Mkafeina.Simulator
{
	public class PanelLineBuilder : AbstractPanelLineBuilder
	{
		private AppConfig _appconfig;

		public override string BuildOrUpdate(string lineName, object caller = null)
		{
			// pequena gambiarra
			if (_appconfig == null)
				_appconfig = (AppConfig)AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractAppConfig>();

			switch (lineName)
			{
				case AppConfig.SIMULATOR_ADDRESS:
					return $"Simulator address: {_appconfig.SimulatorAddress}";

				case AppConfig.SERVER_API_URL:
					return $"Server API URL: {_appconfig.ServerApiUrl}";

				case AppConfig.SIMULATOR_UNIQUE_NAME:
					return $"Unique name : {_appconfig.SimulatorUniqueName}";

				case CMSignals.REGISTERED:
					return FakeCoffeMachine.Sgt.Signals.Registered ? "Registered" : "NOT registered";

				case CMSignals.ENABLED:
					return FakeCoffeMachine.Sgt.Signals.Enabled ? "Enabled" : "Disabled";

				case CMSignals.MAKING_COFFEE:
					return FakeCoffeMachine.Sgt.Signals.MakingCoffee ? "WORKING NOW" : "ZzZzZzZzZzZz";

				case CMSignals.COFFEE:
					return $"Coffee : {FakeCoffeMachine.Sgt.Signals.Coffee:0.0}V";

				case CMSignals.SUGAR:
					return $"Sugar : {FakeCoffeMachine.Sgt.Signals.Sugar:0.0}V";

				case CMSignals.WATER:
					return $"Water : {FakeCoffeMachine.Sgt.Signals.Water:0.0}V";

				case CMSignals.MIN_COFFEE:
					return $"Minimum coffee : {FakeCoffeMachine.Sgt.Signals.CoffeeMin:0.0}V";

				case CMSignals.MAX_COFFEE:
					return $"Maximum coffee : {FakeCoffeMachine.Sgt.Signals.CoffeeMax:0.0}V";

				case CMSignals.MIN_SUGAR:
					return $"Minimum sugar : {FakeCoffeMachine.Sgt.Signals.SugarMin:0.0}V";

				case CMSignals.MAX_SUGAR:
					return $"Maximum sugar : {FakeCoffeMachine.Sgt.Signals.SugarMax:0.0}V";

				case CMSignals.MIN_WATER:
					return $"Minimum water : {FakeCoffeMachine.Sgt.Signals.WaterMin:0.0}V";

				case CMSignals.MAX_WATER:
					return $"Maximum water : {FakeCoffeMachine.Sgt.Signals.WaterMax:0.0}V";

				case CMProxy.CMPROXY_RECIPES:
					return $"Recipes: {((CMProxy)caller).Info.AllRecipesNames}";

				case IngredientManipulator.SELECTED:
					return $"Selected ingredient : {IngredientManipulator.Sgt.SelectedIngredient}";

				#region Configs Panel Lines

				case AppConfig.INGREDIENT_ADDITION_DELAY_MS:
					return $"Ingredient addition delay : {_appconfig.IngredientAdditionDelayMs}ms";

				case AppConfig.STANDARD_TIMEOUT:
					return $"Standard timeout : {_appconfig.StandardTimeout:0,000}ms";

				#endregion Configs Panel Lines

				#region Commands Panel Lines

				case CommandInterpreter.LEFT_ARROW:
					return "LEFT : previous ingredient";

				case CommandInterpreter.RIGHT_ARROW:
					return "RIGHT : next ingredient";

				case CommandInterpreter.UP_ARROW:
					return "UP : ingredient++";

				case CommandInterpreter.DOWN_ARROW:
					return "DOWN : ingredient--";

				case CommandInterpreter.REENABLE:
					return "R : reenable";

				case CommandInterpreter.DISABLE:
					return "D : disable";

				case CommandInterpreter.ONOFF:
					return "ON/OFF";

				case CommandInterpreter.OFFSETS:
					return "O : send offsets";

				#endregion Commands Panel Lines

				default:
					return "NOT IMPLEMENTED";
			}
		}
	}
}