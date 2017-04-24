using Mkfeina.CoffeeMachineSimulator;
using Mkfeina.Domain;
using Mkfeina.Domain.ServerArduinoComm;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Mkfeina.Domain.ServerArduinoComm.Constants;
using static Mkfeina.Simulator.Constants;

namespace Mkfeina.Simulator
{
	public class FakeCoffeMachine
	{
		//private byte[] KEY = new byte[] { 42, 33, 13 };
		private byte[] KEY = new byte[] { 1, 2, 3 };

#warning tranformar os dois em configs
		private bool ENCRYPTION_ENABLE = true;

		#region Singleton Stuff

		private static object _singletonSync = new object();

		private static FakeCoffeMachine __singleton;

		public static FakeCoffeMachine Singleton {
			get {
				if (__singleton == null)
				{
					lock (_singletonSync)
					{
						if (__singleton == null)
							__singleton = new FakeCoffeMachine();
					}
				}
				return __singleton;
			}
		}

		private FakeCoffeMachine()
		{
			_waterMl = 1000;
			_coffeeLevel = 100;
			_isRegistered = false;
			_isMakingCoffee = false;
			_ingredients = new LinkedList<string>(new string[] { INGREDIENTS_COFFEE, INGREDIENTS_WATER });
			_selectedIngredient = _ingredients.First;
			_isEnabled = true;
		}

		#endregion Singleton Stuff

		#region Ingredient Selection

		private readonly LinkedList<string> _ingredients;
		private LinkedListNode<string> _selectedIngredient;

		public void NextIngredient()
		{
			_selectedIngredient = _selectedIngredient.NextOrFirst();
			StatusChangeEvent(PANEL_LINE_SELECTED_INGREDIENT);
		}

		public void PreviousIngredient()
		{
			_selectedIngredient = _selectedIngredient.PreviousOrLast();
			StatusChangeEvent(PANEL_LINE_SELECTED_INGREDIENT);
		}

		public string SelectedIngredient { get => _selectedIngredient.Value; }

		public void IncrementSelectedIngredient(bool negative = false)
		{
			var increment = negative ? -1 * INGREDIENT_KEY_BOARD_INCREMENT : INGREDIENT_KEY_BOARD_INCREMENT;
			switch (_selectedIngredient.Value)
			{
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

		#endregion Ingredient Selection

		#region Coffee Machine State Stuff

		public event Action<string> StatusChangeEvent;

#warning exibir no dash
		public string UniqueName { get; set; }

		private bool _isEnabled;

		public bool IsEnabled {
			get => _isEnabled;
			set {
				_isEnabled = value;
#warning criar panel line e panel line builder + comandos de enable disable
				StatusChangeEvent?.Invoke(PANEL_LINE_IS_ENABLED);
			}
		}

		private bool _isRegistered;

		public bool IsRegistered {
			get => _isRegistered;
			set {
				_isRegistered = value;
				StatusChangeEvent?.Invoke(PANEL_LINE_REGISTRATION);
			}
		}

		private bool _isMakingCoffee;

		public bool IsMakingCoffee {
			get { return _isMakingCoffee; }
			set {
				_isMakingCoffee = value;
				StatusChangeEvent?.Invoke(PANEL_LINE_IS_MAKING_COFFEE);
			}
		}

		private int _coffeeLevel;

		public int CoffeeLevel {
			get { return _coffeeLevel; }
			set {
				_coffeeLevel = value;
#warning contants...
				if (_coffeeLevel > 100)
					_coffeeLevel = 100;
				else if (_coffeeLevel < 0)
					_coffeeLevel = 0;
				StatusChangeEvent?.Invoke(PANEL_LINE_COFFEE_LEVEL);
			}
		}

		private int _waterMl;

		public int WaterMl {
			get { return _waterMl; }
			set {
#warning contants...
				_waterMl = value;
				if (_waterMl > 1000)
					_waterMl = 1000;
				else if (_waterMl < 0)
					_waterMl = 0;
				StatusChangeEvent?.Invoke(PANEL_LINE_WATER_LEVEL);
			}
		}

		#endregion Coffee Machine State Stuff

		public MakeCoffeeResponseEnum MakeCoffee(Recipe recipe) => MakeCoffee(recipe, out Task doesntMatter);

		public MakeCoffeeResponseEnum MakeCoffee(Recipe recipe, out Task processingTask)
		{
			processingTask = null;
			lock (this)
			{
				if (IsMakingCoffee)
				{
					SimulatorDashboard.Singleton.LogAsync("Order rejected, CM already busy.");
					return MakeCoffeeResponseEnum.Busy;
				}

				var coffeeMeasures = recipe['c'];
				var originalCoffeeLevel = CoffeeLevel;

				var waterMl = recipe['w'];
				var originalWaterLevel = WaterMl;

				if (originalCoffeeLevel < coffeeMeasures || originalWaterLevel < waterMl)
				{
					SimulatorDashboard.Singleton.LogAsync($"Order rejected, not enough ingredients.");
					return MakeCoffeeResponseEnum.NotEnoughIngredients;
				}

				processingTask = Task.Factory.StartNew(() =>
				{
					SimulatorDashboard.Singleton.LogAsync($"Order ACCEPTED");
					IsMakingCoffee = true;

					SimulatorDashboard.Singleton.LogAsync($"Adding coffee.");
					while (CoffeeLevel > originalCoffeeLevel - coffeeMeasures)
					{
						Thread.Sleep(SimulatorAppConfig.Singleton.IngredientAdditionDelayMs);
						CoffeeLevel = CoffeeLevel - COFFEE_ITERATION_DECREMENT;
					}

					SimulatorDashboard.Singleton.LogAsync($"Adding water.");
					while (WaterMl > originalWaterLevel - waterMl)
					{
						Thread.Sleep(SimulatorAppConfig.Singleton.IngredientAdditionDelayMs);
						WaterMl = WaterMl - WATER_ITERATION_DECREMENT;
					}
					IsMakingCoffee = false;
					SimulatorDashboard.Singleton.LogAsync($"Ok, we are done.");
				});
			}

			return MakeCoffeeResponseEnum.Ok;
		}

		#region Main Task

		private CancellationTokenSource _mainTaksCancelTokenSource = new CancellationTokenSource();

		private Task _mainTask = null;

		private Task CreateMainTask()
			=> new Task(() =>
			   {
				   Register();
				   while (true)
				   {
					   if (_mainTaksCancelTokenSource.Token.IsCancellationRequested)
						   return;

					   if (IsEnabled)
					   {
						   ReportLevels();
						   switch (_command)
						   {
							   case CommandEnum.Undefined:
								   SimulatorDashboard.Singleton.LogAsync("ATTENTION! Undefined command...");
								   continue;

							   case CommandEnum.DoNothing:
								   continue;

							   case CommandEnum.GetCoffeeOrder:
								   TakeOrder();
								   Task processingTask;
								   MakeCoffeeResponseEnum makeCoffeeResponse;
								   if (_orderReference > 0 && _recipe != null)
									   makeCoffeeResponse = MakeCoffee(_recipe, out processingTask);
								   else
									   continue;

								   if (makeCoffeeResponse == MakeCoffeeResponseEnum.Ok)
								   {
									   processingTask.Wait();
									   TellServerOrderIsReady();
								   }
								   else
									   ReportProblemDuringOrderProcessing();
								   continue;

							   case CommandEnum.Disable:
								   DisableCoffeeMachine();
								   continue;

							   case CommandEnum.DisablingConfirmed:
								   Disable();
								   _command = CommandEnum.DoNothing;
								   continue;

							   default:
								   continue;
						   }
					   }
				   }
				   SimulatorDashboard.Singleton.LogAsync("Main task ENDED.");
			   }, _mainTaksCancelTokenSource.Token);

		private Task MainTask {
			get {
				if (_mainTask == null)
					_mainTask = CreateMainTask();
				return _mainTask;
			}
		}

		#endregion Main Task

		#region Operation aux objs

		private int _registrationAttemptCounter = 0;

		private object _communicationSyncOnj = new object();

		private CommandEnum _command;

		private bool _securityDisable = false;

		private uint _orderReference = 0;

		private Recipe _recipe = null;

		#endregion Operation aux objs

		#region Communication power switch and enable

		public void TurnOn()
		{
			if (MainTask.Status == TaskStatus.Running)
				return;
			MainTask.Start();
			SimulatorDashboard.Singleton.LogAsync("Main task started.");
		}

		public void TurnOff()
		{
			if (MainTask.Status != TaskStatus.Running)
				return;
			_mainTaksCancelTokenSource.Cancel();
			SimulatorDashboard.Singleton.LogAsync("Main task will stop.");
			Unregister();
		}

		public void Enable()
		{
			SimulatorDashboard.Singleton.LogAsync("Fake coffee machine ENABLED.");
			IsEnabled = true;
		}

		public void Disable()
		{
			SimulatorDashboard.Singleton.LogAsync("Fake coffee machine DISABLED.");
			IsEnabled = false;
		}

		#endregion Communication power switch and enable

		#region Communication stuff

		private TResponse SendHttpRequest<TRequest, TResponse>(TRequest requestObj, string url, int timeout = -1, string method = POST)
			where TRequest : Request
			where TResponse : Response

		{
			lock (_communicationSyncOnj)
			{
				try
				{
					var registrationJsonStr = JsonConvert.SerializeObject(requestObj, new JsonSerializerSettings()
					{
						ContractResolver = new CamelCasePropertyNamesContractResolver(),
						NullValueHandling = NullValueHandling.Ignore
					});
					var request = (HttpWebRequest)WebRequest.Create(url);
					request.Method = method;
					request.ContentType = $"{APPLICATION_JSON}";
#warning colocar const aqui!!!
					request.Timeout = timeout <= 0 ? timeout = SimulatorAppConfig.Singleton.StandardTimeout : timeout;

					StreamWriter streamWriter;
					using (streamWriter = new StreamWriter(request.GetRequestStream()))
					{
						try
						{
							streamWriter.Write(registrationJsonStr);
							streamWriter.Flush();
							streamWriter.Close();
						}
						catch (Exception exception)
						{
							if (streamWriter != null)
							{
								streamWriter.Close();
								streamWriter.Dispose();
							}
							SimulatorDashboard.Singleton.LogAsync("Problem ocurred during communication (on write stream phase).");
							return null;
						}
					}

					TResponse responseObj;
					var response = (HttpWebResponse)request.GetResponse();
					using (var streamReader = new StreamReader(response.GetResponseStream()))
					{
						var responseStr = streamReader.ReadToEnd();
						//if (ENCRYPTION_ENABLE)
						//	responseStr = responseStr.Decrypt(KEY);
						responseObj = JObject.Parse(responseStr).ToObject<TResponse>();
					}
					return responseObj;
				}
				catch (WebException exception)
				{
					SimulatorDashboard.Singleton.LogAsync($"Web exception during communication. Status <<{exception.Status.ToString()}>>.");
					return null;
				}
				catch (Exception exception)
				{
					SimulatorDashboard.Singleton.LogAsync("Problem ocurred during communication (exception thrown).");
					return null;
				}
			}
		}

		private List<object> SendRequestToServer<TRequest, TResponse>(string subject, string url, TRequest request, params Func<TResponse, object>[] resultAccessors)
			where TRequest : Request
			where TResponse : Response
		{
			var results = new List<object>();
			results.Add(false);
			try
			{
				request.Mac = SimulatorAppConfig.Singleton.SimulatorMac;
				var response = SendHttpRequest<TRequest, TResponse>(request, url);

				if (response == null)
					return results;

				if (response.ResponseCode == (int)ResponseCodeEnum.OK)
				{
					results[0] = true;
					for (var i = 0; resultAccessors != null && i < resultAccessors.Length; i++)
						results.Add(resultAccessors[i].Invoke(response));
					return results;
				}
				else
				{
					SimulatorDashboard.Singleton.LogAsync($"Message <<{subject}>> FAILED (response code {response.ResponseCode}).");
					return results;
				}
			}
			catch (Exception exception)
			{
				SimulatorDashboard.Singleton.LogAsync($"Message <<{subject}>> FAILED (exception thrown).");
				return results;
			}
		}

#warning tranformar em const

		private List<object> TryManyTimesIfNack(Func<List<object>> routine, uint howManyTimes = 10)
		{
			List<object> results = null;
			for (var i = 0; i < howManyTimes; i++)
			{
				results = routine.Invoke();
				if ((bool)results[0])
					break;
			}
			return results;
		}

		#endregion Communication stuff

		#region Registration

#warning delete this

		private void SendNewOffsets(out bool acknowledge)
		{
			acknowledge = false;
			SimulatorDashboard.Singleton.LogAsync($"I will send new offsets to the server.");
			try
			{
				var request = new RegistrationRequest()
				{
					RegistrationMessage = (int)RegistrationMessageEnum.Offsets,
#warning fazer isso controlavel no dash
					CoffeeEmptyOffset = 0,
					CoffeeFullOffset = 100,
					WaterEmptyOffset = 0,
					WaterFullOffset = 1000,
					Mac = SimulatorAppConfig.Singleton.SimulatorMac,
				};

				var response = SendHttpRequest<RegistrationRequest, RegistrationResponse>(request, SimulatorAppConfig.Singleton.RegistrationUrl);

				if (response == null)
					return;

				if (response.ResponseCode != (int)ResponseCodeEnum.OK)
				{
					SimulatorDashboard.Singleton.LogAsync($"Sending new offsets FAILED (response code {response.ResponseCode}).");
				}
				else
				{
					SimulatorDashboard.Singleton.LogAsync($"Sending new offsets SUCCESSFUL.");
					acknowledge = true;
				}
			}
			catch (Exception)
			{
				SimulatorDashboard.Singleton.LogAsync($"Sending new offsets FAILED (exception thrown).");
			}
#warning add comando no dash
		}

		private void Register()
		{
			while (!IsRegistered)
			{
				SimulatorDashboard.Singleton.LogAsync($"I will try to register at \"{SimulatorAppConfig.Singleton.ServerAddress}\".");
				var attemptRequest = new RegistrationRequest()
				{
					RegistrationMessage = (int)RegistrationMessageEnum.AttemptRegistration,
					CoffeeEmptyOffset = 0,
					CoffeeFullOffset = 100,
					WaterEmptyOffset = 0,
					WaterFullOffset = 1000,
					Mac = SimulatorAppConfig.Singleton.SimulatorMac,
					UniqueName = SimulatorAppConfig.Singleton.SimulatorUniqueName
				};

				var results = SendRequestToServer($"Registration attempt",
												  SimulatorAppConfig.Singleton.RegistrationUrl,
												  attemptRequest,
												  (RegistrationResponse r) => r.TrueUniqueName,
												  (RegistrationResponse r) => r.ResponseCode);

				bool hasResponseCode = false;
				if (results.Count == 3)
					hasResponseCode = true;

				if ((bool)results[ACK] || (hasResponseCode && (int)results[REGISTRATION_ATTEMPT_RESPONSE_CODE] == (int)RegistrationResponseCodeEnum.RegisteredButNotAccepted))
				{
					UniqueName = (string)results[TRUE_UNIQUE_NAME];
					SimulatorDashboard.Singleton.LogAsync($"Registration attempt SUCCESSFUL, I will try to accept. Name: {UniqueName}.");

					var acceptanceRequest = new RegistrationRequest()
					{
						RegistrationMessage = (int)RegistrationMessageEnum.RegistrationAcceptance,
						Mac = SimulatorAppConfig.Singleton.SimulatorMac,
					};
					results = TryManyTimesIfNack(() => SendRequestToServer($"Registration acceptance",
																		   SimulatorAppConfig.Singleton.RegistrationUrl,
																		   acceptanceRequest,
																		   (RegistrationResponse r) => r.ResponseCode));

					bool hasResponseCode2 = false;
					if (results.Count == 2)
						hasResponseCode2 = true;

					if ((bool)results[ACK])
					{
						IsRegistered = true;
						SimulatorDashboard.Singleton.LogAsync($"Registration acceptance SUCCESSFUL.");
					}
					else if (hasResponseCode2 && (int)results[REGISTRATION_ACCEPTANCE_RESPONSE_CODE] == (int)RegistrationResponseCodeEnum.AlreadyRegistered)
					{
						IsRegistered = true;
						SimulatorDashboard.Singleton.LogAsync($"Already registered.");
					}
				}
				else if (hasResponseCode && (int)results[REGISTRATION_ATTEMPT_RESPONSE_CODE] == (int)RegistrationResponseCodeEnum.AlreadyRegistered)
				{
					IsRegistered = true;
					SimulatorDashboard.Singleton.LogAsync($"Already registered.");
				}
			}
		}

		private void Unregister()
		{
			while (IsRegistered)
			{
				SimulatorDashboard.Singleton.LogAsync($"I will try to UNregister <<{UniqueName}>>.");
				var unregisterRequest = new RegistrationRequest()
				{
					RegistrationMessage = (int)RegistrationMessageEnum.Unregister,
					Mac = SimulatorAppConfig.Singleton.SimulatorMac
				};

				var results = SendRequestToServer<RegistrationRequest, RegistrationResponse>($"Unregistration of <<{UniqueName}>>",
																							 SimulatorAppConfig.Singleton.RegistrationUrl,
																							 unregisterRequest);
				if ((bool)results[0])
				{
					SimulatorDashboard.Singleton.LogAsync($"UNregistration of <<{UniqueName}>> SUCCESSFUL.");
					IsRegistered = false;
				}
			}
		}

		#endregion Registration

		#region Report

		private void ReportLevels()
		{
			SimulatorDashboard.Singleton.LogAsync($"Reporting levels ({DateTime.Now.ToString()}).");
			var levelsReportRequest = new ReportRequest()
			{
				ReportMessage = (int)ReportMessageEnum.Levels,
				IsEnabled = IsEnabled,
				CoffeeLevel = CoffeeLevel,
				Mac = SimulatorAppConfig.Singleton.SimulatorMac,
				WaterLevel = WaterMl
			};

			var results = SendRequestToServer("Levels report", SimulatorAppConfig.Singleton.ReportUrl, levelsReportRequest, (ReportResponse r) => r.Command);

			if ((bool)results[ACK])
			{
				_command = (CommandEnum)results[COMMAND];
				SimulatorDashboard.Singleton.LogAsync($"Command received : {_command.ToString()}");
			}
			else
				_command = CommandEnum.DoNothing;
		}

		private void DisableCoffeeMachine()
		{
			SimulatorDashboard.Singleton.LogAsync($"I will warn the server that i'm disabling.");
			var disablingRequest = new ReportRequest()
			{
				ReportMessage = (int)ReportMessageEnum.DisablingCoffeeMachine,
				Mac = SimulatorAppConfig.Singleton.SimulatorMac,
			};

#warning transformar em configs
			var results = TryManyTimesIfNack(() => SendRequestToServer<ReportRequest, ReportResponse>("Disabling",
																									  SimulatorAppConfig.Singleton.ReportUrl,
																									  disablingRequest),
																									  100);

			if ((bool)results[ACK])
			{
				_command = (CommandEnum)results[COMMAND];
				if (_command == CommandEnum.DisablingConfirmed)
					SimulatorDashboard.Singleton.LogAsync($"Ok, server is aware that i will disable.");
				else
					SimulatorDashboard.Singleton.LogAsync($"Disabling CANCELED");
			}
			else
				SimulatorDashboard.Singleton.LogAsync($"Server is NOT AWARE that i will disable, but I'll do it anyway.");
		}

		#endregion Report

		#region Order

		private void TakeOrder()
		{
			SimulatorDashboard.Singleton.LogAsync($"I will ask for an order ({DateTime.Now.ToString()}).");
			var giveMeOrderRequest = new OrderRequest()
			{
				OrderMessage = (int)OrderMessageEnum.GiveMeAnOrder,
				Mac = SimulatorAppConfig.Singleton.SimulatorMac
			};

			var results = TryManyTimesIfNack(() => SendRequestToServer("Give me an order",
																	   SimulatorAppConfig.Singleton.OrderUrl,
																	   giveMeOrderRequest,
																	   (OrderResponse r) => r.OrderReference,
																	   (OrderResponse r) => Recipe.Parse(r.Recipe)));

			if ((bool)results[ACK])
			{
				_orderReference = (uint)results[ORDER_REFERENCE];
				_recipe = (Recipe)results[RECIPE];

				SimulatorDashboard.Singleton.LogAsync($"Order taken (ref {_orderReference}), i'll tell the server that i'll prepare it.");
				var processingWillStartRequest = new OrderRequest()
				{
					OrderMessage = (int)OrderMessageEnum.ProcessingWillStart,
					OrderReference = _orderReference,
					Mac = SimulatorAppConfig.Singleton.SimulatorMac
				};

				results = TryManyTimesIfNack(() => SendRequestToServer("Give me an order",
																	   SimulatorAppConfig.Singleton.ReportUrl,
																	   processingWillStartRequest,
																	   (OrderResponse r) => r.OrderReference,
																	   (OrderResponse r) => Recipe.Parse(r.Recipe)));

				if ((bool)results[ACK])
					SimulatorDashboard.Singleton.LogAsync($"Ok, server knows that ref {_orderReference} will processed.");
				else
				{
					SimulatorDashboard.Singleton.LogAsync($"I couldn't tell the server that {_orderReference} would be processed.");
					SimulatorDashboard.Singleton.LogAsync($"Order {_orderReference} CANCELED.");
					_orderReference = 0;
					_recipe = null;
				}
			}
			else
			{
				SimulatorDashboard.Singleton.LogAsync("I couldn't get an order...");
				return;
			}
		}

		private void TellServerOrderIsReady()
		{
			SimulatorDashboard.Singleton.LogAsync($"I will tell the server that ref {_orderReference} is READY!!!.");

			var orderReadyRequest = new OrderRequest()
			{
				OrderMessage = (int)OrderMessageEnum.OrderReady,
				OrderReference = _orderReference,
				Mac = SimulatorAppConfig.Singleton.SimulatorMac
			};

			var results = TryManyTimesIfNack(() => SendRequestToServer<OrderRequest, OrderResponse>("Order ready!",
																									SimulatorAppConfig.Singleton.OrderUrl,
																									orderReadyRequest));

			if ((bool)results[ACK])
				SimulatorDashboard.Singleton.LogAsync($"Ok, SERVER KNOWS that ref {_orderReference} is READY.");
			else
				SimulatorDashboard.Singleton.LogAsync($"I COULD NOT TELL the server that {_orderReference} is ready.");

			_orderReference = 0;
			_recipe = null;
		}

		private void ReportProblemDuringOrderProcessing()
		{
			SimulatorDashboard.Singleton.LogAsync($"I will warn the server that a problem ocurred.");

			var problemOcurredRequest = new OrderRequest()
			{
				OrderMessage = (int)OrderMessageEnum.ProblemOcurredDuringProcessing,
				OrderReference = _orderReference,
				Mac = SimulatorAppConfig.Singleton.SimulatorMac
			};

			var results = TryManyTimesIfNack(() => SendRequestToServer<OrderRequest, OrderResponse>("Problem ocurred during processing",
																									SimulatorAppConfig.Singleton.OrderUrl,
																									problemOcurredRequest));

			if ((bool)results[ACK])
				SimulatorDashboard.Singleton.LogAsync($"Ok, server knows that a problem ocurred.");
			else
			{
				SimulatorDashboard.Singleton.LogAsync($"I COULD NOT tell the server that a problem ocurred and I WILL DISABLE.");
				_securityDisable = true;
			}

			_orderReference = 0;
			_recipe = null;
		}

		#endregion Order
	}
}