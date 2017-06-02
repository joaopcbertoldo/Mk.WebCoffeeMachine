using Mkafeina.Domain;
using System;
using static Mkafeina.Domain.Dashboard.Panels.Constants;
using static Mkafeina.Domain.Extentions;

namespace Mkfeina.Domain.Dashboard.Panels
{
	public class ConsoleWriter
	{
		private static object __consoleWriteSyncObj = new object();

		public void WriteLine(int[] position, string text)
		{
			// write the title line
			lock (__consoleWriteSyncObj)
			{
				Console.SetCursorPosition(position[LEFT], position[TOP]);
				Console.WriteLine(text);
				Console.SetCursorPosition(CURSOR_ORIGIN_LEFT, CURSOR_ORIGIN_TOP);
			}
		}

		public void CleanUp(int[] space)
		{
			lock (__consoleWriteSyncObj)
			{
				for (var top = space[TOP]; top < space[TOP] + space[HEIGHT]; top++)
				{
					Console.SetCursorPosition(space[LEFT], top);
					Console.Write(" ".AdjustLength(space[WIDTH]));
				}
				Console.SetCursorPosition(CURSOR_ORIGIN_LEFT, CURSOR_ORIGIN_TOP);
			}
		}
	}
}