using System.Collections.Generic;
using System.Linq;
using static WebCoffeeMachine.Domain.Panels.Constants;

namespace WebCoffeeMachine.Domain.Panels
{
    internal static class ConsoleSpaceManager
    {
        private static List<int[]> __usedSpaces = new List<int[]>();

        internal static bool RegisterSpaceUsage(int left, int top, int width, int height)
        {
            if (left < 0 || top < 0 || width <= 0 || height <= 0)
                return false;

            bool spaceIsAvailable = true;
            foreach (var used in __usedSpaces) {
                int left0 = used[SQUARE_LEFT], top0 = used[SQUARE_TOP], width0 = used[SQUARE_WIDTH], height0 = used[SQUARE_HIGHT];

                spaceIsAvailable &= left >= (left0 + width0 + MARGIN_BETWEEN_PANELS) ||
                                    top >= (top0 + height0 + MARGIN_BETWEEN_PANELS) ||
                                    (left < (left0 - MARGIN_BETWEEN_PANELS) && top < (top0 - MARGIN_BETWEEN_PANELS) && (left + width) < (left0 - MARGIN_BETWEEN_PANELS) && (top + height) < (top0 - MARGIN_BETWEEN_PANELS)) ||
                                    (left >= (left0 - MARGIN_BETWEEN_PANELS) && (top + height) < (top0 - MARGIN_BETWEEN_PANELS)) ||
                                    (top >= (top0 - MARGIN_BETWEEN_PANELS) && (left + width) < (left0 - MARGIN_BETWEEN_PANELS));

                if (!spaceIsAvailable)
                    return false;
            }

            __usedSpaces.Add(new int[] { left, top, width, height });

            return true;
        }

        internal static bool UnregisterSpaceUsage(int left, int top)
        {
            var space = __usedSpaces.FirstOrDefault(usedSpace => usedSpace[SQUARE_LEFT] == left && usedSpace[SQUARE_TOP] == top);
            if (space == null)
                return false;
            __usedSpaces.Remove(space);
            return true;
        }
    }
}