using System;
using System.Threading.Tasks;
using Mkafeina.Domain;
using static Mkafeina.Simulator.Constants;
using Mkafeina.CoffeeMachineSimulator;

namespace Mkafeina.Simulator
{
    public class SimulatorCommandInterpreter : CommandInterpreter
    {
        public override void HandleCommand(ConsoleKeyInfo key)
        {
            Task.Factory.StartNew(() => {
                switch (key.Key) {
                    case ConsoleKey.Tab:
                        if ((key.Modifiers & ConsoleModifiers.Shift) != 0)
                            CookBook.Sgt.PreviousRecipe();
                        else
							CookBook.Sgt.NextRecipe();
                        break;

                    case ConsoleKey.Enter:
                        SimulatorDashboard.Singleton.LogAsync($"Keyboard <<{CookBook.Sgt.SelectedRecipeName()}>> order.");
                        FakeCoffeMachine.Singleton.MakeCoffee(CookBook.Sgt.SelectedRecipe());
                        break;

                    case ConsoleKey.LeftArrow:
						FakeCoffeMachine.Singleton.PreviousIngredient();
                        break;

                    case ConsoleKey.RightArrow:
						FakeCoffeMachine.Singleton.NextIngredient();
                        break;

                    case ConsoleKey.UpArrow:
                        FakeCoffeMachine.Singleton.IncrementSelectedIngredient();
                        break;

                    case ConsoleKey.DownArrow:
                        FakeCoffeMachine.Singleton.IncrementSelectedIngredient(negative:true);
                        break;

                    case ConsoleKey.F5:
                        SimulatorAppConfig.Singleton.ReloadConfigs();
                        SimulatorDashboard.Singleton.ReloadAllPanelsAsync(SimulatorAppConfig.Singleton.PanelsConfigs);
                        break;

                    case ConsoleKey.F4:
						CookBook.Sgt.LoadRecipes();
                        break;

                    case ConsoleKey.OemPeriod: // >
                        if ((key.Modifiers & ConsoleModifiers.Shift) != 0)
                            SimulatorAppConfig.Singleton.IngredientAdditionDelayMs += INGREDIENT_ADDITION_DELAY_INCREMENT;

                        break;

                    case ConsoleKey.OemComma: // <
                        if ((key.Modifiers & ConsoleModifiers.Shift) != 0) {
                            SimulatorAppConfig.Singleton.IngredientAdditionDelayMs -= INGREDIENT_ADDITION_DELAY_INCREMENT;
                        }
                        break;

					case ConsoleKey.I:
						FakeCoffeMachine.Singleton.TurnOn();
						break;

					case ConsoleKey.O:
						FakeCoffeMachine.Singleton.TurnOff();
						break;

                    default:
                        return;
                }
            });
        }
    }
}