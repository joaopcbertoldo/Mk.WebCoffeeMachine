using Mkafeina.Domain.ServerArduinoComm;
using System;

namespace Mkafeina.Server.Domain
{
	public class CMProxyState
	{
		private string _uniqueName;
		private bool _isMakingCoffee;
		private int _coffeeLevel;
		private int _waterLevel;
		private int _milkLevel;
		private int _sugarLevel;
		private bool _registrationIsAccepted;
		private bool _isEnabled;

		public event Action StateChangeEvent;

		public string UniqueName {
			get => _uniqueName;
			set {
				_uniqueName = value;
				StateChangeEvent?.Invoke();
			}
		}

		public bool IsMakingCoffee {
			get => _isMakingCoffee;
			set {
				_isMakingCoffee = value;
				StateChangeEvent?.Invoke();
			}
		}

		public int CoffeeLevel {
			get => _coffeeLevel;
			set {
				_coffeeLevel = value;
				StateChangeEvent?.Invoke();
			}
		}

		public int WaterLevel {
			get => _waterLevel;
			set {
				_waterLevel = value;
				StateChangeEvent?.Invoke();
			}
		}

		public int MilkLevel {
			get => _milkLevel;
			set {
				_milkLevel = value;
				StateChangeEvent?.Invoke();
			}
		}

		public int SugarLevel {
			get => _sugarLevel;
			set {
				_sugarLevel = value;
				StateChangeEvent?.Invoke();
			}
		}

		public bool RegistrationIsAccepted {
			get => _registrationIsAccepted;
			set {
				_registrationIsAccepted = value;
				StateChangeEvent?.Invoke();
			}
		}

		public bool IsEnabled {
			get => _isEnabled;
			set {
				_isEnabled = value;
				StateChangeEvent?.Invoke();
			}
		}

		public string Mac { get; set; }

		public bool LevelsUnderMinimum()
		{
#warning parametrizar minimos
			return WaterLevel <= 10 || CoffeeLevel <= 10 || SugarLevel <= 10 || MilkLevel <= 10;
		}

		public void Update(ReportRequest request, CMProxyOffsets offsets)
		{
			CoffeeLevel = offsets.AdjustSignal(request.CoffeeLevel, "Coffee");
			WaterLevel = offsets.AdjustSignal(request.WaterLevel, "Water");
			SugarLevel = offsets.AdjustSignal(request.SugarLevel, "Sugar");
			MilkLevel = offsets.AdjustSignal(request.MilkLevel, "Milk");
			IsEnabled = request.IsEnabled;
		}
	}
}