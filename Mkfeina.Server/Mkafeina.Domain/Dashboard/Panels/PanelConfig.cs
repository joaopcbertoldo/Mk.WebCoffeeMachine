using static Mkafeina.Domain.Dashboard.Panels.Constants;

namespace Mkafeina.Domain.Dashboard.Panels
{
	public class PanelConfig
	{
		internal Panel OwnerPanel { get; set; }

		public string Title { get; set; }

		public int NLines { get; set; }

		public int Columns { get; set; }

		public int SpaceId { get; set; }

		public int[] Space { get => ConsoleSpaceManager.GetSpace(SpaceId); }

		public int ColumnWidth { get => Space[WIDTH] / Columns - (Columns - 1) * MARGIN_BETWEEN_COLUMNS; }

		public int NVisibleLines { get => Columns * (NLines - 1); } // -1 because of the title

		public int NVisibleLinesPerColumn { get => NLines - 1; } // -1 because of the title

		public int NFixedLines { get => OwnerPanel._fixedLines.Count; }

		public int NRollingLines { get => OwnerPanel._rollingLines.Count; }
	}
}