using System;
using System.Threading.Tasks;
using Mkfeina.Domain;
using static Mkfeina.CoffeeMachineSimulator.Constants;

namespace Mkfeina.CoffeeMachineSimulator
{
    public class SimulatorCommandInterpreter : CommandInterpreter
    {
        public override void HandleCommand(ConsoleKeyInfo key)
        {
            Task.Factory.StartNew(() => {
                switch (key.Key) {
                    case ConsoleKey.Tab:
                        if ((key.Modifiers & ConsoleModifiers.Shift) != 0)
                            FakeCoffeMachine.Singleton.SelectedRecipe = RECIPES_PREVIOUS;
                        else
                            FakeCoffeMachine.Singleton.SelectedRecipe = RECIPES_NEXT;
                        break;

                    case ConsoleKey.Enter:
                        Dashboard.LogAsync($"Keyboard <<{FakeCoffeMachine.Singleton.SelectedRecipe}>> order.");
                        FakeCoffeMachine.Singleton.MakeSelectedCoffee();
                        break;

                    case ConsoleKey.LeftArrow:
                        FakeCoffeMachine.Singleton.SelectedIngredient = RECIPES_PREVIOUS;
                        break;

                    case ConsoleKey.RightArrow:
                        FakeCoffeMachine.Singleton.SelectedIngredient = RECIPES_NEXT;
                        break;

                    case ConsoleKey.UpArrow:
                        FakeCoffeMachine.Singleton.IncrementSelectedIngredient(AppConfig.IngredientKeyboardIncrement);
                        break;

                    case ConsoleKey.DownArrow:
                        FakeCoffeMachine.Singleton.IncrementSelectedIngredient(-1 * AppConfig.IngredientKeyboardIncrement);
                        break;

                    case ConsoleKey.F5:
                        AppConfig.LoadAppConfig();
                        Dashboard.ReloadPanels();
                        break;

                    case ConsoleKey.F4:
                        FakeCoffeMachine.Singleton.LoadRecipesAsync();
                        break;

                    case ConsoleKey.OemPeriod:
                        if ((key.Modifiers & ConsoleModifiers.Shift) != 0)
                            AppConfig.IngredientAdditionDelayMs = AppConfig.IngredientAdditionDelayMs + INGREDIENT_ADDITION_DELAY_INCREMENT;

                        break;

                    case ConsoleKey.OemComma:
                        if ((key.Modifiers & ConsoleModifiers.Shift) != 0) {
                            AppConfig.IngredientAdditionDelayMs = AppConfig.IngredientAdditionDelayMs - INGREDIENT_ADDITION_DELAY_INCREMENT;
                        }
                        break;

                    default:
                        return;
                }
            });
        }
    }
}