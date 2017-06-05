using Mkafeina.Domain;
using Mkafeina.Server.Domain.Entities;
using System.Threading.Tasks;
using System;

namespace Mkafeina.Server.Domain
{
	public class AppConfig : AbstractAppConfig
	{
		public const string
			RECIPES = "recipes",
			INGREDIENTS = "ingredients",
#warning checar se estas consts estao sendo usadas
			STATUS = "status",
			CONFIGS = "configs",
			COMMANDS = "commands",
			LOG = "log",

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

			MINIMUM_LEVEL = "minimumLevel";


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

		public void LoadRecipesOnCookBookAsync(CookBook cookbook, bool wait = false)
		{
			ReloadConfigs();

			var recipesNames = _cache[RECIPES].SplitValueSeparatedBy(",");
			var ingredients = _cache[INGREDIENTS].SplitValueSeparatedBy(",");
			var task = Task.Factory.StartNew(() =>
			{
				lock (cookbook)
				{
					foreach (var name in recipesNames)
					{
						var recipe = new Recipe() { Name = name };
						foreach (var i in ingredients)
						{
							var portion = _cache[$"{name}.{i}"]?.ParseToInt();
							if (portion == null)
								continue;
							recipe.AddIngredient(i, portion.Value);
						}
						cookbook.UpsertRecipe(recipe);
					}
				}
			});

			if (wait)
				task.Wait();
		}
	}
}