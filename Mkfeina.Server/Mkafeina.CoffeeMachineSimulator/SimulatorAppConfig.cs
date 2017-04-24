using Mkfeina.Domain;
using Mkfeina.Domain.Panels;
using System.Linq;
using System.Net.NetworkInformation;
using static Mkfeina.Simulator.Constants;

namespace Mkfeina.Simulator
{
	public class SimulatorAppConfig : AppConfig
	{
		private const string
			STATUS = "status",
			CONFIGS = "configs",
			COMMANDS = "commands",
			LOG = "log";

		#region Sinlgeton Stuff

		private SimulatorAppConfig() : base()
		{
		}

		private static SimulatorAppConfig __singleton;

		public static SimulatorAppConfig Singleton {
			get {
				if (__singleton == null)
					__singleton = new SimulatorAppConfig();
				return __singleton;
			}
		}

		#endregion Sinlgeton Stuff

		#region Simulator

		public string SimulatorUniqueName { get => _cache[APP_CONFIG_SIMULATOR_UNIQUE_NAME]; }

		public string SimulatorMac { get => (
												from nic in NetworkInterface.GetAllNetworkInterfaces()
												where nic.OperationalStatus == OperationalStatus.Up
												select nic.GetPhysicalAddress().ToString()
											).FirstOrDefault(); }

		public string SimulatorIp { get => _cache[APP_CONFIG_SIMULATOR_IP]; }

		public int SimulatorPort { get => _cache[APP_CONFIG_SIMULATOR_PORT].ParseToInt(); }

		private int _ingredientAdditionDelayMs = -1;

		public int IngredientAdditionDelayMs {
			get {
				if (_ingredientAdditionDelayMs < 0)
					_ingredientAdditionDelayMs = _cache[APP_CONFIG_SIMULATOR_DEFUALT_INGREDIENT_ADDITION_DELAY_MS].ParseToInt();
				return _ingredientAdditionDelayMs;
			}
			set {
				_ingredientAdditionDelayMs = value;
				OnConfigChangeEvent(PANEL_LINE_INGREDIENT_ADDITION_DELAY);
			}
		}

		#endregion Simulator

		#region Panels Configs

		public PanelConfig StatusPanelConfig { get => new PanelConfig(PanelTitle(STATUS), 0, 0, PanelWidth(STATUS), PanelHeight(STATUS), PanelColumns(STATUS)); }

		public PanelConfig ConfigsPanelConfig { get => new PanelConfig(PanelTitle(CONFIGS), 0, PanelHeight(STATUS) + VERTICAL_MARGIN_BETWEEN_PANELS, PanelWidth(CONFIGS), PanelHeight(CONFIGS), PanelColumns(CONFIGS)); }

		public PanelConfig CommandsPanelConfig { get => new PanelConfig(PanelTitle(COMMANDS), 0, PanelHeight(CONFIGS) + PanelHeight(STATUS) + 2 * VERTICAL_MARGIN_BETWEEN_PANELS, PanelWidth(COMMANDS), PanelHeight(COMMANDS), PanelColumns(COMMANDS)); }

		public PanelConfig LogPanelConfig { get => new PanelConfig(PanelTitle(LOG), 0, PanelHeight(COMMANDS) + PanelHeight(CONFIGS) + PanelHeight(STATUS) + 3 * VERTICAL_MARGIN_BETWEEN_PANELS, PanelWidth(LOG), PanelHeight(LOG), PanelColumns(LOG)); }

		#endregion Panels Configs

		#region Server/Communication

		public string ServerAddress { get => _cache[APP_CONFIG_SERVER_ADDRESS]; }

#warning add standart timeout no dashboard
		public int StandardTimeout { get => _cache[APP_CONFIG_STANDARD_TIMEOUT].ParseToInt(); }

		#endregion Server

		#region Registration Configs

		public string RegistrationUrl { get => $"http://{ServerAddress}/{RegistratioRoute}"; }

		public string RegistratioRoute { get => _cache[APP_CONFIG_REGISTRATION_ROUTE]; }
#warning excluir isso
		public int RegistrationTimeout { get => _cache[APP_CONFIG_REGISTRATION_TIMEOUT].ParseToInt(); }

		public int RegistrationWaitAfter10FailedAtemptMs { get => _cache[APP_CONFIG_REGISTRATION_WAIT_AFTER_FAILED_ATTEMPT_MS].ParseToInt(); }
#warning excluir isso
		public int RegistrationWaitAfterSuccessfulAttempMs { get => _cache[APP_CONFIG_REGISTRATION_WAIT_AFTER_SUCCESSFUL_ATTEMPT_MS].ParseToInt(); }

		#endregion Registration Configs

		#region Report Configs
#warning mudar nome de report status para report
		public string ReportUrl { get => $"http://{ServerAddress}/{ReportRoute}"; }

		public string ReportRoute { get => _cache[APP_CONFIG_REPORT_STATUS_ROUTE]; }
#warning excluir isso
		public int ReportTimeout { get => _cache[APP_CONFIG_REPORT_STATUS_TIMEOUT].ParseToInt(); }

		#endregion Report Status Configs

		#region Order Configs

		public string OrderUrl { get => $"http://{ServerAddress}/{OrderRoute}"; }

		public string OrderRoute { get => _cache[APP_CONFIG_ORDER_ROUTE]; }

		//public string OrderCompletedUrl { get => $"http://{ServerAddress}/{OrderCompletedRoute}"; }
#warning excluir isso
		//public string OrderCompletedRoute { get => _cache[APP_CONFIG_ORDER_COMPLETED_ROUTE]; }

		public int OrderTimeout { get => _cache[APP_CONFIG_ORDER_TIMEOUT].ParseToInt(); }

		#endregion Order Configs
	}
}