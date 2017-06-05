using Microsoft.Practices.Unity;
using Mkafeina.Domain;
using Mkafeina.Server.Domain.CoffeeMachineProxy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mkafeina.Server.Domain.Entities
{
	public class CookBook
	{
		public const string
			RECIPES = "recipes";

		private Dictionary<string, Recipe> _recipes = new Dictionary<string, Recipe>();

		private CMProxy _boss;

		public event Action<string, object> ChangeEvent;

		public static CookBook CreateCookbook(CMProxy owner = null)
		{
			if (owner == null)
				return new CookBook();

			var cookbook = new CookBook()
			{
				_boss = owner
			};
			cookbook.GetRecipesFromMainCookbook();
			cookbook.ChangeEvent += owner.OnStateChangeEvent;
			return cookbook;
		}

		private CookBook()
		{
		}

		public void GetRecipesFromMainCookbook()
		{
			var mainCookbook = AppDomain.CurrentDomain.UnityContainer().Resolve<CookBook>();
			foreach (var recipe in mainCookbook.AllRecipes(_boss?.State.AvailableIngredients))
				UpsertRecipe(recipe.Value);
			ChangeEvent?.Invoke(RECIPES, _boss);
		}

		public void UpsertRecipe(Recipe recipe)
		{
			if (recipe != null)
			{
				if (_recipes.ContainsKey(recipe.Name))
					_recipes.Remove(recipe.Name);
				_recipes.Add(recipe.Name, recipe);
			}
		}

		public IEnumerable<string> AllRecipesNames { get => _recipes.Select(kv => kv.Key); }

		public IEnumerable<KeyValuePair<string, Recipe>> AllRecipes(IEnumerable<string> availabelIngredients = null)
			=> availabelIngredients == null ? _recipes.ToList() :
									 _recipes.Where(kv => kv.Value.AllIngredients.Intersect(availabelIngredients).Count() ==
														  kv.Value.AllIngredients.Count());

		public Recipe this[string recipeName] { get => _recipes[recipeName]; }
	}
}