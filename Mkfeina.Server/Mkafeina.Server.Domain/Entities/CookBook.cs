using Microsoft.Practices.Unity;
using Mkafeina.Domain;
using Mkafeina.Server.Domain.CoffeeMachineProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mkafeina.Server.Domain.Entities
{
	public abstract class CookBook
	{
		private Dictionary<string, Recipe> _recipes = new Dictionary<string, Recipe>();

		public Recipe this[string recipeName] { get { try { return _recipes[recipeName]; } catch { return null; } } }

		public IEnumerable<string> AllRecipesNames { get => _recipes.Select(kv => kv.Key); }

		public IEnumerable<KeyValuePair<string, Recipe>> AllRecipes(IEnumerable<string> availabelIngredients = null)
			=> availabelIngredients == null ? _recipes.ToList() :
									 _recipes.Where(kv => kv.Value.AllIngredients.Intersect(availabelIngredients).Count() ==
														  kv.Value.AllIngredients.Count());

		public void UpsertRecipe(Recipe recipe)
		{
			if (recipe != null)
			{
				if (_recipes.ContainsKey(recipe.Name))
					_recipes.Remove(recipe.Name);
				_recipes.Add(recipe.Name, recipe);
			}
		}
	}

	public class MainCookBook : CookBook
	{
		public MainCookBook() : base()
		{
			ReloadRecipesFromAppConfig(wait: true);
		}

		public void ReloadRecipesFromAppConfig(bool wait = false)
		{
			var appconfig = (AppConfig)AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractAppConfig>();
			appconfig.ReloadConfigs();

			var recipesNames = appconfig.Recipes;
			var ingredients = appconfig.Ingredients;
			var task = Task.Factory.StartNew(() =>
			{
				lock (this)
				{
					foreach (var name in recipesNames)
					{
						var recipe = new Recipe() { Name = name };
						foreach (var i in ingredients)
						{
							var portion = appconfig.Portion(name, i);
							if (portion == null)
								continue;
							recipe.AddIngredient(i, portion.Value);
						}
						UpsertRecipe(recipe);
					}
				}
			});

			if (wait)
				task.Wait();
		}

	}

	public class ProxyCookBook : CookBook
	{
		private CMProxy _owner;

		public ProxyCookBook(CMProxy owner) : base()
		{
			_owner = owner;
		}

		public void GetRecipesFromMainCookbook()
		{
			var mainCookbook = AppDomain.CurrentDomain.UnityContainer().Resolve<MainCookBook>();
			foreach (var recipe in mainCookbook.AllRecipes(_owner?.Info.AvailableIngredients))
				UpsertRecipe(recipe.Value);
			_owner.OnChangeEvent(CMProxy.CMPROXY_RECIPES);
		}
	}
}