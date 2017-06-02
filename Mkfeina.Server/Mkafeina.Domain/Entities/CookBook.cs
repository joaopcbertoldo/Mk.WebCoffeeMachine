using System.Collections.Generic;
using System.Linq;

namespace Mkafeina.Domain.Entities
{
	public class CookBook
	{
		private Dictionary<string, Recipe> _recipes;

		public void AddRecipe(Recipe recipe)
		{
			if (recipe != null)
			{
				if (_recipes.ContainsKey(recipe.Name))
					_recipes.Remove(recipe.Name);
				_recipes.Add(recipe.Name, recipe);
			}
		}

		public IEnumerable<string> AllRecipesNames { get => _recipes.Select(kv => kv.Key); }

		public IEnumerable<KeyValuePair<string, Recipe>> AllRecipes { get => _recipes.ToList(); }

		public Recipe this[string recipeName] { get => _recipes[recipeName]; }
	}
}