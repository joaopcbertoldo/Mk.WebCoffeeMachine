using Mkafeina.Domain;
using System.Linq;
using System.Net.NetworkInformation;

namespace Mkafeina.Simulator
{
	public class AppConfig : AbstractAppConfig
	{
		public const string
			SERVER_API_URL = "server.apiUrl",

			FAKE_COFFEE_MACHINE = "fakeCoffeeMachine",
			CONFIGS = "configs",
			COMMANDS = "commands",
			LOG = "log",

			SIMULATOR_UNIQUE_NAME = "simulator.uniqueName",
			SIMULATOR_ADDRESS = "simulator.address",
			INGREDIENT_ADDITION_DELAY_MS = "simulator.ingredientAdditionDelayMs",

			STANDARD_TIMEOUT = "standardTimeout"
			;

		public string SimulatorUniqueName { get => _cache[SIMULATOR_UNIQUE_NAME]; }

		public string SimulatorMac {
			get => (
					   from nic in NetworkInterface.GetAllNetworkInterfaces()
					   where nic.OperationalStatus == OperationalStatus.Up
					   select nic.GetPhysicalAddress().ToString()
				   ).FirstOrDefault();
		}

		public string SimulatorAddress { get => _cache[SIMULATOR_ADDRESS]; }

		private int _ingredientAdditionDelayMs = -1;

		public int IngredientAdditionDelayMs {
			get {
				if (_ingredientAdditionDelayMs < 0)
					_ingredientAdditionDelayMs = _cache[INGREDIENT_ADDITION_DELAY_MS].ParseToInt();
				return _ingredientAdditionDelayMs;
			}
			set {
				_ingredientAdditionDelayMs = value;
				OnConfigChangeEvent(INGREDIENT_ADDITION_DELAY_MS);
			}
		}

		public int StandardTimeout { get => _cache[STANDARD_TIMEOUT].ParseToInt(); }

		public string ServerApiUrl { get => _cache[SERVER_API_URL]; }
	}
}