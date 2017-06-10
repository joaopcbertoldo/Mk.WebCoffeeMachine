using Microsoft.Practices.Unity;
using Mkafeina.Domain;
using Mkafeina.Domain.ArduinoApi;
using Mkafeina.Domain.Dashboard;
using Mkafeina.Domain.ServerArduinoComm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mkafeina.Server.Domain.CoffeeMachineProxy
{
	public enum RegistrationStatusEnum
	{
		Undefined = 0,
		NotRegistered,
		RegistrationNotAccepted,
		Registered
	}

	public class CMProxyHub : IProxyEventObserver
	{
		

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

		public string GetMac(string uniqueName)
			=> _proxies.Any(kv => kv.Value.Info.UniqueName == uniqueName) ?
					_proxies.First(kv => kv.Value.Info.UniqueName == uniqueName).Value.Info.Mac :
					null;

		public CMProxy GetProxy(string mac) => _proxies.ContainsKey(mac) ? _proxies[mac] : null;

		public RegistrationResponse HandleRegistration(RegistrationRequest request)
		{
			lock (_proxies)
			{
				if (_proxies.ContainsKey(request.Mac))
					return _ardResponseFac.InvalidRequest<RegistrationResponse>(ErrorEnum.MacAlreadyRegistered);

				var proxy = CreateProxy(request);
				_proxies.Add(request.Mac, proxy);
				return _ardResponseFac.RegistrationOK(CommandEnum.Enable);
			}
		}

		private CMProxy CreateProxy(RegistrationRequest request)
		{
			var uniqueName = request.UniqueName;
			while (_proxies.Any(kv => kv.Value.Info.UniqueName == request.UniqueName))
				uniqueName = uniqueName.GenerateNameVersion();

			var proxy = CMProxy.CreateCMProxy(request.Mac, uniqueName, request.IngredientsSetup);

			return proxy;
		}

		internal void Unregister(string mac)
		{
			lock (this)
			{
				if (!_proxies.ContainsKey(mac))
					return;
				var dash = AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractDashboard>();
				var proxy = GetProxy(mac);
#warning remover dynamic panel
				//dash.DeleteDynamicPanel(proxy.Info.UniqueName);
				_proxies.Remove(mac);
			}
		}

		public void Notify(ProxyEventEnum action)
		{
			return;
		}
	}
}