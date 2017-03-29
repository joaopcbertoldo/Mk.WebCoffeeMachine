using System;
using System.Threading.Tasks;
using Mkfeina.Domain;
using Mkfeina.Domain.Panels;
using static Mkfeina.CoffeeMachineSimulator.Constants;

namespace Mkfeina.CoffeeMachineSimulator
{
    public static class Dashboard
    {
        private static long __logCounter = 0;

        private static Panel __statusPanel;
        private static Panel __configsPanel;
        private static Panel __commandsPanel;
        private static Panel __logPanel;
        private static CommandInterpreter __commandInterpreter;
        private static Action<ConsoleKeyInfo> __keyEvent;
        private static PanelLineBuilder __panelLineBuilder;

        public static void Load()
        {
#warning transformar em dependency injection ???????
            __commandInterpreter = new SimulatorCommandInterpreter();
            __keyEvent += __commandInterpreter.HandleCommand;

#warning transformar em dependency injection ????????????
            __panelLineBuilder = new SimulatorPanelLineBuilder();

            FakeCoffeMachine.Singleton.StatusChangeEvent += OnSimulatorPanelChangeEvent;
            AppConfig.ConfigChangeEvent += OnSimulatorPanelChangeEvent;

            Console.Title = AppConfig.SimulatorUniqueName;

            LoadPanels();
        }

        public static void LoadPanels()
        {

#warning colocar isso tudo no app config
            __statusPanel = Panel.Factory.CreatePanel(AppConfig.StatusPanelConfig);
            __statusPanel.AddFixedLineAsync(PANEL_LINE_IP, __panelLineBuilder.Build(PANEL_LINE_IP));
            __statusPanel.AddFixedLineAsync(PANEL_LINE_PORT, __panelLineBuilder.Build(PANEL_LINE_PORT));
            __statusPanel.AddFixedLineAsync(PANEL_LINE_SIMULATOR_UNIQUE_NAME, __panelLineBuilder.Build(PANEL_LINE_SIMULATOR_UNIQUE_NAME));
            __statusPanel.AddFixedLineAsync(PANEL_LINE_PIN, __panelLineBuilder.Build(PANEL_LINE_PIN));
            __statusPanel.AddFixedLineAsync(PANEL_LINE_IS_REGISTERED, __panelLineBuilder.Build(PANEL_LINE_IS_REGISTERED));
            __statusPanel.AddFixedLineAsync(PANEL_LINE_IS_MAKING_COFFEE, __panelLineBuilder.Build(PANEL_LINE_IS_MAKING_COFFEE));
            __statusPanel.AddFixedLineAsync(PANEL_LINE_COFFEE, __panelLineBuilder.Build(PANEL_LINE_COFFEE));
            __statusPanel.AddFixedLineAsync(PANEL_LINE_WATER, __panelLineBuilder.Build(PANEL_LINE_WATER));
            __statusPanel.AddFixedLineAsync(PANEL_LINE_INGREDIENT, __panelLineBuilder.Build(PANEL_LINE_INGREDIENT));
            __statusPanel.AddFixedLineAsync(PANEL_LINE_RECIPE, __panelLineBuilder.Build(PANEL_LINE_RECIPE));

            __configsPanel = Panel.Factory.CreatePanel(AppConfig.ConfigsPanelConfig);
            __configsPanel.AddFixedLineAsync(PANEL_LINE_INGREDIENT_ADDITION_DELAY, __panelLineBuilder.Build(PANEL_LINE_INGREDIENT_ADDITION_DELAY));
            __configsPanel.AddFixedLineAsync(PANEL_LINE_INGREDIENT_KEYBOARD_INCREMENT, __panelLineBuilder.Build(PANEL_LINE_INGREDIENT_KEYBOARD_INCREMENT));
            __configsPanel.AddFixedLineAsync(PANEL_LINE_REGISTRATION_REQUEST_TIMEOUT, __panelLineBuilder.Build(PANEL_LINE_REGISTRATION_REQUEST_TIMEOUT));
            __configsPanel.AddFixedLineAsync(PANEL_LINE_WAIT_AFTER_REGISTRATION_FAILED_ATTEMPT, __panelLineBuilder.Build(PANEL_LINE_WAIT_AFTER_REGISTRATION_FAILED_ATTEMPT));
            __configsPanel.AddFixedLineAsync(PANEL_LINE_WAIT_AFTER_REGISTRATION_SUCCESSFUL_ATTEMPT, __panelLineBuilder.Build(PANEL_LINE_WAIT_AFTER_REGISTRATION_SUCCESSFUL_ATTEMPT));
            __configsPanel.AddFixedLineAsync(PANEL_LINE_REGISTRATION_MANAGER_SLEEP, __panelLineBuilder.Build(PANEL_LINE_REGISTRATION_MANAGER_SLEEP));
            __configsPanel.AddFixedLineAsync(PANEL_LINE_MAX_TIME_ELAPSED_BETWEEN_RECEIVED_REQUESTS, __panelLineBuilder.Build(PANEL_LINE_MAX_TIME_ELAPSED_BETWEEN_RECEIVED_REQUESTS));
            
            __commandsPanel = Panel.Factory.CreatePanel(AppConfig.CommandsPanelConfig);
            __commandsPanel.AddFixedLineAsync(COMMAND_TAB, COMMAND_TAB_PANEL_LINE);
            __commandsPanel.AddFixedLineAsync(COMMAND_SHIFT_TAB, COMMAND_SHIFT_TAB_PANEL_LINE);
            __commandsPanel.AddFixedLineAsync(COMMAND_ENTER, COMMAND_ENTER_PANEL_LINE);
            __commandsPanel.AddFixedLineAsync(COMMAND_LEFT_ARROW, COMMAND_LEFT_ARROW_PANEL_LINE);
            __commandsPanel.AddFixedLineAsync(COMMAND_RIGHT_ARROW, COMMAND_RIGHT_ARROW_PANEL_LINE);
            __commandsPanel.AddFixedLineAsync(COMMAND_UP_ARROW, COMMAND_UP_ARROW_PANEL_LINE);
            __commandsPanel.AddFixedLineAsync(COMMAND_DOWN_ARROW, COMMAND_DOWN_ARROW_PANEL_LINE);
            __commandsPanel.AddFixedLineAsync(COMMAND_F5, COMMAND_F5_PANEL_LINE);
            __commandsPanel.AddFixedLineAsync(COMMAND_F4, COMMAND_F4_PANEL_LINE);
            __commandsPanel.AddFixedLineAsync(COMMAND_LESS_THAN, COMMAND_LESS_THAN_PANEL_LINE);
            __commandsPanel.AddFixedLineAsync(COMMAND_GREATER_THAN, COMMAND_GREATER_THAN_PANEL_LINE);

            __logPanel = Panel.Factory.CreatePanel(AppConfig.LogPanelConfig);
            __logPanel.AddFixedLineAsync(PANEL_LINE_LAST_RECEIVED_REQUEST, __panelLineBuilder.Build(PANEL_LINE_LAST_RECEIVED_REQUEST));
            __logPanel.AddFixedLineAsync(PANEL_LINE_LAST_EVENT, __panelLineBuilder.Build(PANEL_LINE_LAST_EVENT));

            LogAsync("Dashboard is set!");
        }

        public static void StartCommandInterpreter()
        {
            while (true)
                __commandInterpreter.HandleCommand(Console.ReadKey(intercept: true));
        }

        public static Task ReloadPanels()
            => Task.Factory.StartNew(() => {
                __statusPanel.Unregister();
                __configsPanel.Unregister();
                __commandsPanel.Unregister();
                __logPanel.Unregister();

                __statusPanel.CleanUp();
                __configsPanel.CleanUp();
                __commandsPanel.CleanUp();
                __logPanel.CleanUp();

                __statusPanel = __statusPanel.TransferLines(Panel.Factory.CreatePanel(AppConfig.StatusPanelConfig));
                __configsPanel = __configsPanel.TransferLines(Panel.Factory.CreatePanel(AppConfig.ConfigsPanelConfig));
                __commandsPanel = __commandsPanel.TransferLines(Panel.Factory.CreatePanel(AppConfig.CommandsPanelConfig));
                __logPanel = __logPanel.TransferLines(Panel.Factory.CreatePanel(AppConfig.LogPanelConfig));

                __statusPanel.ReprintEverythingAsync();
                __configsPanel.ReprintEverythingAsync();
                __commandsPanel.ReprintEverythingAsync();
                __logPanel.ReprintEverythingAsync();

                LogAsync("Dashboard is reset!");
            });

        public static void OnSimulatorPanelChangeEvent(string changedInfo)
            => __statusPanel.RefreshFixedLineAsync(changedInfo, __panelLineBuilder.Build(changedInfo));

        public static void LogAsync(string message)
        {
            __logPanel.AddRollingLineAsync(message.ToLogMessage(++__logCounter));
            __logPanel.RefreshFixedLineAsync(PANEL_LINE_LAST_EVENT, __panelLineBuilder.Build(PANEL_LINE_LAST_EVENT));
        }

        public static void ReprintEverythingAsync()
        {
            __statusPanel.ReprintEverythingAsync();
            __commandsPanel.ReprintEverythingAsync();
            __logPanel.ReprintEverythingAsync();
        }
    }
}