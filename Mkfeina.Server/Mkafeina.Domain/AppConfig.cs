using Microsoft.Practices.Unity;
using Mkafeina.Domain.Panels;
using Mkafeina.Simulator;
using System;
using System.Collections.Generic;
using System.Linq;
using static Mkafeina.Domain.Constants;

namespace Mkafeina.Domain
{
	public abstract class AppConfig
	{
		protected AppSettingsCache _cache = new AppSettingsCache();

		public event Action<string> ConfigChangeEvent;

		protected AppConfig()
		{
			ReloadConfigs();
		}

		public void ReloadConfigs()
		{
			_cache.RefreshCache();
			foreach (var key in _cache.AllKeys)
				ConfigChangeEvent?.Invoke(key);
		}

		protected void OnConfigChangeEvent(string obj) => ConfigChangeEvent?.Invoke(obj);

		#region Panels Stuff

		public IEnumerable<string> PanelsNames { get => _cache[APP_CONFIG_PANELS_NAMES].SplitValueSeparatedBy(","); }

		public IDictionary<string, PanelConfig> PanelsConfigs {
			get {
				var configs = new Dictionary<string, PanelConfig>();
				var appConfigType = AppDomain.CurrentDomain.UnityContainer().Resolve<AppConfig>().GetType();
				foreach (var name in PanelsNames)
					configs.Add(name, (PanelConfig)appConfigType.GetProperties()
																.First(
								prop => prop.Name == name.FirstLetterToUpper() + APP_CONFIG_PANEL_CONFIG_PROPERTY_TERMINATION
																 ).GetMethod
																  .Invoke(this, null));
					return configs;
			}
		}

		public string PanelTitle(string panelName)
			=> _cache[$"{panelName}.{APP_CONFIG_PANEL_TITLE_PROP}"];

		public int PanelHeight(string panelName)
			=> _cache[$"{panelName}.{APP_CONFIG_PANEL_HEIGHT_PROP}"].ParseToInt();

		public int PanelWidth(string panelName)
		{
			var width = _cache[$"{panelName}.{APP_CONFIG_PANEL_WIDTH_PROP}"];
			if (width == APP_CONFIG_PANEL_WIDTH_FULL_WINDOW)
				return Console.WindowWidth;
			return width.ParseToInt();
		}

		public int PanelColumns(string panelName)
			=> _cache[$"{panelName}.{APP_CONFIG_PANEL_COLUMNS_PROP}"].ParseToInt();

		public IDictionary<string, IEnumerable<string>> PanelsLinesCollections {
			get {
				var linesCollections = new Dictionary<string, IEnumerable<string>>();
				foreach (var name in PanelsNames)
					linesCollections.Add(name, _cache[$"{name}.{APP_CONFIG_PANEL_LINES_PROP}"].SplitValueSeparatedBy(","));
				return linesCollections;
			}
		}

		#endregion Panels Stuff
	}
}