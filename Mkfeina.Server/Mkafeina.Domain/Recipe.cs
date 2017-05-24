using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mkfeina.Domain
{
	public class Recipe : IEnumerable<KeyValuePair<char, int>>
	{
		private Dictionary<char, int> _ingredients = new Dictionary<char, int>();

		public string Name { get; set; }

		public bool AddIngredient(char ingredient, int portion)
		{
			if (_ingredients.ContainsKey(ingredient) || portion <= 0)
				return false;
			_ingredients.Add(ingredient, portion);
			return true;
		}

		public int this[char ingredient] => _ingredients[ingredient];

		public override string ToString()
			=> $"({string.Join(",", _ingredients.Select(kv => $"{kv.Key}={kv.Value}").ToArray())})";

		public static Recipe Parse(string str)
		{
			var recipe = new Recipe();

			var parentesis = str[0]; // get the (
			if (parentesis != '(')
				return null;

			while (true)
			{
				str = str.Remove(0, 1); // cut the (

				var ingredient = str[0];  // get the char for the resource
				str = str.Remove(0, 1); // remove the char and the =

				var capture = Regex.Match(str, @"=\d+"); // get the number
				var captureStr = capture.ToString().Remove(0, 1);
				var portion = int.Parse(captureStr); // parse to an int
				str = str.Remove(0, capture.Length); // remove the number

				recipe.AddIngredient(ingredient, portion);

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

		public IEnumerator<KeyValuePair<char, int>> GetEnumerator() => _ingredients.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _ingredients.GetEnumerator();
	}
}