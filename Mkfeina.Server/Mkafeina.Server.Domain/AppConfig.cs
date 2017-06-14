using Mkafeina.Domain;
using Mkafeina.Server.Domain.Entities;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mkafeina.Server.Domain
{
	public class AppConfig : AbstractAppConfig
	{
		public const string
			RECIPES = "recipes",
			INGREDIENTS = "ingredients",
			CONFIGS = "configs",

			SERVER_ADDRESS = "server.address",
			SERVER_NICE_ADDRESS = "server.niceAddress",
			SERVER_NAME = "server.name",

			WAITRESS_QUEUE_CAPACITY = "waitress.queueCapacity",
			WAITRESS_MINIMUM_SECONDS_BETWEEN_ORDERS = "waitress.minimumSecondsBetweenOrders",

			EMAIL_SENDER_USER = "emailSender.user",
			EMAIL_SENDER_PASSWORD = "emailSender.password",
			EMAIL_SENDER_HOST = "emailSender.host",
			EMAIL_SENDER_PORT = "emailSender.port",
			EMAIL_SENDER_ENABLE_SSL = "emailSender.enableSsl",
			EMAIL_SENDER_DEFAULT_CREDENTIALS = "emailSender.defaultCredentials",
			EMAIL_SENDER_TIMEOUT_MS = "emailSender.timeoutMs",

			MINIMUM_LEVEL = "minimumLevel"
			;

		public AppConfig()
		{
			ReloadConfigs();
		}

		public string ServerAddress { get => _cache[SERVER_ADDRESS]; }
		public string ServerNiceAddress { get => _cache[SERVER_NICE_ADDRESS]; }
		public string ServerName { get => _cache[SERVER_NAME]; }

		public int WaitressCapacity { get => _cache[WAITRESS_QUEUE_CAPACITY].ParseToInt(); }
		public int MinimumSecondsBetweenOrders { get => _cache[WAITRESS_MINIMUM_SECONDS_BETWEEN_ORDERS].ParseToInt(); }

		public string EmailSenderUser { get => _cache[EMAIL_SENDER_USER]; }
		public string EmailSenderPassword { get => _cache[EMAIL_SENDER_PASSWORD]; }
		public string EmailSenderHost { get => _cache[EMAIL_SENDER_HOST]; }
		public int EmailSenderPort { get => _cache[EMAIL_SENDER_PORT].ParseToInt(); }
		public bool EmailSenderEnableSsl { get => _cache[EMAIL_SENDER_ENABLE_SSL].ParseToBool(); }
		public bool EmailSenderUseDefaultCredentials { get => _cache[EMAIL_SENDER_DEFAULT_CREDENTIALS].ParseToBool(); }
		public int EmailSenderTimeoutMs { get => _cache[EMAIL_SENDER_TIMEOUT_MS].ParseToInt(); }

		internal int IngredientMinimumLevel(string name) => _cache[($"{name}." + MINIMUM_LEVEL)].ParseToInt();

		public IEnumerable<string> Recipes { get => _cache[RECIPES].SplitValueSeparatedBy(","); }

		public IEnumerable<string> Ingredients { get => _cache [INGREDIENTS].SplitValueSeparatedBy(","); }

		public int? Portion(string recipe, string ingredient) => _cache[$"{recipe}.{ingredient}"]?.ParseToInt();

	}
}