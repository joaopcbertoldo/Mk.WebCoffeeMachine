using System;
using System.Configuration;

namespace Mkfeina.Simulator
{
	public class AppSettingsCache
	{
		private const string APP_SETTINGS = "appSettings";

		private ExeConfigurationFileMap _configFileMap = new ExeConfigurationFileMap();

		private KeyValueConfigurationCollection _settings;

		public AppSettingsCache()
		{
			_configFileMap.ExeConfigFilename = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
		}

		public void RefreshCache()
		{
			Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(_configFileMap, ConfigurationUserLevel.None);
			var section = (AppSettingsSection)configuration.GetSection(APP_SETTINGS);
			_settings = section.Settings;
		}

		public string this[string key] { get => _settings[key].Value; }

		public string[] AllKeys { get => _settings.AllKeys; }

		
	}
}