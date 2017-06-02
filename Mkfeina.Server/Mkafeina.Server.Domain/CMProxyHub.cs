using Mkafeina.Domain;
using Mkafeina.Domain.ServerArduinoComm;
using Mkafeina.Server.Domain;
using Mkfeina.Domain.ServerArduinoComm;
using System.Collections.Generic;
using System.Linq;

namespace Mkfeina.Server.Domain
{
	public class CMProxyHub
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

		private Dictionary<string, CMProxy> _proxies = new Dictionary<string, CMProxy>();

		private ArduinoResponseFactory _ardResponseFac = new ArduinoResponseFactory();

		public RegistrationStatusEnum RegistrationStatus(string mac)
		{
			if (!_proxies.ContainsKey(mac))
				return RegistrationStatusEnum.NotRegistered;
			else if (!_proxies[mac].State.RegistrationIsAccepted)
				return RegistrationStatusEnum.RegistrationNotAccepted;
			else
				return RegistrationStatusEnum.Registered;
		}

		public bool IsRegistered(string mac)
			=> RegistrationStatus(mac) == RegistrationStatusEnum.Registered;

		public bool IsRegisteredByUniqueName(string uniqueName)
			=> _proxies.Any(kv => kv.Value.State.UniqueName == uniqueName) &&
			   _proxies.First(kv => kv.Value.State.UniqueName == uniqueName).Value.State.RegistrationIsAccepted;

		public CMProxy GetProxy(string mac)
		{
			if (!_proxies.ContainsKey(mac))
				return null;
			return _proxies[mac];
		}

		public CMProxy GetProxyByUniqueName(string uniqueName)
			=> _proxies.FirstOrDefault(kv => kv.Value.State.UniqueName == uniqueName).Value;

		public RegistrationResponse HandleRegistrationRequest(RegistrationRequest request)
		{
			switch ((RegistrationMessageEnum)request.RegistrationMessage)
			{
				case RegistrationMessageEnum.AttemptRegistration:
					return HandleRegistrationAttempt(request);

				case RegistrationMessageEnum.RegistrationAcceptance:
					return HandleRegistrationAcceptance(request);

				case RegistrationMessageEnum.Offsets:
#warning fazer Offsets
					return _ardResponseFac.RegistrationInvalidRequest();

				case RegistrationMessageEnum.Unregister:
#warning fazer Unregister
					return _ardResponseFac.RegistrationInvalidRequest();

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
					return _ardResponseFac.RegistrationAttemptWithMacAlreadyExisting(alreadyRegistered: _proxies[mac].State.RegistrationIsAccepted);

				string trueUniqueName = request.UniqueName;
				while (_proxies.Any(kv => kv.Value.State.UniqueName == request.UniqueName))
					trueUniqueName = trueUniqueName.GenerateNameVersion();

				var newProxy = CMProxy.CreateNew(trueUniqueName, request);

				_proxies.Add(mac, newProxy);

				return _ardResponseFac.RegistrationAttemptOK(trueUniqueName);
			}
		}

		public RegistrationResponse HandleRegistrationAcceptance(RegistrationRequest request)
		{
			lock (_proxies)
			{
				if (_proxies.ContainsKey(request.Mac))
				{
					if (_proxies[request.Mac].State.RegistrationIsAccepted)
						return _ardResponseFac.RegistrationAcceptanceButIsAlreadyAccepted();
					else
					{
						_proxies[request.Mac].State.RegistrationIsAccepted = true;
						return _ardResponseFac.RegistrationAcceptanceOK();
					}
				}
				else
					return _ardResponseFac.RegistrationInvalidRequest();
			}
		}
	}
}