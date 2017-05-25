using Mkafeina.CoffeeMachineSimulator;
using Mkafeina.Domain;
using Mkafeina.Domain.Panels;
using System;
using static Mkafeina.Simulator.Constants;

namespace Mkafeina.Simulator
{
	public class SimulatorPanelLineBuilder : PanelLineBuilder
	{
		public override string Build(string lineName)
		{
			switch (lineName)
			{
				#region Status Panel Lines

				case PANEL_LINE_IP:
					return $"IP: {SimulatorAppConfig.Singleton.SimulatorIp}";

				case PANEL_LINE_PORT:
					return $"PORT: {SimulatorAppConfig.Singleton.SimulatorPort}";

				case PANEL_LINE_UNIQUE_NAME:
					return $"Unique name : {SimulatorAppConfig.Singleton.SimulatorUniqueName}";
					
				case PANEL_LINE_REGISTRATION:
					return FakeCoffeMachine.Singleton.IsRegistered ? "Registration: OK" : "Registration: ---";

				case PANEL_LINE_IS_MAKING_COFFEE:
					return FakeCoffeMachine.Singleton.IsMakingCoffee ? "WORKING NOW" : "ZzZzZzZzZzZz";

				case PANEL_LINE_COFFEE_LEVEL:
					return $"Coffee : {FakeCoffeMachine.Singleton.CoffeeLevel:00}%";

				case PANEL_LINE_WATER_LEVEL:
					return $"Water : {FakeCoffeMachine.Singleton.WaterMl:0}ml";

				case PANEL_LINE_SELECTED_INGREDIENT:
					if (FakeCoffeMachine.Singleton.SelectedIngredient == INGREDIENTS_COFFEE)
						return $"Selected ingredient : coffee";
					else if (FakeCoffeMachine.Singleton.SelectedIngredient == INGREDIENTS_WATER)
						return $"Selected ingredient : water";
					else
						return $"Selected ingredient : ERROR";

				case PANEL_LINE_SELECTED_RECIPE:
					return $"Selected recipe : {CookBook.Sgt.SelectedRecipeName()}";

				#endregion Status Panel Lines

				#region Configs Panel Lines

				case PANEL_LINE_INGREDIENT_ADDITION_DELAY:
					return $"Ingredient addition delay : {SimulatorAppConfig.Singleton.IngredientAdditionDelayMs}ms";

				case PANEL_LINE_REGISTRATION_REQUEST_TIMEOUT:
					return $"Registration request timeout : {SimulatorAppConfig.Singleton.RegistrationTimeout}ms";

				case PANEL_LINE_WAIT_AFTER_REGISTRATION_WAIT_AFTER_FAILED_ATTEMPT:
					return $"Wait time after FAILED reg. : {SimulatorAppConfig.Singleton.RegistrationWaitAfter10FailedAtemptMs}ms";

				case PANEL_LINE_WAIT_AFTER_REGISTRATION_WAIT_AFTER_SUCCESSFUL_ATTEMPT:
					return $"Wait time after SUCCESSFUL reg. : {SimulatorAppConfig.Singleton.RegistrationWaitAfterSuccessfulAttempMs}ms";

				#endregion Configs Panel Lines

				#region Commands Panel Lines

				case PANEL_LINE_COMMAND_TAB:
					return "TAB : next recipe";

				case PANEL_LINE_COMMAND_SHIFT_TAB:
					return "SHIFT+TAB : previous recipe";

				case PANEL_LINE_COMMAND_ENTER:
					return "ENTER : make the selected recipe";

				case PANEL_LINE_COMMAND_LEFT_ARROW:
					return "LEFT : next ingredient";

				case PANEL_LINE_COMMAND_RIGHT_ARROW:
					return "RIGHT : previous ingredient";

				case PANEL_LINE_COMMAND_UP_ARROW:
					return "UP : ingredient++";

				case PANEL_LINE_COMMAND_DOWN_ARROW:
					return "DOWN : ingredient--";

				case PANEL_LINE_COMMAND_F5:
					return "F5 : reload configs and dashboard";

				case PANEL_LINE_COMMAND_F4:
					return "F4 : reload recipes";

				case PANEL_LINE_COMMAND_LESS_THAN:
					return "< : slower ingredient addition delay";

				case PANEL_LINE_COMMAND_GREATER_THAN:
					return "> : faster ingredient addition delay";

				#endregion Commands Panel Lines

				default:
					throw new NotImplementedException();
			}
		}
	}
}