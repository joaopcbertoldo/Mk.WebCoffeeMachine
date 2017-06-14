using Mkafeina.CoffeeMachineSimulator;
using Mkafeina.Domain.Dashboard;
using System;

namespace Mkafeina.Simulator
{
	public class CommandInterpreter : AbstractCommandInterpreter
	{
		public const string
			LEFT_ARROW = "left",
			RIGHT_ARROW = "right",
			UP_ARROW = "up",
			DOWN_ARROW = "down",
			REENABLE = "reenable",
			DISABLE = "disable",
			ONOFF = "on/off",
			OFFSETS = "offsets"
			;

		public override void HandleCommand(ConsoleKeyInfo key)
		{
			switch (key.Key)
			{
				case ConsoleKey.D:
					FakeCoffeMachine.Sgt._disableFlag = true;
					break;

				case ConsoleKey.Enter:
					if (FakeCoffeMachine.Sgt.IsRunning)
						FakeCoffeMachine.Sgt.TurnOff();
					else
						FakeCoffeMachine.Sgt.TurnOn();
					break;

				case ConsoleKey.O:
					FakeCoffeMachine.Sgt._sendOffsetsFlag = true;
					break;

				case ConsoleKey.LeftArrow:
					IngredientManipulator.Sgt.PreviousIngredient();
					break;

				case ConsoleKey.RightArrow:
					IngredientManipulator.Sgt.NextIngredient();
					break;

				case ConsoleKey.UpArrow:
					IngredientManipulator.Sgt.IncrementSelectedIngredient(negative: false);
					break;

				case ConsoleKey.DownArrow:
					IngredientManipulator.Sgt.IncrementSelectedIngredient(negative: true);
					break;

				case ConsoleKey.R:
					FakeCoffeMachine.Sgt._reenableFlag = true;
					break;

				default:
					return;
			}
		}
	}
}