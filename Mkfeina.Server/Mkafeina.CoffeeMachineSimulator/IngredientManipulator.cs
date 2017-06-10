using Mkafeina.Simulator;
using Mkfeina.Simulator;
using System;
using System.Collections.Generic;

namespace Mkafeina.CoffeeMachineSimulator
{
	public class IngredientManipulator
	{
		private const float KEY_BOARD_INCREMENT = (float)0.1;

		public const string
			NEXT = "next",
			PREVIOUS = "previous",
			COFFEE = "Coffee",
			SUGAR = "Sugar",
			WATER = "Water",
			SELECTED = "selected"
			;

		private static IngredientManipulator __sgt = new IngredientManipulator();
		public static IngredientManipulator Sgt { get => __sgt; }

		private IngredientManipulator()
		{
			_ingredients = new LinkedList<string>(new string[] { COFFEE, SUGAR, WATER });
			_selectedIngredient = _ingredients.First;
		}

		private LinkedList<string> _ingredients;
		private LinkedListNode<string> _selectedIngredient;
		public event Action<string, object> ChangeEvent;

		private void OnChangeEvent(string lineName) => ChangeEvent?.Invoke(lineName, this);

		public void NextIngredient()
		{
			_selectedIngredient = _selectedIngredient.NextOrFirst();
			OnChangeEvent(SELECTED);
		}

		public void PreviousIngredient()
		{
			_selectedIngredient = _selectedIngredient.PreviousOrLast();
			OnChangeEvent(SELECTED);
		}

		public string SelectedIngredient { get => _selectedIngredient.Value; }

		public void IncrementSelectedIngredient(bool negative = false)
		{
			var increment = (negative ? -1 : 1) * KEY_BOARD_INCREMENT;
			switch (_selectedIngredient.Value)
			{
				case COFFEE:
					FakeCoffeMachine.Sgt.Signals.Coffee += increment;
					break;

				case SUGAR:
					FakeCoffeMachine.Sgt.Signals.Sugar += increment;
					break;

				case WATER:
					FakeCoffeMachine.Sgt.Signals.Water += increment;
					break;

				default:
					return;
			}
		}
	}
}