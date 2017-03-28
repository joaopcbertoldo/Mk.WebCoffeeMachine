using System;
using System.Collections.Generic;
using System.Configuration;

namespace WebCoffeeMachine.CoffeeMachineSimulator
{
    public class AppSettingsCache
    {
        private const string APP_SETTINGS = "appSettings";

        private Dictionary<string, string> _settings;

        public void LoadSettingsFromAppConfig()
        {
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
            var section = (AppSettingsSection)configuration.GetSection(APP_SETTINGS);
            _settings = new Dictionary<string, string>();

            foreach (var key in section.Settings.AllKeys)
                _settings.Add(key, section.Settings[key].Value);
        }

        public string this[string key] {
            get {
                try {
                    var value = _settings[key];
                    return value;
                } catch (Exception) {
                    return null;
                }
            }
        }

        // the string parameter is the name of the changed setting
        public event Action<string> AppConfigChangeEvent;
    }
}