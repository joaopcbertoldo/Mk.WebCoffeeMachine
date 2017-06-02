using Mkafeina.Domain;
using Mkafeina.Domain.Dashboard.Panels;
using static Mkafeina.Server.Constants;

namespace Mkafeina.Server
{
	public class AppConfig : AbstractAppConfig
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

		public PanelConfig StatusPanelConfig { get => new PanelConfig() { Title = PanelTitle(STATUS), Columns = PanelColumns(STATUS), NLines = PanelNLines(STATUS) }; }

		public PanelConfig ConfigsPanelConfig { get => new PanelConfig() { Title = PanelTitle(CONFIGS), Columns = PanelColumns(CONFIGS), NLines = PanelNLines(CONFIGS) }; }

		public PanelConfig CommandsPanelConfig { get => new PanelConfig() { Title = PanelTitle(COMMANDS), Columns = PanelColumns(COMMANDS), NLines = PanelNLines(COMMANDS) }; }

		public PanelConfig LogPanelConfig { get => new PanelConfig() { Title = PanelTitle(LOG), Columns = PanelColumns(LOG), NLines = PanelNLines(LOG) }; }

		#endregion Panels Configs

		public string ServerAddress { get => _cache[SERVER_ADDRESS]; }
		public string ServerNiceAddress { get => _cache[SERVER_NICE_ADDRESS]; }
		public string ServerName { get => _cache[SERVER_NAME]; }
	}
}