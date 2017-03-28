namespace WebCoffeeMachine.Domain.Panels
{
    internal static class Constants
    {
        internal const int SQUARE_LEFT = 0, SQUARE_TOP = 1, SQUARE_WIDTH = 2, SQUARE_HIGHT = 3,
                           POSITION_LEFT = 0, POSITION_TOP = 1,
                           SIZE_WIDTH = 0, SIZE_HEIGHT = 1,
                           MARGIN_BETWEEN_PANELS = 1,
                           MARGIN_BETWEEN_COLUMNS = 1,
                           EMPTY_LINES_BETWEEN_ROLLING_AND_FIXED = 1;

        internal const char TITLE_FULFILLER = '#';

        internal static int[] CURSOR_ORIGIN = new int[] { 0, 0 },
                              NOT_VISIBLE_LINE = new int[] { -1, -1 };
    }
}