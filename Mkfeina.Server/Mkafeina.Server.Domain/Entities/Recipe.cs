using Mkafeina.Server.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mkafeina.Server.Domain.Entities
{
	public class Recipe
	{
		private Dictionary<string, int> _ingredients = new Dictionary<string, int>();

		public string Name { get; set; }

		public IEnumerable<string> AllIngredients { get => _ingredients.Select(kv => kv.Key); }

		public int this[string ingredientName] => _ingredients[ingredientName];

		public bool AddIngredient(string ingredientName, int portion)
		{
			if (_ingredients.ContainsKey(ingredientName) || portion <= 0)
				return false;
			_ingredients.Add(ingredientName, portion);
			return true;
		}

		public override string ToString()
			=> $"({string.Join(",", _ingredients.Select(kv => $"{Ingredient.GetCode(kv.Key)}={kv.Value}").ToArray())})";

		public static Recipe Parse(string str)
		{
			var recipe = new Recipe();

			var parentesis = str[0]; // get the (
			if (parentesis != '(')
				return null;

			while (true)
			{
				str = str.Remove(0, 1); // cut the (

				var ingredientCode = str[0];  // get the char for the resource
				str = str.Remove(0, 1); // remove the char and the =

				var capture = Regex.Match(str, @"=\d+"); // get the number
				var captureStr = capture.ToString().Remove(0, 1);
				var portion = int.Parse(captureStr); // parse to an int
				str = str.Remove(0, capture.Length); // remove the number

				recipe.AddIngredient(Ingredient.GetName(ingredientCode), portion);

				var nextChar = str[0]; // get the next char

				if (nextChar == ',')
					continue;
				else if (nextChar == ')')
					break;
				else
					return null;
			}

			return recipe;
		}
	}
}