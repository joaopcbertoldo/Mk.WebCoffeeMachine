using Microsoft.Practices.Unity;
using Mkafeina.Domain;
using Mkafeina.Domain.ArduinoApi;
using Mkafeina.Domain.Dashboard;
using Mkafeina.Domain.ServerArduinoComm;
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

		#region Internal Stuff

		private ArduinoResponseFactory _ardResponseFac = new ArduinoResponseFactory();

		private Dictionary<string, CMProxy> _proxies = new Dictionary<string, CMProxy>();

		#endregion Internal Stuff

		public RegistrationStatusEnum RegistrationStatus(string mac)
			=> !_proxies.ContainsKey(mac) ? RegistrationStatusEnum.NotRegistered :
				_proxies[mac].Info.RegistrationIsAccepted ? RegistrationStatusEnum.Registered :
				  											RegistrationStatusEnum.RegistrationNotAccepted;

		public string GetMac(string uniqueName)
			=> _proxies.Any(kv => kv.Value.Info.UniqueName == uniqueName) ?
					_proxies.First(kv => kv.Value.Info.UniqueName == uniqueName).Value.Info.Mac :
					null;

		public CMProxy GetProxy(string mac) => _proxies.ContainsKey(mac) ? _proxies[mac] : null;

		public RegistrationResponse HandleRegistrationAttempt(RegistrationRequest request)
		{
			lock (_proxies)
			{
				var mac = request.Mac;
				if (_proxies.ContainsKey(mac))
					return _ardResponseFac.RegistrationAttemptWithMacAlreadyExisting(alreadyAccepted: _proxies[mac].Info.RegistrationIsAccepted);

				var proxy = CreateProxy(request);
				_proxies.Add(mac, proxy);
				return _ardResponseFac.RegistrationOK(proxy.Info.UniqueName);
			}
		}

		private CMProxy CreateProxy(RegistrationRequest request)
		{
			var uniqueName = request.UniqueName;
			while (_proxies.Any(kv => kv.Value.Info.UniqueName == request.UniqueName))
				uniqueName = uniqueName.GenerateNameVersion();

			var proxy = CMProxy.CreateCMProxy(request.Mac, uniqueName, request.IngredientsSetup);

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

			return proxy;
		}
	}
}