using System;
using WebCoffeeMachine.Domain;
using static WebCoffeeMachine.CoffeeMachineSimulator.Constants;

namespace WebCoffeeMachine.CoffeeMachineSimulator
{
    public class SimulatorPanelLineBuilder : PanelLineBuilder
    {
        public override string Build(string lineName)
        {
            switch (lineName) {
                case PANEL_LINE_IP:
                    return $"IP: {AppConfig.SimulatorIp}";

                case PANEL_LINE_PORT:
                    return $"PORT: {AppConfig.SimulatorPort}";

                case PANEL_LINE_SIMULATOR_UNIQUE_NAME:
                    return $"Simulator unique name : {AppConfig.SimulatorUniqueName}";

                case PANEL_LINE_PIN:
                    return $"Communication PIN : {FakeCoffeMachine.Singleton.Pin}";

                case PANEL_LINE_IS_REGISTERED:
                    return FakeCoffeMachine.Singleton.IsRegistered ? "Registered" : "NOT Registered";

                case PANEL_LINE_IS_MAKING_COFFEE:
                    return FakeCoffeMachine.Singleton.IsMakingCoffee ? "Making some coffee" : "Waiting";

                case PANEL_LINE_COFFEE:
                    return $"Coffee : {FakeCoffeMachine.Singleton.CoffeeLevel:00}%";

                case PANEL_LINE_WATER:
                    return $"Water : {FakeCoffeMachine.Singleton.WaterMl:0}ml";

                case PANEL_LINE_INGREDIENT:
                    return $"Selected ingredient : {FakeCoffeMachine.Singleton.SelectedIngredient}";

                case PANEL_LINE_RECIPE:
                    return $"Selected recipe : {FakeCoffeMachine.Singleton.SelectedRecipe}";

                case PANEL_LINE_LAST_EVENT:
                    return $"Last event : {DateTime.Now}";

                case PANEL_LINE_INGREDIENT_ADDITION_DELAY:
                    return $"Ingredient addition delay : {AppConfig.IngredientAdditionDelayMs}ms";

                case PANEL_LINE_INGREDIENT_KEYBOARD_INCREMENT:
                    return $"Ingredient keyboard increment : {AppConfig.IngredientKeyboardIncrement}";

                case PANEL_LINE_LAST_RECEIVED_REQUEST:
                    return $"Last received request : {FakeCoffeMachine.Singleton.LastReceivedRequest}";

                case PANEL_LINE_REGISTRATION_REQUEST_TIMEOUT:
                    return $"Registration request timeout : {AppConfig.RegistrationTimeout}ms";

                case PANEL_LINE_WAIT_AFTER_REGISTRATION_FAILED_ATTEMPT:
                    return $"Wait time after FAILED reg. : {AppConfig.RegistrationWaitAfterFailedAttempMs}ms";

                case PANEL_LINE_WAIT_AFTER_REGISTRATION_SUCCESSFUL_ATTEMPT:
                    return $"Wait time after SUCCESSFUL reg. : {AppConfig.RegistrationWaitAfterSuccessfulAttempMs}ms";

                case PANEL_LINE_REGISTRATION_MANAGER_SLEEP:
                    return $"Reg. manager sleep : {AppConfig.RegistrationManagerSleepMs}ms";

                case PANEL_LINE_MAX_TIME_ELAPSED_BETWEEN_RECEIVED_REQUESTS:
                    return $"Max time between received req.s : {AppConfig.MaxTimeElapsedBetweenReceivedRequestsMs}ms";

                default:
                    throw new NotImplementedException();
            }
        }
    }
}