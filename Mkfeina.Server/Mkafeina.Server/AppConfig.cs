using Mkafeina.Domain.Panels;
using static Mkafeina.Server.Constants;

namespace Mkafeina.Server
{
	public class AppConfig : Mkafeina.Domain.AppConfig
	{
		private const string
			STATUS = "status",
			CONFIGS = "configs",
			COMMANDS = "commands",
			LOG = "log";

		#region Sinlgeton Stuff

		private AppConfig() : base()
		{
		}

		private static AppConfig __sgt;

		public static AppConfig Sgt {
			get {
				if (__sgt == null)
					__sgt = new AppConfig();
				return __sgt;
			}
		}

		#endregion Sinlgeton Stuff

		#region Panels Configs

		public PanelConfig StatusPanelConfig { get => new PanelConfig(PanelTitle(STATUS), 0, 0, PanelWidth(STATUS), PanelHeight(STATUS), PanelColumns(STATUS)); }

		public PanelConfig ConfigsPanelConfig { get => new PanelConfig(PanelTitle(CONFIGS), 0, PanelHeight(STATUS) + VERTICAL_MARGIN_BETWEEN_PANELS, PanelWidth(CONFIGS), PanelHeight(CONFIGS), PanelColumns(CONFIGS)); }

		public PanelConfig CommandsPanelConfig { get => new PanelConfig(PanelTitle(COMMANDS), 0, PanelHeight(CONFIGS) + PanelHeight(STATUS) + 2 * VERTICAL_MARGIN_BETWEEN_PANELS, PanelWidth(COMMANDS), PanelHeight(COMMANDS), PanelColumns(COMMANDS)); }

		public PanelConfig LogPanelConfig { get => new PanelConfig(PanelTitle(LOG), 0, PanelHeight(COMMANDS) + PanelHeight(CONFIGS) + PanelHeight(STATUS) + 3 * VERTICAL_MARGIN_BETWEEN_PANELS, PanelWidth(LOG), PanelHeight(LOG), PanelColumns(LOG)); }

		#endregion Panels Configs

		public string ServerAddress { get => _cache[SERVER_ADDRESS]; }
		public string ServerNiceAddress { get => _cache[SERVER_NICE_ADDRESS]; }
		public string ServerName { get => _cache[SERVER_NAME]; }
	}
}