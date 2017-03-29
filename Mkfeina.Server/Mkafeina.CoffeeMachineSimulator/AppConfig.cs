using System;
using System.Collections.Generic;
using System.Configuration;
using Mkfeina.Domain.Panels;
using static Mkfeina.CoffeeMachineSimulator.Constants;

namespace Mkfeina.CoffeeMachineSimulator
{
#warning change the class to use app setings cache

    public static class AppConfig
    {
        private const string APP_SETTINGS = "appSettings";

        private static Dictionary<string, string> __settings = new Dictionary<string, string>();

        public static void LoadAppConfig()
        {
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
            var section = (AppSettingsSection)configuration.GetSection(APP_SETTINGS);
            var settings = section.Settings;

            foreach (var key in settings.AllKeys) {
                if (__settings.ContainsKey(key)) {
                    __settings[key] = settings[key].Value;
                } else
                    __settings.Add(key, settings[key].Value);
            }
        }

        public static event Action<string> ConfigChangeEvent;

        public static PanelConfig StatusPanelConfig { get => new PanelConfig(SimulatorUniqueName, 0, 0, StatusPanelWidth, StatusPanelHeight, StatusPanelColumns); }

        public static PanelConfig ConfigsPanelConfig { get => new PanelConfig(ConfigsPanelTitle, 0, StatusPanelHeight + VERTICAL_MARGIN_BETWEEN_PANELS, ConfigsPanelWidth, ConfigsPanelHeight, ConfigsPanelColumns); }

        public static PanelConfig CommandsPanelConfig { get => new PanelConfig(CommandsPanelTitle, 0, ConfigsPanelHeight + StatusPanelHeight + 2 * VERTICAL_MARGIN_BETWEEN_PANELS, CommandsPanelWidth, CommandsPanelHeight, CommandsPanelColumns); }

        public static PanelConfig LogPanelConfig { get => new PanelConfig(LogPanelTitle, 0, CommandsPanelHeight + ConfigsPanelHeight + StatusPanelHeight + 3 * VERTICAL_MARGIN_BETWEEN_PANELS, LogPanelWidth, LogPanelHeight, LogPanelColumns); }

        public static string SimulatorUniqueName { get => __settings[APP_CONFIG_SIMULATOR_UNIQUE_NAME]; }

        public static string SimulatorIp { get => __settings[APP_CONFIG_SIMULATOR_IP]; }

        public static int SimulatorPort { get => int.Parse(__settings[APP_CONFIG_SIMULATOR_PORT]); }

        public static int MaxLogMessages { get => int.Parse(__settings[APP_CONFIG_MAX_LOG_MESSAGES]); }

        public static int StatusPanelWidth {
            get {
                var fromAppConfig = __settings[APP_CONFIG_STATUS_PANEL_WIDTH];
                if (fromAppConfig == APP_CONFIG_FULL_WINDOW)
                    return Console.WindowWidth;
                return int.Parse(fromAppConfig);
            }
        }

        public static int StatusPanelHeight { get => int.Parse(__settings[APP_CONFIG_STATUS_PANEL_HIGHT]); }

        public static int StatusPanelColumns { get => int.Parse(__settings[APP_CONFIG_STATUS_PANEL_COLUMNS]); }

        public static string ConfigsPanelTitle { get => __settings[APP_CONFIG_CONFIGS_PANEL_TITLE]; }

        public static int ConfigsPanelWidth {
            get {
                var fromAppConfig = __settings[APP_CONFIG_CONFIGS_PANEL_WIDTH];
                if (fromAppConfig == APP_CONFIG_FULL_WINDOW)
                    return Console.WindowWidth;
                return int.Parse(fromAppConfig);
            }
        }

        public static int ConfigsPanelHeight { get => int.Parse(__settings[APP_CONFIG_CONFIGS_PANEL_HIGHT]); }

        public static int ConfigsPanelColumns { get => int.Parse(__settings[APP_CONFIG_CONFIGS_PANEL_COLUMNS]); }

        public static string CommandsPanelTitle { get => __settings[APP_CONFIG_COMMANDS_PANEL_TITLE]; }

        public static int CommandsPanelWidth {
            get {
                var fromAppConfig = __settings[APP_CONFIG_COMMANDS_PANEL_WIDTH];
                if (fromAppConfig == APP_CONFIG_FULL_WINDOW)
                    return Console.WindowWidth;
                return int.Parse(fromAppConfig);
            }
        }

        public static int CommandsPanelHeight { get => int.Parse(__settings[APP_CONFIG_COMMANDS_PANEL_HIGHT]); }

        public static int CommandsPanelColumns { get => int.Parse(__settings[APP_CONFIG_COMMANDS_PANEL_COLUMNS]); }

        public static string LogPanelTitle { get => __settings[APP_CONFIG_LOG_PANEL_TITLE]; }

        public static int LogPanelWidth {
            get {
                var fromAppConfig = __settings[APP_CONFIG_LOG_PANEL_WIDTH];
                if (fromAppConfig == APP_CONFIG_FULL_WINDOW)
                    return Console.WindowWidth;
                return int.Parse(fromAppConfig);
            }
        }

        public static int LogPanelHeight { get => int.Parse(__settings[APP_CONFIG_LOG_PANEL_HIGHT]); }

        public static int LogPanelColumns { get => int.Parse(__settings[APP_CONFIG_LOG_PANEL_COLUMNS]); }

        private static int _ingredientKeyboardIncrement = -1;

        public static int IngredientKeyboardIncrement {
            get => _ingredientKeyboardIncrement < 0 ? int.Parse(__settings[APP_CONFIG_INGREDIENT_KEY_BOARD_INCREMENT]) : _ingredientKeyboardIncrement;
            set {
                _ingredientKeyboardIncrement = value;
                ConfigChangeEvent(PANEL_LINE_INGREDIENT_KEYBOARD_INCREMENT);
            }
        }

        private static int __ingredientAdditionDelayMs = -1;

        public static int IngredientAdditionDelayMs {
            get => __ingredientAdditionDelayMs < 0 ? int.Parse(__settings[APP_CONFIG_INGREDIENT_ADDITION_DELAY_MS]) : __ingredientAdditionDelayMs;
            set {
                __ingredientAdditionDelayMs = value;
                ConfigChangeEvent(PANEL_LINE_INGREDIENT_ADDITION_DELAY);
            }
        }

        public static string ServerIp { get => __settings[APP_CONFIG_SERVER_IP]; }

        public static int ServerPort { get => int.Parse(__settings[APP_CONFIG_SERVER_PORT]); }

        public static string RegistrationUrl { get => $"http://{ServerIp}:{ServerPort}/register"; }

        public static int RegistrationTimeout { get => int.Parse(__settings[APP_CONFIG_REGISTRATION_TIMEOUT]); }

        public static int RegistrationWaitAfterFailedAttempMs { get => int.Parse(__settings[APP_CONFIG_REGISTRATION_WAIT_AFTER_FAILED_ATTEMPT_MS]); }

        public static int RegistrationWaitAfterSuccessfulAttempMs { get => int.Parse(__settings[APP_CONFIG_REGISTRATION_WAIT_AFTER_SUCCESSFUL_ATTEMPT_MS]); }

        public static int RegistrationManagerSleepMs { get => int.Parse(__settings[APP_CONFIG_REGISTRATION_MANAGER_SLEEP_MS]); }

        public static int MaxTimeElapsedBetweenReceivedRequestsMs { get => int.Parse(__settings[APP_CONFIG_MAX_TIME_ELAPSED_BETWEEN_RECEIVED_REQUESTS_MS]); }
    }
}