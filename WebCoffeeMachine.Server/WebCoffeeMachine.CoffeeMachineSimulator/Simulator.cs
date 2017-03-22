using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebCoffeeMachine.Domain;

namespace WebCoffeeMachine.CoffeeMachineSimulator
{
    public class Simulator
    {
        private static object _singletonSync = new object();
        private static Simulator __singleton;
        public static Simulator Singleton {
            get {
                if(__singleton == null) {
                    lock (_singletonSync) {
                        if (__singleton == null)
                            __singleton = new Simulator();
                    }
                }
                return __singleton;
            }
        }
        private readonly Dictionary<string, Dictionary<char, int>> _recipes;

        private readonly List<string> _ingredients;

        private string _uniqueName;

        public string UniqueName {
            get { return _uniqueName; }
            set {
                _uniqueName = value;
                SimulatorDashboard.RefreshSimulatorStatus();
            }
        }

        private bool _isMakingCoffee;

        public bool IsMakingCoffee {
            get { return _isMakingCoffee; }
            set {
                _isMakingCoffee = value;
                SimulatorDashboard.RefreshSimulatorStatus();
            }
        }

        private int _coffeeLevel;

        public int CoffeeLevel {
            get { return _coffeeLevel; }
            set {
                _coffeeLevel = value;
                if (_coffeeLevel > 100)
                    _coffeeLevel = 100;
                else if (_coffeeLevel < 0)
                    _coffeeLevel = 0;
                SimulatorDashboard.RefreshSimulatorStatus();
            }
        }

        private int _waterMl;

        public int WaterMl {
            get { return _waterMl; }
            set {
                _waterMl = value;
                if (_waterMl > 1000)
                    _waterMl = 1000;
                else if (_waterMl < 0)
                    _waterMl = 0;
                SimulatorDashboard.RefreshSimulatorStatus();
            }
        }

        private bool _isConnected;

        public bool IsConnected {
            get { return _isConnected; }
            set {
                _isConnected = value;
                SimulatorDashboard.RefreshSimulatorStatus();
            }
        }

        private int _communicationPin;

        public int CommunicationPin {
            get { return _communicationPin; }
            set {
                _communicationPin = value;
                SimulatorDashboard.RefreshSimulatorStatus();
            }
        }

        private int _selectedRecipeIndex;

        public string SelectedRecipe {
            get { return _recipes.ElementAt(_selectedRecipeIndex).Key; }
            set {
                switch (value) {
                    case "forward":
                        if (++_selectedRecipeIndex >= _recipes.Count)
                            _selectedRecipeIndex = 0;
                        break;

                    case "back":
                        if (--_selectedRecipeIndex < 0)
                            _selectedRecipeIndex = _recipes.Count - 1;
                        break;

                    default:
                        return;
                }
                SimulatorDashboard.RefreshSimulatorStatus();
            }
        }

        private int _selectedIngredientIndex;

        public string SelectedIngredient {
            get { return _ingredients[_selectedIngredientIndex]; }
            set {
                switch (value) {
                    case "forward":
                        if (++_selectedIngredientIndex >= _ingredients.Count)
                            _selectedIngredientIndex = 0;
                        break;

                    case "back":
                        if (--_selectedIngredientIndex < 0)
                            _selectedIngredientIndex = _ingredients.Count - 1;
                        break;

                    default:
                        return;
                }
                SimulatorDashboard.RefreshSimulatorStatus();
            }
        }

        private Simulator()
        {
            _recipes = new Dictionary<string, Dictionary<char, int>>();

            var expresso = new Dictionary<char, int>();
            expresso.Add('c', 1);
            expresso.Add('w', 100);

            var doubleExpresso = new Dictionary<char, int>();
            doubleExpresso.Add('c', 2);
            doubleExpresso.Add('w', 100);

            _recipes.Add("expresso", expresso);
            _recipes.Add("double", doubleExpresso);

            _ingredients = new List<string>(new string[] { "coffee", "water" });

            _uniqueName = AppConfig.SimulatorTitle;
            _waterMl = 1000;
            _coffeeLevel = 100;
            _isConnected = false;
            _isMakingCoffee = false;
            _communicationPin = 0;
            _selectedRecipeIndex = 0;
            _selectedIngredientIndex = 0;
        }

        public void HandleCommand(ConsoleKeyInfo consoleKeyInfo)
        {
            Task.Factory.StartNew(() => {
                switch (consoleKeyInfo.Key) {
                    case ConsoleKey.Tab:
                        SelectedRecipe = "forward";
                        break;

                    case ConsoleKey.Enter:
                        SimulatorDashboard.Log($"Order of <<{SelectedRecipe}>> received from SIMULATOR.");
                        MakeCoffee(_recipes[SelectedRecipe]);
                        break;

                    case ConsoleKey.LeftArrow:
                        SelectedIngredient = "back";
                        break;

                    case ConsoleKey.RightArrow:
                        SelectedIngredient = "forward";
                        break;

                    case ConsoleKey.UpArrow:
                        IncrementIngredient(_ingredients[_selectedIngredientIndex], AppConfig.Increment);
                        break;

                    case ConsoleKey.DownArrow:
                        IncrementIngredient(_ingredients[_selectedIngredientIndex], -1 * AppConfig.Increment);
                        break;

                    default:
                        return;
                }
            });
        }

        private void IncrementIngredient(string ingredient, int increment)
        {
            switch (ingredient) {
                case "coffee":
                    CoffeeLevel += increment;
                    break;

                case "water":
                    WaterMl += increment;
                    break;

                default:
                    return;
            }
        }

        private object _syncObj = new object();

        public MakeCoffeeResponseEnum MakeCoffee(Dictionary<char, int> recipe)
        {
            lock (_syncObj) {
                if (IsMakingCoffee) {
                    SimulatorDashboard.Log("Order rejected because the coffee machine is already busy.");
                    return MakeCoffeeResponseEnum.Busy;
                }

                var coffeeMeasures = recipe['c'];
                var originalCoffeeLevel = CoffeeLevel;

                var waterMl = recipe['w'];
                var originalWaterLevel = WaterMl;

                if (originalCoffeeLevel < coffeeMeasures * AppConfig.Increment ||
                    originalWaterLevel < waterMl) {
                    SimulatorDashboard.Log($"Order rejected because there is not enough ingredients.");
                    return MakeCoffeeResponseEnum.NotEnoughIngredients;
                }

                Task.Factory.StartNew(() => {
                    SimulatorDashboard.Log($"LET'S MAKE SOME COFFEE");
                    IsMakingCoffee = true;
                    SimulatorDashboard.Log($"Adding coffee.");
                    while (CoffeeLevel > originalCoffeeLevel - coffeeMeasures * AppConfig.Increment) {
                        Thread.Sleep(AppConfig.IngredientAdditionBaseDelayMs);
                        CoffeeLevel = CoffeeLevel - 1;
                    }

                    SimulatorDashboard.Log($"Adding water.");
                    while (WaterMl > originalWaterLevel - waterMl) {
                        Thread.Sleep(AppConfig.IngredientAdditionBaseDelayMs);
                        WaterMl = WaterMl - 10;
                    }
                    IsMakingCoffee = false;
                    SimulatorDashboard.Log($"Ok, we are done.");
                });
            }

            return MakeCoffeeResponseEnum.Ok;
        }

        public void MakeCoffeeFromSimulator(Dictionary<char, int> recipe)
        {
        }

        public List<string> ToPanel()
            => new List<string>() {
                AppConfig.SimulatorTitle,
                $"IP: {AppConfig.SimulatorIp}",
                $"PORT: {AppConfig.SimulatorPort}",
                $"Simulator unique name : {UniqueName}",
                $"Communication PIN : {CommunicationPin}",
                (IsConnected ? "Connection OK" : "Disconnected") + " | " + (IsMakingCoffee ? "Making some coffee" : "Waiting"),
                $"Coffee : {CoffeeLevel:00}%",
                $"Water : {WaterMl:0}ml",
                $"Selected ingredient : {SelectedIngredient}",
                $"Selected recipe : {SelectedRecipe}"
            };
    }
}