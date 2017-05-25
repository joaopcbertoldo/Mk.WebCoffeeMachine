namespace Mkafeina.Simulator
{
	public static class Constants
	{
		// panel lines
		public const string

		#region Status Panel Lines

			PANEL_LINE_IP = "ip",
			PANEL_LINE_PORT = "port",
			PANEL_LINE_UNIQUE_NAME = "uniqueName",
			PANEL_LINE_PIN = "pin",
			PANEL_LINE_REGISTRATION = "registration",
			PANEL_LINE_IS_ENABLED = "isEnabled",
			PANEL_LINE_IS_MAKING_COFFEE = "isMakingCofee",
			PANEL_LINE_COFFEE_LEVEL = "coffeeLevel",
			PANEL_LINE_WATER_LEVEL = "waterLevel",
			PANEL_LINE_SELECTED_INGREDIENT = "selectedIngredient",
			PANEL_LINE_SELECTED_RECIPE = "selectedRecipe",

		#endregion Status Panel Lines

		#region Configs Panel Lines

			PANEL_LINE_INGREDIENT_ADDITION_DELAY = "ingredient.additionDelay",
			PANEL_LINE_REGISTRATION_REQUEST_TIMEOUT = "registration.requestTimeout",
			PANEL_LINE_WAIT_AFTER_REGISTRATION_WAIT_AFTER_FAILED_ATTEMPT = "registration.waitAfterFailedAttempt",
			PANEL_LINE_WAIT_AFTER_REGISTRATION_WAIT_AFTER_SUCCESSFUL_ATTEMPT = "registration.waitAfterSuccessfulAttempt",

		#endregion Configs Panel Lines

		#region Commands Panel Lines

			PANEL_LINE_COMMAND_TAB = "tab",
			PANEL_LINE_COMMAND_SHIFT_TAB = "shiftTab",
			PANEL_LINE_COMMAND_ENTER = "enter",
			PANEL_LINE_COMMAND_LEFT_ARROW = "left",
			PANEL_LINE_COMMAND_RIGHT_ARROW = "right",
			PANEL_LINE_COMMAND_UP_ARROW = "up",
			PANEL_LINE_COMMAND_DOWN_ARROW = "down",
			PANEL_LINE_COMMAND_F5 = "F5",
			PANEL_LINE_COMMAND_F4 = "F4",
			PANEL_LINE_COMMAND_LESS_THAN = "lt",
			PANEL_LINE_COMMAND_GREATER_THAN = "gt";

		#endregion Commands Panel Lines

		// app.config
		public const string
			APP_CONFIG_SIMULATOR_UNIQUE_NAME = "simulator.uniqueName",
			APP_CONFIG_SIMULATOR_IP = "simulator.ip",
			APP_CONFIG_SIMULATOR_PORT = "simulator.port",
			APP_CONFIG_SIMULATOR_DEFUALT_INGREDIENT_ADDITION_DELAY_MS = "simulator.defaultIngredientAdditionDelayMs",

			APP_CONFIG_SERVER_ADDRESS = "server.address",
			APP_CONFIG_STANDARD_TIMEOUT = "standardTimeout",

			APP_CONFIG_REGISTRATION_ROUTE = "registration.route",
			APP_CONFIG_REGISTRATION_TIMEOUT = "registration.timeout",
			APP_CONFIG_REGISTRATION_WAIT_AFTER_FAILED_ATTEMPT_MS = "registration.waitAfterFailedAttemptMs",
			APP_CONFIG_REGISTRATION_WAIT_AFTER_SUCCESSFUL_ATTEMPT_MS = "registration.waitAfterSuccessfulAttemptMs",

			APP_CONFIG_REPORT_STATUS_ROUTE = "report.route",
			APP_CONFIG_REPORT_STATUS_TIMEOUT = "report.timeout",

			APP_CONFIG_ORDER_ROUTE = "order.route",
			APP_CONFIG_ORDER_TIMEOUT = "order.timeout";

		// others
		public const string
			INGREDIENTS_NEXT = "next",
			INGREDIENTS_PREVIOUS = "previous",
			INGREDIENTS_WATER = "water",
			INGREDIENTS_COFFEE = "coffee",
			INGREDIENTS_WATER_CHAR = "w",
			INGREDIENTS_COFFEE_CHAR = "c";

		// others
		public const int
#warning relocalizar algumas consts
			ACK = 0,
			TRUE_UNIQUE_NAME = 1,
			COMMAND = 1,
			ORDER_REFERENCE = 1,
			RECIPE = 2,
			REGISTRATION_ATTEMPT_RESPONSE_CODE = 2,
			REGISTRATION_ACCEPTANCE_RESPONSE_CODE = 1,
			INGREDIENT_KEY_BOARD_INCREMENT = 10,
			COFFEE_ITERATION_DECREMENT = 1,
			WATER_ITERATION_DECREMENT = 10,
			INGREDIENT_ADDITION_DELAY_INCREMENT = 100,
			VERTICAL_MARGIN_BETWEEN_PANELS = 1;
	}
}