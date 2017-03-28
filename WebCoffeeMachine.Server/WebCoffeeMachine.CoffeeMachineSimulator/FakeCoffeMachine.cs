using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using WebCoffeeMachine.Domain;
using WebCoffeeMachine.Domain.ServerArduinoComm;
using static WebCoffeeMachine.CoffeeMachineSimulator.Constants;
using static WebCoffeeMachine.Domain.ServerArduinoComm.Constants;

namespace WebCoffeeMachine.CoffeeMachineSimulator
{
    public class FakeCoffeMachine
    {
        private static object _singletonSync = new object();

        private static FakeCoffeMachine __singleton;
#warning make ingredients get from app config
        private readonly LinkedList<string> _ingredients;

        private LinkedList<string> _recipes;

        private bool _isConnected;

        private int _communicationPin;

        private DateTime _lastReceivedRequest;

        private bool _isMakingCoffee;

        private int _coffeeLevel;

        private int _waterMl;

        private LinkedListNode<string> _selectedRecipe;

        private LinkedListNode<string> _selectedIngredient;

        private int _connectionAttemptCounter = 0;

        private object _connectionSync = new object();

        public event Action<string> StatusChangeEvent;

        public static FakeCoffeMachine Singleton {
            get {
                if (__singleton == null) {
                    lock (_singletonSync) {
                        if (__singleton == null)
                            __singleton = new FakeCoffeMachine();
                    }
                }
                return __singleton;
            }
        }

        public bool IsRegistered {
            get { return _isConnected; }
            set {
                _isConnected = value;
                StatusChangeEvent(PANEL_LINE_IS_REGISTERED);
            }
        }

        public int Pin {
            get { return _communicationPin; }
            set {
                _communicationPin = value;
                StatusChangeEvent(PANEL_LINE_PIN);
            }
        }

        public DateTime LastReceivedRequest {
            get => _lastReceivedRequest;
            set {
                _lastReceivedRequest = value;
                StatusChangeEvent(PANEL_LINE_LAST_RECEIVED_REQUEST);
            }
        }

        public bool IsMakingCoffee {
            get { return _isMakingCoffee; }
            set {
                _isMakingCoffee = value;
                StatusChangeEvent(PANEL_LINE_IS_MAKING_COFFEE);
            }
        }

        public int CoffeeLevel {
            get { return _coffeeLevel; }
            set {
                _coffeeLevel = value;
                if (_coffeeLevel > 100)
                    _coffeeLevel = 100;
                else if (_coffeeLevel < 0)
                    _coffeeLevel = 0;
                StatusChangeEvent(PANEL_LINE_COFFEE);
            }
        }

        public int WaterMl {
            get { return _waterMl; }
            set {
                _waterMl = value;
                if (_waterMl > 1000)
                    _waterMl = 1000;
                else if (_waterMl < 0)
                    _waterMl = 0;
                StatusChangeEvent(PANEL_LINE_WATER);
            }
        }

        public string SelectedRecipe {
            get { return _selectedRecipe.Value; }
            set {
                switch (value) {
                    case RECIPES_NEXT:
                        _selectedRecipe = _selectedRecipe.NextOrFirst();
                        break;

                    case RECIPES_PREVIOUS:
                        _selectedRecipe = _selectedRecipe.PreviousOrLast();
                        break;

                    default:
                        return;
                }
                StatusChangeEvent(PANEL_LINE_RECIPE);
            }
        }

        public string SelectedIngredient {
            get { return _selectedIngredient.Value; }
            set {
                switch (value) {
                    case RECIPES_NEXT:
                        _selectedIngredient = _selectedIngredient.NextOrFirst();
                        break;

                    case RECIPES_PREVIOUS:
                        _selectedIngredient = _selectedIngredient.PreviousOrLast();
                        break;

                    default:
                        return;
                }
                StatusChangeEvent(PANEL_LINE_INGREDIENT);
            }
        }

        private Task RegistrationTask {
            get => Task.Factory.StartNew(() => {
                if (IsRegistered)
                    return;

                lock (_connectionSync) {
                    if (IsRegistered)
                        return;

                    try {
                        Dashboard.LogAsync($"Server registration attempt #{++_connectionAttemptCounter}.");
                        var url = AppConfig.RegistrationUrl;
                        var requestJson = JObject.FromObject(
                            new RegistrationRequest() {
                                i = AppConfig.SimulatorIp,
                                p = AppConfig.SimulatorPort,
                                un = AppConfig.SimulatorUniqueName
                            });
                        Dashboard.LogAsync($"Json created.");

                        var requestStr = requestJson.ToString();
                        var requestBuffer = new System.Text.UTF8Encoding().GetBytes(requestStr);
                        Dashboard.LogAsync($"Json serialized (length = {requestBuffer.Length}).");

                        WebRequest request = WebRequest.Create(url);
                        request.Method = POST;
                        request.ContentType = $"{APPLICATION_JSON}; {CHARSET_UTF8}";
                        request.Timeout = AppConfig.RegistrationTimeout;
                        request.ContentLength = requestBuffer.Length;
                        Dashboard.LogAsync($"Web request set up.");

                        Dashboard.LogAsync($"Attempt to open stream.");
                        StreamWriter streamWriter;
                        using (streamWriter = new StreamWriter(request.GetRequestStream())) {
                            try {
                                Dashboard.LogAsync($"Stream writer opened.");
                                streamWriter.Write(requestJson);
                                Dashboard.LogAsync($"Stream writen.");
                                streamWriter.Flush();
                                Dashboard.LogAsync($"Stream flushed.");
                                streamWriter.Close();
                                Dashboard.LogAsync($"Stream stream closed.");
                            } catch (Exception ex) {
                                Dashboard.LogAsync($"Exception thrown while trying to write stream.");
                                if (streamWriter != null) {
                                    streamWriter.Close();
                                    Dashboard.LogAsync($"Stream stream closed.");
                                }
                            }
                        }

                        var response = (HttpWebResponse)request.GetResponse();

                        if (response.StatusCode != HttpStatusCode.OK)
                            return;

                        using (var streamReader = new StreamReader(response.GetResponseStream())) {
                            var responsestr = streamReader.ReadToEnd();
                            var responseJson = JObject.Parse(responsestr);
                            var registrationResponse = responseJson.ToObject<RegistrationResponse>();
                            Pin = registrationResponse.p;
                            IsRegistered = true;
                        }
                    } catch (Exception) {
                        Dashboard.LogAsync("Ooops problem at registration task.");
                    }
                }
            });
        }

        private FakeCoffeMachine()
        {
            _waterMl = 1000;
            _coffeeLevel = 100;
            _isConnected = false;
            _isMakingCoffee = false;
            _communicationPin = 0;
#warning make ingredients load like recipes (from app config)
            _ingredients = new LinkedList<string>(new string[] { INGREDIENTS_COFFEE, INGREDIENTS_WATER });
            _selectedIngredient = _ingredients.First;

            LoadRecipesAsync();

            StartManagingRegistrationAsync();
        }

        public void LoadRecipesAsync()
        {
            // gambiarra
            if (_recipes == null)
                _recipes = new LinkedList<string>();

            lock (_recipes) {
                _recipes = null;
                _recipes = new LinkedList<string>(Recipes.Singleton.LoadRecipesAsync());
                _selectedRecipe = _recipes.First;
            }
        }

        public void IncrementSelectedIngredient(int increment)
        {
            IncrementIngredient(_selectedIngredient.Value, increment);
        }

        private void IncrementIngredient(string ingredient, int increment)
        {
            switch (ingredient) {
                case INGREDIENTS_COFFEE:
                    CoffeeLevel += increment;
                    break;

                case INGREDIENTS_WATER:
                    WaterMl += increment;
                    break;

                default:
                    return;
            }
        }

        public MakeCoffeeResponseEnum MakeSelectedCoffee()
        {
            return MakeCoffee(Recipes.Singleton[_selectedRecipe.Value]);
        }

        public MakeCoffeeResponseEnum MakeCoffee(Dictionary<char, int> recipe)
        {
            lock (this) {
                if (IsMakingCoffee) {
                    Dashboard.LogAsync("Order rejected, CM already busy.");
                    return MakeCoffeeResponseEnum.Busy;
                }

                var coffeeMeasures = recipe['c'];
                var originalCoffeeLevel = CoffeeLevel;

                var waterMl = recipe['w'];
                var originalWaterLevel = WaterMl;

                if (originalCoffeeLevel < coffeeMeasures || originalWaterLevel < waterMl) {
                    Dashboard.LogAsync($"Order rejected, not enough ingredients.");
                    return MakeCoffeeResponseEnum.NotEnoughIngredients;
                }

                Task.Factory.StartNew(() => {
                    Dashboard.LogAsync($"Order ACCEPTED");
                    IsMakingCoffee = true;

                    Dashboard.LogAsync($"Adding coffee.");
                    while (CoffeeLevel > originalCoffeeLevel - coffeeMeasures) {
                        Thread.Sleep(AppConfig.IngredientAdditionDelayMs);
                        CoffeeLevel = CoffeeLevel - COFFEE_ITERATION_DECREMENT;
                    }

                    Dashboard.LogAsync($"Adding water.");
                    while (WaterMl > originalWaterLevel - waterMl) {
                        Thread.Sleep(AppConfig.IngredientAdditionDelayMs);
                        WaterMl = WaterMl - WATER_ITERATION_DECREMENT;
                    }
                    IsMakingCoffee = false;
                    Dashboard.LogAsync($"Ok, we are done.");
                });
            }

            return MakeCoffeeResponseEnum.Ok;
        }

        public void StartManagingRegistrationAsync()
        {
            Task.Factory.StartNew(() => {
                while (true) {
                    if (!IsRegistered) {
                        var registrationTask = RegistrationTask;
                        registrationTask.Wait();
                        if (!IsRegistered)
                            Thread.Sleep(AppConfig.RegistrationWaitAfterFailedAttempMs);
                        else
                            Thread.Sleep(AppConfig.RegistrationWaitAfterSuccessfulAttempMs);
                    } else {
                        Thread.Sleep(AppConfig.RegistrationManagerSleepMs);
                        var elapsedTime = DateTime.Now - LastReceivedRequest;
                        if (elapsedTime > TimeSpan.FromMilliseconds(AppConfig.MaxTimeElapsedBetweenReceivedRequestsMs))
                            Dashboard.LogAsync("Uh oh, server doesn't love me...");
                    }
                }
            });
        }
    }
}