using Mkafeina.Simulator;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mkafeina.Domain
{
	public class CookBook
	{
		#region Singleton Stuff

		private static CookBook _sgt = new CookBook();

		public static CookBook Sgt { get => _sgt; }

		private CookBook()
		{
		}

		#endregion Singleton Stuff

		private const string
			RECIPES = "recipes",
			INGREDIENTS = "ingredients";

		private Dictionary<string, Recipe> _recipes;

		public IEnumerable<string> LoadRecipes(bool wait = false)
		{
			var settingsCache = new AppSettingsCache();
			settingsCache.RefreshCache();
			var recipesNames = settingsCache[RECIPES].SplitValueSeparatedBy(",");

			var task = Task.Factory.StartNew(() =>
			{
				lock (this)
				{
					var ingredients = settingsCache[INGREDIENTS].SplitValueSeparatedBy(",");

					_recipes = null;
					_recipes = new Dictionary<string, Recipe>();
					foreach (var name in recipesNames)
					{
						var recipe = new Recipe() { Name = name };
						foreach (var ingredient in ingredients)
						{
							var portion = settingsCache[$"{name}.{ingredient}"]?.ParseToInt();
							if (portion == null)
								continue;
							recipe.AddIngredient(ingredient.ToCharArray()[0], portion.Value);
						}
						_recipes.Add(name, recipe);
					}
				}
			});

			if (wait)
				task.Wait();

			return recipesNames;
		}

		public IEnumerable<string> AllRecipesNames { get => _recipes.Select(kv => kv.Key); }

		public IEnumerable<KeyValuePair<string, Recipe>> AllRecipes { get => _recipes.ToList(); }

		public Recipe this[string recipeName] { get => _recipes[recipeName]; }
	}
}