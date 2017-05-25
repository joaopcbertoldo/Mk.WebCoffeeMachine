using static Mkafeina.Domain.Panels.Constants;

namespace Mkafeina.Domain.Panels
{
    public class PanelConfig
    {
        public string Title { get; set; }

        public int OriginLeft { get; set; }

        public int OriginTop { get; set; }

        public int Width { get; set; }

        public int Hight { get; set; }

        public int Columns { get; set; }

        public PanelConfig(string title, int originLeft, int originTop, int width, int hight, int columns)
        {
            Title = title;
            OriginLeft = originLeft;
            OriginTop = originTop;
            Width = width;
            Hight = hight;
            Columns = columns;
        }

        public PanelConfig(string title, int[] square, int columns) :
            this(title, square[SQUARE_LEFT], square[SQUARE_TOP], square[SQUARE_WIDTH], square[SQUARE_HIGHT], columns)
        {
        }

        public PanelConfig(string title, int[] position, int[] size, int columns) :
            this(title, position[SQUARE_LEFT], position[SQUARE_TOP], size[SIZE_WIDTH], size[SIZE_HEIGHT], columns)
        {
        }
    }
}