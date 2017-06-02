using Microsoft.Practices.Unity;
using Mkafeina.Domain.Dashboard.Panels;
using Mkafeina.Domain.Entities;
using Mkafeina.Simulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Mkafeina.Domain.Constants;

namespace Mkafeina.Domain
{
	public abstract class AbstractAppConfig
	{
		private const string
			RECIPES = "recipes",
			INGREDIENTS = "ingredients";

		protected AppSettingsCache _cache = new AppSettingsCache();

		public event Action<string,object> ConfigChangeEvent;

		protected AbstractAppConfig()
		{
			ReloadConfigs();
		}

		public void ReloadConfigs()
		{
			_cache.RefreshCache();
			foreach (var key in _cache.AllKeys)
				ConfigChangeEvent?.Invoke(key,this);
		}

		protected void OnConfigChangeEvent(string obj) => ConfigChangeEvent?.Invoke(obj,this);

		#region Panels Stuff

		public IEnumerable<string> PanelsNames { get => _cache[APP_CONFIG_PANELS_NAMES].SplitValueSeparatedBy(","); }

		public IDictionary<string, PanelConfig> PanelsConfigs {
			get {
				var configs = new Dictionary<string, PanelConfig>();
				var appConfigType = AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractAppConfig>().GetType();
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

		public int PanelNLines(string panelName)
			=> _cache[$"{panelName}.{APP_CONFIG_PANEL_NLINES_PROP}"].ParseToInt();

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

		public IEnumerable<string> LoadRecipesOnCookBookAsync(CookBook cookbook, bool wait = false)
		{
			_cache.RefreshCache();
			var recipesNames = _cache[RECIPES].SplitValueSeparatedBy(",");
			var ingredients = _cache[INGREDIENTS].SplitValueSeparatedBy(",");
			var task = Task.Factory.StartNew(() =>
			{
				lock (cookbook)
				{
					foreach (var name in recipesNames)
					{
						var recipe = new Recipe() { Name = name };
						foreach (var ingredient in ingredients)
						{
							var portion = _cache[$"{name}.{ingredient}"]?.ParseToInt();
							if (portion == null)
								continue;
							recipe.AddIngredient(ingredient.ToCharArray()[0], portion.Value);
						}
						cookbook.AddRecipe(recipe);
					}
				}
			});

			if (wait)
				task.Wait();

			return recipesNames;
		}
	}
}