using Mkafeina.Domain.ServerArduinoComm;
using Mkafeina.Domain.ArduinoApi;
using Mkafeina.Server.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Mkafeina.Domain;

namespace Mkafeina.Server.Domain.CoffeeMachineProxy
{
	public class CMProxyState
	{
		public const string
			ENABLED = "enabled",
			REGISTRATION = "registration",
			MAKING_COFFEE = "makingCoffee";

		private Dictionary<string, Ingredient> _ingredients;

		public static CMProxyState CreateCMProxyState(CMProxy owner, string mac, string uniqueName, IngredientsSetup setup)
		{
			var state = new CMProxyState()
			{
				_ingredients = new Dictionary<string, Ingredient>(Ingredient.GetAllExistingIngredientsInstances()),
				Mac = mac,
				UniqueName = uniqueName,
				RegistrationIsAccepted = false,
				IsMakingCoffee = false,
				Enabled = false
			};
			state.SetupAvaiabilityAndOffsets(setup);
			state.ChangeEvent += owner.OnStateChangeEvent;
			return state;
		}

		private CMProxyState() { }

		private string _uniqueName;
		private bool _isMakingCoffee;
		private bool _registrationIsAccepted;
		private bool _isEnabled;

		public event Action<string, object> ChangeEvent;

		public string UniqueName {
			get => _uniqueName;
			set {
				_uniqueName = value;
			}
		}

		public bool IsMakingCoffee {
			get => _isMakingCoffee;
			set {
				_isMakingCoffee = value;
				ChangeEvent?.Invoke(MAKING_COFFEE, this);
			}
		}

		public int? GetLevel(string ingredientName) => _ingredients[ingredientName].Level;

		public void SetSignal(string ingredientName, float value)
		{
			_ingredients[ingredientName].Signal = value;
			ChangeEvent?.Invoke(ingredientName, this);
		}

		public void SetupIngredient(string ingredientName, bool available, float? emptyOffset = null, float? fullOffset = null)
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

		public bool RegistrationIsAccepted {
			get => _registrationIsAccepted;
			set {
				_registrationIsAccepted = value;
				ChangeEvent?.Invoke(REGISTRATION, this);
			}
		}

		public bool Enabled {
			get => _isEnabled;
			set {
				_isEnabled = value;
				ChangeEvent?.Invoke(ENABLED, this);
			}
		}

		public string Mac { get; set; }

		public IEnumerable<string> AvailableIngredients { get => _ingredients.Where(i => i.Value.Available).Select(i => i.Value.Name).ToList(); }

		public bool LevelsUnderMinimum() => _ingredients.Values.Any(i => i.Level < i.MinimumLevel);

		public void Update(ReportRequest request)
		{
			_ingredients.Values
						.ToList()
						.ForEach(i =>
						{
							i.Signal = (float)typeof(ReportRequest).GetProperty(i.Name).GetValue(request);
							ChangeEvent(i.Name, this);
						});
			Enabled = request.IsEnabled;
			IsMakingCoffee = false;
		}
	}
}