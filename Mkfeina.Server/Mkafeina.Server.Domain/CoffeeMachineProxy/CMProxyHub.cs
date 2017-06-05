using Microsoft.Practices.Unity;
using Mkafeina.Domain;
using Mkafeina.Domain.Dashboard;
using Mkafeina.Domain.ServerArduinoComm;
using Mkafeina.Server.Domain.CoffeeMachineProxy;
using Mkafeina.Server.Domain.Entities;
using Mkafeina.Domain.ArduinoApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mkafeina.Server.Domain.CoffeeMachineProxy
{
	public enum RegistrationStatusEnum
	{
		Undefined = 0,
		NotRegistered,
		RegistrationNotAccepted,
		Registered
	}

	public class CMProxyHub
	{
		private const string
			COFFE_MACHINE = "cm";

		#region Singleton Stuff

		private static CMProxyHub __sgt;

		public static CMProxyHub Sgt {
			get {
				if (__sgt == null)
					__sgt = new CMProxyHub();
				return __sgt;
			}
		}

		private CMProxyHub()
		{
		}

		#endregion Singleton Stuff

		private string CreateProxy(RegistrationRequest request)
		{
			var uniqueName = request.UniqueName;
			while (_proxies.Any(kv => kv.Value.State.UniqueName == request.UniqueName))
				uniqueName = uniqueName.GenerateNameVersion();

			var proxy = CMProxy.CreateCMProxy(request.Mac,uniqueName,request.IngredientsSetup);

			var mac = request.Mac;
			_proxies.Add(mac, proxy);

			var task = Task.Factory.StartNew(() =>
			{
#warning mover Dash para este namespace para colocar isso dentro do dash
				var dash = AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractDashboard>();
				var appconfig = AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractAppConfig>();
				dash.LogAsync($"New Coffee Machine! Name: {uniqueName}.");
				var config = appconfig.PanelConfigs(COFFE_MACHINE);
				config.Title = uniqueName;
				dash.CreateDynamicPanel(uniqueName, config);
				dash.AddFixedLinesToDynamicPanel(uniqueName, appconfig.PanelFixedLines(COFFE_MACHINE));
				proxy.ChangeEvent += dash.UpdateEventHandlerOfPanel(uniqueName);
			});

			return uniqueName;
		}

		private ArduinoResponseFactory _ardResponseFac = new ArduinoResponseFactory();

		private Dictionary<string, CMProxy> _proxies = new Dictionary<string, CMProxy>();

		public RegistrationStatusEnum RegistrationStatus(string mac)
			=> !_proxies.ContainsKey(mac) ? RegistrationStatusEnum.NotRegistered :
				_proxies[mac].State.RegistrationIsAccepted ? RegistrationStatusEnum.Registered :
				  											 RegistrationStatusEnum.RegistrationNotAccepted;

		public bool IsRegistered(string mac)
			=> RegistrationStatus(mac) == RegistrationStatusEnum.Registered;

		public bool IsRegisteredByUniqueName(string uniqueName)
			=> _proxies.Any(kv => kv.Value.State.UniqueName == uniqueName) &&
			   _proxies.First(kv => kv.Value.State.UniqueName == uniqueName).Value.State.RegistrationIsAccepted;

		public CMProxy GetProxy(string mac)
			=> _proxies.ContainsKey(mac) ? _proxies[mac] : null;

		public string GetMac(string uniqueName)
			=> _proxies.Any(kv => kv.Value.State.UniqueName == uniqueName) ? 
						_proxies.First(kv => kv.Value.State.UniqueName == uniqueName).Value.State.Mac :
						null;

		public RegistrationResponse HandleRegistrationRequest(RegistrationRequest request)
		{
			switch ((RegistrationMessageEnum)request.RegistrationMessage)
			{
				case RegistrationMessageEnum.AttemptRegistration:
					return HandleRegistrationAttempt(request);

				case RegistrationMessageEnum.RegistrationAcceptance:
					return HandleRegistrationAcceptance(request);

				case RegistrationMessageEnum.Offsets:
					return HandleRegistrationOffsets(request);

				case RegistrationMessageEnum.Unregister:
					return HandleRegistrationUnregister(request);

				default:
					return _ardResponseFac.RegistrationInvalidRequest();
			}
		}

		public RegistrationResponse HandleRegistrationAttempt(RegistrationRequest request)
		{
			lock (_proxies)
			{
				var mac = request.Mac;
				if (_proxies.ContainsKey(mac))
					return _ardResponseFac.RegistrationAttemptWithMacAlreadyExisting(alreadyAccepted: _proxies[mac].State.RegistrationIsAccepted);

				var trueUniqueName = CreateProxy(request);
				return _ardResponseFac.RegistrationOK(trueUniqueName);
			}
		}

		public RegistrationResponse HandleRegistrationAcceptance(RegistrationRequest request)
		{
			lock (_proxies)
			{
				if (!_proxies.ContainsKey(request.Mac))
					return _ardResponseFac.RegistrationInvalidRequest();
			}

			var proxy = _proxies[request.Mac];

			lock (proxy)
			{
				if (proxy.State.RegistrationIsAccepted)
					return _ardResponseFac.RegistrationAcceptanceButIsAlreadyAccepted();
				else
				{
					proxy.State.RegistrationIsAccepted = true;

					var task = Task.Factory.StartNew(() =>
					{
						var dash = AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractDashboard>();
						dash.LogAsync($"{proxy.State.UniqueName} registration accepted.");
					});

					return _ardResponseFac.RegistrationOK();
				}
			}
		}

		private RegistrationResponse HandleRegistrationOffsets(RegistrationRequest request)
		{
			lock (_proxies)
			{
				if (!_proxies.ContainsKey(request.Mac))
					return _ardResponseFac.RegistrationInvalidRequest();
			}

			var proxy = _proxies[request.Mac];

			lock (proxy)
			{
				proxy.State.SetupAvaiabilityAndOffsets(request.IngredientsSetup);

				var task = Task.Factory.StartNew(() =>
				{
					var dash = AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractDashboard>();
					dash.LogAsync($"{proxy.State.UniqueName}'s offsets have been reseted.");
				});

				return _ardResponseFac.RegistrationOK();
			}
		}

		private RegistrationResponse HandleRegistrationUnregister(RegistrationRequest request)
		{
			lock (_proxies)
			{
				if (!_proxies.ContainsKey(request.Mac))
					return _ardResponseFac.RegistrationInvalidRequest();

				var name = _proxies[request.Mac].State.UniqueName;
				_proxies.Remove(request.Mac);

				var task = Task.Factory.StartNew(() =>
				{
					var dash = AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractDashboard>();
					dash.LogAsync($"{name} have been unregistered :(.");
				});

				return _ardResponseFac.RegistrationOK();
			}
		}

		public ReportResponse HandleReportRequest(ReportRequest request)
		{
			var mac = request.Mac;
			if (!IsRegistered(mac))
				return _ardResponseFac.ReportInvalidRequest();
			var proxy = GetProxy(mac);
			return proxy.HandleReportRequest(request);
		}

		public OrderResponse HandleOrderRequest(OrderRequest request)
		{
			var mac = request.Mac;
			if (!IsRegistered(mac))
				return _ardResponseFac.OrderInvalidRequest();
			var proxy = GetProxy(mac);
			return proxy.HandleOrderRequest(request);
		}
	}
}