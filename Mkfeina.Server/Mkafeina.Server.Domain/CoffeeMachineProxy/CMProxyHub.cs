using Mkafeina.Domain;
using Mkafeina.Domain.ServerArduinoComm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mkafeina.Server.Domain.CoffeeMachineProxy
{
	public class CMProxyHub : IProxyEventObserver
	{
		public const string
			NEXT = "CMProxyHub.next",
			PREVIOUS = "CMProxyHub.previous",
			SELECTED = "CMProxyHub.selected",
			DISABLE_SELECTED = "CMProxyHub.disableSelected",
			UNREGISTER_SELECTED = "CMProxyHub.unregisterSelected"
			;

		private int _selectedCMIndex = 0;

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

		public event Action<string, object> ChangeEvent;

		internal void OnChangeEvent(string lineName) => ChangeEvent?.Invoke(lineName, this);

		public string GetMac(string uniqueName)
			=> _proxies.Any(kv => kv.Value.Info.UniqueName == uniqueName) ?
					_proxies.First(kv => kv.Value.Info.UniqueName == uniqueName).Value.Info.Mac :
					null;

		internal void ReloadRecipesOnProxies()
		{
			lock (_proxies)
			{
				foreach (var p in _proxies)
					p.Value.Cookbook.GetRecipesFromMainCookbook();
			}
		}

		public CMProxy GetProxy(string mac) => _proxies.ContainsKey(mac) ? _proxies[mac] : null;

		public RegistrationResponse HandleRegistration(RegistrationRequest request)
		{
			lock (_proxies)
			{
				if (_proxies.ContainsKey(request.mac))
				{
					Dashboard.Sgt.LogAsync($"Registration request for mac <<{request.mac}>> REJECTED (already registered).");
					return _ardResponseFac.InvalidRequest<RegistrationResponse>(ErrorEnum.MacAlreadyRegistered);
				}
				else
				{
					while (_proxies.Any(kv => kv.Value.Info.UniqueName == request.un))
						request.un = request.un.GenerateNameVersion();
					var proxy = new CMProxy(request);
					_proxies.Add(request.mac, proxy);
					Dashboard.Sgt.LogAsync($"New Coffee Machine! Name: {request.un}.");
					return _ardResponseFac.RegistrationOK(CommandEnum.Enable);
				}
			}
		}

		internal void Unregister(string mac)
		{
			lock (_proxies)
			{
				if (!_proxies.ContainsKey(mac))
					return;
				else
				{
					var proxy = GetProxy(mac);
					var uniqueName = proxy.Info.UniqueName;
					Waitress.DeleteWaitress(mac);
					_proxies.Remove(mac);
					_selectedCMIndex = 0;
					Dashboard.Sgt.DeleteDynamicPanel(uniqueName);
					Dashboard.Sgt.LogAsync($"Coffee machine {uniqueName} has been unregistered.");
				}
			}
		}

		public void Notify(ProxyEventEnum action)
		{
#warning do something here ????????????????
			return;
		}

		public void NextCM()
		{
			_selectedCMIndex = _selectedCMIndex + 1 >= _proxies.Count ? _proxies.Count - 1 : _selectedCMIndex + 1;
			OnChangeEvent(SELECTED);
		}

		public void PreviousCM()
		{
			_selectedCMIndex = _selectedCMIndex <= 0 ? 0 : _selectedCMIndex - 1;
			OnChangeEvent(SELECTED);
		}

		public string SelectedCM {
			get => _proxies.ElementAtOrDefault(_selectedCMIndex).Equals(default(KeyValuePair<string, CMProxy>)) ?
						   "---" : _proxies.ElementAtOrDefault(_selectedCMIndex).Value.Info.UniqueName;
		}

		public void DisableSelectedCM()
		{
			var proxy = _proxies.ElementAtOrDefault(_selectedCMIndex);
			if (proxy.Equals(default(KeyValuePair<string, CMProxy>)))
				return;
			proxy.Value.SetDisableFlag();
		}

	}
}