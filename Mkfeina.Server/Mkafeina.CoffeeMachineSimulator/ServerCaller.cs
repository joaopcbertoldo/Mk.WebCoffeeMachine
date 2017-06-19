using Microsoft.Practices.Unity;
using Mkafeina.Domain;
using Mkafeina.Domain.ArduinoApi;
using Mkafeina.Domain.ServerArduinoComm;
using Mkafeina.Simulator;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Net;

namespace Mkafeina.CoffeeMachineSimulator
{
	public class ServerCaller
	{
		private const string
			REGISTRATION_ROUTE = "/registration",
			REPORT_ROUTE = "/report",
			ORDER_ROUTE = "/order"
			;

		private const string
			APPLICATION_JSON = "application/json",
			POST = "POST"
			;

		private const int
			STANDARD_TIMEOUT = 30000
			;

		private object _communicationSyncOnj = new object();
		private string _serverApiUrl;
		private ArduinoRequestFactory _ardRequestFac;
		private FakeCoffeMachine _fakeCoffeMachine;
		private AppConfig _appconfig;

		public ServerCaller(FakeCoffeMachine fakeCoffeMachine)
		{
			_appconfig = (AppConfig)AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractAppConfig>();
			_serverApiUrl = _appconfig.ServerApiUrl;
			_fakeCoffeMachine = fakeCoffeMachine;
			_ardRequestFac = new ArduinoRequestFactory(fakeCoffeMachine);
		}

		private TResponse SendHttpRequest<TRequest, TResponse>(TRequest requestObj, string url)
			where TRequest : ArduinoRequest
			where TResponse : ArduinoResponse

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
					request.Method = POST;
					request.ContentType = APPLICATION_JSON;
					request.Timeout = _appconfig.StandardTimeout;

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
							Dashboard.Sgt.LogAsync("Problem ocurred during communication (on write stream phase).");
							return null;
						}
					}

					TResponse responseObj;
					var response = (HttpWebResponse)request.GetResponse();
					using (var streamReader = new StreamReader(response.GetResponseStream()))
					{
						var responseStr = streamReader.ReadToEnd();
						responseObj = JObject.Parse(responseStr).ToObject<TResponse>();
					}
					return responseObj;
				}
				catch (WebException exception)
				{
					Dashboard.Sgt.LogAsync($"Web exception during communication. Status <<{exception.Status.ToString()}>>.");
					return null;
				}
				catch (Exception exception)
				{
					Dashboard.Sgt.LogAsync("Problem ocurred during communication (exception thrown).");
					return null;
				}
			}
		}

		public bool Send<TRequest, TResponse>(TRequest request, out TResponse response, string url)
			where TRequest : ArduinoRequest
			where TResponse : ArduinoResponse
		{
			try
			{
				request.mac = _appconfig.SimulatorMac;
				response = SendHttpRequest<TRequest, TResponse>(request, url);

				if (response == null)
				{
					Dashboard.Sgt.LogAsync($"{request.msg.ToString()} >> NULL RESPONSE.");
					return false;
				}
				else
				{
					Dashboard.Sgt.LogAsync($"{request.msg.ToString()} >> RC={response.rc.ToString()} COMM={response.c.ToString()} ERR={response.e.ToString()}.");
					return true;
				}
			}
			catch (Exception exception)
			{
				Dashboard.Sgt.LogAsync($"{request.msg.ToString()} >> EXCEPTION THROWN.");
				response = null;
				return false;
			}
		}

		public CommandEnum RegisterNoMatterWhat()
		{
			RegistrationResponse response;
			do
			{
				Dashboard.Sgt.LogAsync($"I will try to register.");
				var registrationRequest = _ardRequestFac.Registration();
				var ack = Send(registrationRequest, out response, _serverApiUrl + REGISTRATION_ROUTE);
				if (!ack)
					continue;

				if (response.rc == ResponseCodeEnum.OK || response.e == ErrorEnum.MacAlreadyRegistered)
				{
					_fakeCoffeMachine.Signals.Registered = true;
					_fakeCoffeMachine.Signals.Enabled = true;
				}
			} while (!_fakeCoffeMachine.Signals.Registered);

			return response.c;
		}

		public bool TryToSendNewOffsets(out CommandEnum command)
		{
			Dashboard.Sgt.LogAsync($"I will send new offsets to the server.");
			try
			{
				var rand = new Random((int)DateTime.Now.Millisecond);

				_fakeCoffeMachine.Signals.CoffeeMin = (float)rand.NextDouble();
				_fakeCoffeMachine.Signals.CoffeeMax = (float)rand.NextDouble() + 4;
				_fakeCoffeMachine.Signals.SugarMin = (float)rand.NextDouble();
				_fakeCoffeMachine.Signals.SugarMax = (float)rand.NextDouble() + 4;
				_fakeCoffeMachine.Signals.WaterMin = (float)rand.NextDouble();
				_fakeCoffeMachine.Signals.WaterMax = (float)rand.NextDouble() + 4;

				var request = _ardRequestFac.Offsets();
				RegistrationResponse response;
				var ack = Send(request, out response, _serverApiUrl + REGISTRATION_ROUTE);
				command = ack ? response.c : CommandEnum.Undef;
				if (ack)
				{
					Dashboard.Sgt.LogAsync($"Offsets have been reset on the server.");
					return true;
				}
				else
					return false;
			}
			catch (Exception ex)
			{
				command = CommandEnum.Undef;
				return false;
			}
		}

		public bool TryToUnregister()
		{
			Dashboard.Sgt.LogAsync($"I will try to UNregister.");
			var unregistrationRequest = _ardRequestFac.Unregistration();
			RegistrationResponse response;
			var ack = Send(unregistrationRequest, out response, _serverApiUrl + REGISTRATION_ROUTE);
			if (ack && response.c == CommandEnum.Unregister)
			{
				Dashboard.Sgt.LogAsync($"UNregistration of <<{_fakeCoffeMachine.UniqueName}>> SUCCESSFUL.");
			}
			return ack && response.c == CommandEnum.Unregister;
		}

		public bool TryToReportSignals(out CommandEnum command)
		{
			ReportResponse response;
			Dashboard.Sgt.LogAsync($"Reporting levels ({DateTime.Now.ToString()}).");
			var levelsReportRequest = _ardRequestFac.Signals();
			var ack = Send(levelsReportRequest, out response, _serverApiUrl + REPORT_ROUTE);
			command = ack ? response.c : CommandEnum.Undef;
			return ack;
		}

		public bool TryToDisable(out CommandEnum command)
		{
			Dashboard.Sgt.LogAsync($"I will warn the server that i'm disabling.");
			var disablingRequest = _ardRequestFac.Disabling();
			ReportResponse response;
			var ack = Send(disablingRequest, out response, _serverApiUrl + REPORT_ROUTE);
			command = response?.c ?? CommandEnum.Undef;
			return ack;
		}

		public CommandEnum ReenableNoMatterWhat()
		{
			CommandEnum command;
			do
			{
				Dashboard.Sgt.LogAsync($"I will reenable!");
				var reenablingRequest = _ardRequestFac.Reenable();
				ReportResponse response;
				var ack = Send(reenablingRequest, out response, _serverApiUrl + REPORT_ROUTE);
				command = response?.c ?? CommandEnum.Undef;
				if (ack && command == CommandEnum.Enable)
					_fakeCoffeMachine.Signals.Enabled = true;
			} while (!_fakeCoffeMachine.Signals.Enabled);
			return command;
		}

		// OLD VERSION
		//public bool TryToTakeOrder(out CommandEnum command, out string orderRef, out string recipe)
		public bool TryToTakeOrder(out CommandEnum command, out string orderRef, out RecipeObj recipe)
		{
			Dashboard.Sgt.LogAsync($"I will ask for an order ({DateTime.Now.ToString()}).");
			var giveMeOrderRequest = _ardRequestFac.GiveMeAnOrder();
			OrderResponse response;
			var ack = Send(giveMeOrderRequest, out response, _serverApiUrl + ORDER_ROUTE);
			orderRef = response?.oref;
			recipe = response?.rec;
			command = response?.c ?? CommandEnum.Undef;
			if (orderRef != null)
				Dashboard.Sgt.LogAsync($"ORDER RECEIVED!!!! ref: {orderRef}.");
			return ack;
		}

		public bool TryReady(out CommandEnum command)
		{
			Dashboard.Sgt.LogAsync($"I will tell the server that ref {_fakeCoffeMachine._orderRef} is READY.");
			var orderReadyRequest = _ardRequestFac.OrderReady(_fakeCoffeMachine._orderRef);
			OrderResponse response;
			var ack = Send(orderReadyRequest, out response, _serverApiUrl + ORDER_ROUTE);
			command = response?.c ?? CommandEnum.Undef;
			return ack;
		}

		public bool TryCancelOrders(out CommandEnum command)
		{
			Dashboard.Sgt.LogAsync($"I will cancel all the orders.");
			var orderReadyRequest = _ardRequestFac.CancelOrders();
			OrderResponse response;
			var ack = Send(orderReadyRequest, out response, _serverApiUrl + ORDER_ROUTE);
			command = response?.c ?? CommandEnum.Undef;
			return ack;
		}
	}
}