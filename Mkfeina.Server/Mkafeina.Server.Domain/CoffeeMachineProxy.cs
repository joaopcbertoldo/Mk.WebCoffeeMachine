using Mkfeina.Domain;
using Mkfeina.Domain.ServerArduinoComm;
using System.Collections.Generic;
using System.Linq;

namespace Mkfeina.Server.Domain
{
	public class CoffeeMachineProxy : IObservable<CoffeeMachineProxy>
	{
		#region Static stuff

		private static Dictionary<string, CoffeeMachineProxy> _coffeeMachines = new Dictionary<string, CoffeeMachineProxy>();

		public static bool IsRegistered(string mac) => _coffeeMachines.ContainsKey(mac);

		public static CoffeeMachineProxy GetProxy(string mac)
		{
			if (_coffeeMachines.ContainsKey(mac))
				return null;
			return _coffeeMachines[mac];
		}

		public static RegistrationResponse HandleRegistrationAttempt(RegistrationRequest request)
		{
			lock (_coffeeMachines)
			{
				var mac = request.Mac;
				if (_coffeeMachines.ContainsKey(mac))
					return new RegistrationResponse()
					{
						ResponseCode = _coffeeMachines[mac].RegistrationIsAccepted ? (int)RegistrationResponseCodeEnum.AlreadyRegistered :
																					 (int)RegistrationResponseCodeEnum.RegisteredButNotAccepted
					};

				string trueUniqueName = request.UniqueName;
				while (_coffeeMachines.Any(kv => kv.Value.UniqueName == request.UniqueName))
					trueUniqueName = trueUniqueName.GenerateNameVersion();

				var newProxy = new CoffeeMachineProxy()
				{
					Mac = request.Mac,
					UniqueName = trueUniqueName,
					CoffeeEmptyOffset = request.CoffeeEmptyOffset,
					CoffeeFullOffset = request.CoffeeFullOffset,
					WaterEmptyOffset = request.WaterEmptyOffset,
					WaterFullOffset = request.WaterFullOffset,
					MilkEmptyOffset = request.MilkEmptyOffset,
					MilkFullOffset = request.MilkFullOffset,
					SugarEmptyOffset = request.SugarEmptyOffset,
					SugarFullOffset = request.SugarFullOffset,
					CoffeeLevel = 0,
					WaterLevel = 0,
					MilkLevel = 0,
					SugarLevel = 0,
					RegistrationIsAccepted = false,
					IsMakingCoffee = false,
					IsEnabled = false
				};

				_coffeeMachines.Add(mac, newProxy);

				return new RegistrationResponse()
				{
					ResponseCode = (int)ResponseCodeEnum.OK,
					TrueUniqueName = trueUniqueName
				};
			}
		}

		public static RegistrationResponse HandleRegistrationAcceptance(RegistrationRequest request)
		{
			lock (_coffeeMachines)
			{
				if (_coffeeMachines.ContainsKey(request.Mac) && !_coffeeMachines[request.Mac].RegistrationIsAccepted)
					return new RegistrationResponse()
					{
						ResponseCode = (int)ResponseCodeEnum.OK
					};
				else if (_coffeeMachines.ContainsKey(request.Mac) && _coffeeMachines[request.Mac].RegistrationIsAccepted)
					return new RegistrationResponse()
					{
						ResponseCode = (int)RegistrationResponseCodeEnum.AlreadyRegistered
					};
				else
					return new RegistrationResponse()
					{
						ResponseCode = (int)ResponseCodeEnum.InvalidRequest
					};
			}
		}

		#endregion Static stuff

		protected Dictionary<string, string> _recipes;

		protected List<IObserver<CoffeeMachineProxy>> _observers;

		private string _uniqueName;

		public string UniqueName {
			get { return _uniqueName; }
			set { _uniqueName = value; }
		}

		private bool _isMakingCoffee;

		public bool IsMakingCoffee {
			get { return _isMakingCoffee; }
			set { _isMakingCoffee = value; }
		}

		private float _coffeeLevel; // voltage

		public int CoffeeLevel { // %
			get {
				// convert voltage to %
				return (int)_coffeeLevel;
			}
			set {
				_coffeeLevel = value;
			}
		}

		private float _waterLevel; // voltage

		public int WaterLevel { // %
			get {
				// convert voltage to %
				return (int)_waterLevel;
			}
			set {
				_waterLevel = value;
			}
		}

		private float _milkLevel; // voltage

		public int MilkLevel { // %
			get {
				// convert voltage to %
				return (int)_milkLevel;
			}
			set {
				_milkLevel = value;
			}
		}

		private float _sugarLevel; // voltage

		public int SugarLevel { // %
			get {
				// convert voltage to %
				return (int)_sugarLevel;
			}
			set {
				_sugarLevel = value;
			}
		}

		private bool _registrationIsAccepted;

		public bool RegistrationIsAccepted {
			get { return _registrationIsAccepted; }
			set { _registrationIsAccepted = value; }
		}

		private bool _isEnabled;

		public bool IsEnabled {
			get { return _isEnabled; }
			set { _isEnabled = value; }
		}

		public string Mac { get; set; }

		public int CoffeeEmptyOffset { get; set; }

		public int CoffeeFullOffset { get; set; }

		public int WaterEmptyOffset { get; set; }

		public int WaterFullOffset { get; set; }

		public int MilkEmptyOffset { get; set; }

		public int MilkFullOffset { get; set; }

		public int SugarEmptyOffset { get; set; }

		public int SugarFullOffset { get; set; }

		public MakeCoffeeResponseEnum MakeCoffee(string recipe)
		{
			return MakeCoffeeResponseEnum.Undefined;
		}

		public void RegisterObserver(IObserver<CoffeeMachineProxy> newObserver)
		{
			if (newObserver != null)
				_observers.Add(newObserver);
		}

		public void NotifyObservers()
		{
			foreach (var observer in _observers)
				observer.Notify(this);
		}
	}
}