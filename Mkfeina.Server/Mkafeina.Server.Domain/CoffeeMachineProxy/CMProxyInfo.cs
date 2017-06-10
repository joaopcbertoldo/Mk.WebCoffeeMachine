using Mkafeina.Domain.ArduinoApi;
using Mkafeina.Server.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Mkafeina.Server.Domain.CoffeeMachineProxy
{
	public class CMProxyInfo
	{
		public const string
			ENABLED = "enabled",
			REGISTRATION = "registration",
			MAKING_COFFEE = "makingCoffee";

		private Dictionary<string, Ingredient> _ingredients;

		public static CMProxyInfo CreateCMProxyInfo(CMProxy owner, string mac, string uniqueName, IngredientsSetup setup)
		{
			var info = new CMProxyInfo()
			{
				_ingredients = new Dictionary<string, Ingredient>(Ingredient.GetAllExistingIngredientsInstances()),
				Mac = mac,
				UniqueName = uniqueName
			};
			info.SetupAvaiabilityAndOffsets(setup);
			_owner = owner;
			return info;
		}

		private CMProxyInfo()
		{
		}

		private string _uniqueName;
		private bool _makingCoffee;
		private bool _enabled;
		private static CMProxy _owner;

		public IEnumerable<string> AllRecipesNames { get => _owner._cookbook.AllRecipesNames; }

		internal bool HasRecipe(string recipeName) => _owner._cookbook[recipeName] == null ? false : true;

		public IEnumerable<string> AvailableIngredients { get => _ingredients.Where(i => i.Value.Available).Select(i => i.Value.Name).ToList(); }

		public bool LevelsUnderMinimum { get => _ingredients.Values.Any(i => i.Level < i.MinimumLevel); }

		public bool Enabled {
			get => _enabled;
			set {
				_enabled = value;
				_owner.OnChangeEvent(ENABLED);
			}
		}

		public bool MakingCoffee {
			get => _makingCoffee;
			set {
				_makingCoffee = value;
				_owner.OnChangeEvent(MAKING_COFFEE);
			}
		}

		public string Mac { get; set; }

		public string UniqueName {
			get => _uniqueName;
			set {
				_uniqueName = value;
			}
		}

		public int? GetLevel(string ingredientName) => _ingredients[ingredientName].Level;

		public void UpdateIngredients(IngredientsSignals signals)
		{
			_ingredients.Values
						.ToList()
						.ForEach(i =>
						{
							i.Signal = (float)typeof(IngredientsSignals).GetProperty(i.Name).GetValue(signals);
							_owner.OnChangeEvent(i.Name);
						});
		}

		public void SetupIngredient(string ingredientName, bool available, float? emptyOffset = null, float? fullOffset = null, int? minimumLevel = null)
		{
			_ingredients[ingredientName].Available = available;
			_ingredients[ingredientName].EmptyOffset = emptyOffset;
			_ingredients[ingredientName].FullOffset = fullOffset;
		}

		public void SetupAvaiabilityAndOffsets(IngredientsSetup setup)
		{
			var ingredients = Ingredient.GetAllExistingIngredientsNames().ToList();
			ingredients.ForEach(iName =>
			{
				var isAvailable = (bool)typeof(IngredientsSetup).GetProperty($"{iName}Available").GetValue(setup);
				if (isAvailable)
				{
					var empty = (float)typeof(IngredientsSetup).GetProperty($"{iName}EmptyOffset").GetValue(setup);
					var full = (float)typeof(IngredientsSetup).GetProperty($"{iName}FullOffset").GetValue(setup);
					SetupIngredient(iName, isAvailable, empty, full);
				}
				else
					SetupIngredient(iName, isAvailable);
			});
		}
	}
}