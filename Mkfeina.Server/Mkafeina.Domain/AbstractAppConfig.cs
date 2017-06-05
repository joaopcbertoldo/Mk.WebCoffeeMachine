using Mkafeina.Domain.Dashboard.Panels;
using Mkafeina.Simulator;
using System;
using System.Collections.Generic;

namespace Mkafeina.Domain
{
	public abstract class AbstractAppConfig
	{
		private const string
			FIXED_PANELS_NAMES = "fixedPanelsNames",

			TITLE_PROP = "title",
			NLINES_PROP = "nLines",
			COLUMNS_PROP = "columns",

			FIXED_LINES_PROP = "fixedLines";

		protected AppSettingsCache _cache = new AppSettingsCache();

		public event Action<string, object> ConfigChangeEvent;

		protected AbstractAppConfig()
		{
			ReloadConfigs();
		}

		public void ReloadConfigs()
		{
			_cache.RefreshCache();
			foreach (var key in _cache.AllKeys)
				ConfigChangeEvent?.Invoke(key, this);
		}

		protected void OnConfigChangeEvent(string obj) => ConfigChangeEvent?.Invoke(obj, this);

		#region Panels Stuff

		public IEnumerable<string> PanelsNames { get => _cache[FIXED_PANELS_NAMES].SplitValueSeparatedBy(","); }

		public IDictionary<string, PanelConfig> FixedPanelsConfigs {
			get {
				var configs = new Dictionary<string, PanelConfig>();
				foreach (var name in PanelsNames)
					configs.Add(name, PanelConfigs(name));
				return configs;
			}
		}

		public IDictionary<string, IEnumerable<string>> AllPanelsFixedLines {
			get {
				var linesDic = new Dictionary<string, IEnumerable<string>>();
				foreach (var name in PanelsNames)
					linesDic.Add(name, PanelFixedLines(name));
				return linesDic;
			}
		}

		public PanelConfig PanelConfigs(string panelName)
			=> new PanelConfig()
			{
				Title = PanelTitle(panelName),
				NLines = PanelNLines(panelName),
				Columns = PanelColumns(panelName)
			};

		public IEnumerable<string> PanelFixedLines(string panelName)
			=> _cache[$"{panelName}.{FIXED_LINES_PROP}"].SplitValueSeparatedBy(",");

		public string PanelTitle(string panelName)
			=> _cache[$"{panelName}.{TITLE_PROP}"];

		public int PanelNLines(string panelName)
			=> _cache[$"{panelName}.{NLINES_PROP}"].ParseToInt();

		public int PanelColumns(string panelName)
			=> _cache[$"{panelName}.{COLUMNS_PROP}"].ParseToInt();

		#endregion Panels Stuff
	}
}