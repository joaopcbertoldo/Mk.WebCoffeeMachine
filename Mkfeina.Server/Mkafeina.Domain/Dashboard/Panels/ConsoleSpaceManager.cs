using System;
using System.Collections.Generic;
using System.Linq;
using static Mkafeina.Domain.Dashboard.Panels.Constants;

namespace Mkafeina.Domain.Dashboard.Panels
{
	public static class ConsoleSpaceManager
	{
		private const int VERTICAL_MARGIN_BETWEEN_PANELS = 1;

		private static List<int[]> __usedSpaces = new List<int[]>();

		private static int __lastId = 0;

		public static int RegisterSpaceUsage(int nLines)
		{
			int left = 0;
			int width = Console.WindowWidth;
			int top;
			if (__usedSpaces.Any())
			{
				var lastSpace = __usedSpaces.Last();
				top = lastSpace[TOP] + lastSpace[HEIGHT] + VERTICAL_MARGIN_BETWEEN_PANELS;
			}
			else
				top = 0;
			int height = nLines;

			// *** OLD CODE 1 ***

			var id = __lastId++;
			__usedSpaces.Add(new int[] { left, top, width, height, id });

			return id;
		}

		public static bool UnregisterSpaceUsage(int id)
		{
			// *** OLD CODE 2 ***
			if (!__usedSpaces.Any(s => s[ID] == id))
				return false;
			else
			{
				var spaceToDelete = __usedSpaces.First(s => s[ID] == id);
				var height = spaceToDelete[HEIGHT];
				__usedSpaces.Remove(spaceToDelete);
				foreach (var space in __usedSpaces)
				{
					if (space[ID] < id)
						continue;
					else
					{
						space[ID]--;
						space[TOP] -= (height + VERTICAL_MARGIN_BETWEEN_PANELS);
					}
				}
				return true;
			}
		}

		public static int[] GetSpace(int id)
		{
			if (!__usedSpaces.Any(s => s[ID] == id))
				return null;
			else
			{
				var space = __usedSpaces.First(s => s[ID] == id);
				return new int[] { space[LEFT], space[TOP], space[WIDTH], space[HEIGHT] };
			}
		}
	}
}

// *** OLD CODE 1 ***
/*
if (left< 0 || top< 0 || width <= 0 || height <= 0)
                return false;

            bool spaceIsAvailable = true;
            foreach (var used in __usedSpaces) {
                int left0 = used[SQUARE_LEFT], top0 = used[SQUARE_TOP], width0 = used[SQUARE_WIDTH], height0 = used[SQUARE_HIGHT];

spaceIsAvailable &= left >= (left0 + width0 + MARGIN_BETWEEN_PANELS) ||
                                    top >= (top0 + height0 + MARGIN_BETWEEN_PANELS) ||
                                    (left<(left0 - MARGIN_BETWEEN_PANELS) && top<(top0 - MARGIN_BETWEEN_PANELS) && (left + width) < (left0 - MARGIN_BETWEEN_PANELS) && (top + height) < (top0 - MARGIN_BETWEEN_PANELS)) ||
                                    (left >= (left0 - MARGIN_BETWEEN_PANELS) && (top + height) < (top0 - MARGIN_BETWEEN_PANELS)) ||
                                    (top >= (top0 - MARGIN_BETWEEN_PANELS) && (left + width) < (left0 - MARGIN_BETWEEN_PANELS));

                if (!spaceIsAvailable)
                    return false;
            }
*/

// *** OLD CODE 2 ***
/*
  var space = __usedSpaces.FirstOrDefault(usedSpace => usedSpace[SQUARE_LEFT] == left && usedSpace[SQUARE_TOP] == top);
			if (space == null)
				return false;
	*/