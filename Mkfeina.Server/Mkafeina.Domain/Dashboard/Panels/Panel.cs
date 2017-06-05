using Microsoft.Practices.Unity;
using Mkafeina.Domain.Dashboard.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Mkafeina.Domain.Dashboard.Panels.Constants;
using static Mkafeina.Domain.Extentions;

namespace Mkafeina.Domain.Dashboard.Panels
{
	public class Panel
	{
		private const int EMPTY_LINES_BETWEEN_ROLLING_AND_FIXED = 1;

		private const char TITLE_FULFILLER = '#';

		private static int[] NOT_VISIBLE_LINE = new int[] { -1, -1 };

		internal Dictionary<string, string> _fixedLines;

		internal LinkedList<string> _rollingLines;

		protected AbstractPanelLineBuilder _lineBuilder;

		protected ConsoleWriter _consoleWriter;

		public PanelConfig Config { get; private set; }

		internal Panel(PanelConfig config)
		{
			var id = ConsoleSpaceManager.RegisterSpaceUsage(config.NLines);
			config.SpaceId = id;
			config.OwnerPanel = this;
			_fixedLines = new Dictionary<string, string>();
			_rollingLines = new LinkedList<string>();
			_lineBuilder = AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractPanelLineBuilder>();
			_consoleWriter = new ConsoleWriter();
			Config = config;
			PrintTitle();
		}

		private void PrintTitle()
			=> _consoleWriter.WriteLine(new int[] { Config.Space[LEFT], Config.Space[TOP] },
									   Config.Title.AdjustLength(Console.WindowWidth, TITLE_FULFILLER, FulfillStringMode.Centered));

		public void AddFixedLinesAsync(IEnumerable<string> linesNames, bool wait)
		{
			var task = Task.Factory.StartNew(() =>
			{
				lock (_fixedLines)
				{
					foreach (var name in linesNames)
						_fixedLines.Add(name, _lineBuilder.BuildOrUpdate(name));
				}
				ReprintEverythingAsync();
			});

			if (wait)
			{
				if (task.Status == TaskStatus.Running)
					task.Wait();
			}
		}

		public void RefreshFixedLineAsync(string name, string content)
		{
			Task.Factory.StartNew(() =>
			{
				lock (_fixedLines)
				{
					_fixedLines[name] = content.AdjustLength(Config.ColumnWidth);
					var position = GetCursorPositionForFixedLine(name);
					if (position == NOT_VISIBLE_LINE)
						return;
					_consoleWriter.WriteLine(position, _fixedLines[name]);
				}
			});
		}

		public void UpdateEventHandler(string lineToUpdate, object caller)
		{
			if (_fixedLines.ContainsKey(lineToUpdate))
				RefreshFixedLineAsync(lineToUpdate, _lineBuilder.BuildOrUpdate(lineToUpdate, caller));
		}

		public void AddRollingLineAsync(string content)
		{
			Task.Factory.StartNew(() =>
			{
				if (content == null)
					return;

				lock (_rollingLines)
				{
					_rollingLines.AddFirst(content.AdjustLength(Config.ColumnWidth));
				}
				ReprintAllRollingLinesAsync();
			});
		}

		protected void ReprintAllFixedLinesAsync()
		{
			Task.Factory.StartNew(() =>
			{
				lock (_fixedLines)
				{
					foreach (var line in _fixedLines)
					{
						var position = GetCursorPositionForFixedLine(line.Key);
						if (position == NOT_VISIBLE_LINE)
							break;
						_consoleWriter.WriteLine(position, _fixedLines[line.Key]);
					}
				}
			});
		}

		protected void ReprintAllRollingLinesAsync()
		{
			Task.Factory.StartNew(() =>
			{
				lock (_rollingLines)
				{
					for (var rollingLineIndex = 0; rollingLineIndex < _rollingLines.Count; rollingLineIndex++)
					{
						var position = GetCursorPositionForRollingLine(rollingLineIndex);
						if (position == NOT_VISIBLE_LINE)
						{
							// delete all those that are not visible
							var firstNotVisibleIndex = rollingLineIndex;
							string firstNotVisible;
							do
							{
								_rollingLines.RemoveLast();
								firstNotVisible = _rollingLines.ElementAtOrDefault(firstNotVisibleIndex);
							} while (firstNotVisible != null);
						}
						else
							_consoleWriter.WriteLine(position, _rollingLines.ElementAt(rollingLineIndex));
					}
				}
			});
		}

		public void ReprintEverythingAsync()
		{
			ReprintAllFixedLinesAsync();
			ReprintAllRollingLinesAsync();
		}

		public void Unregister() => ConsoleSpaceManager.UnregisterSpaceUsage(Config.SpaceId);

		public void CleanUp() => _consoleWriter.CleanUp(Config.Space);

		protected int[] GetCursorPositionForFixedLine(string name)
			=> GetCursorPosition(_fixedLines.Keys.ToList().IndexOf(name));

		protected int[] GetCursorPositionForRollingLine(int rollingLineRelativeIndex)
			=> GetCursorPosition(Config.NFixedLines + rollingLineRelativeIndex + EMPTY_LINES_BETWEEN_ROLLING_AND_FIXED);

		protected int[] GetCursorPosition(int absoluteLineIndex)
		{
			if (absoluteLineIndex >= Config.NVisibleLines)
				return NOT_VISIBLE_LINE;
			var column = absoluteLineIndex / Config.NVisibleLinesPerColumn;
			var lineInColumn = absoluteLineIndex % Config.NVisibleLinesPerColumn;
			var left = column * (Config.ColumnWidth + MARGIN_BETWEEN_COLUMNS);
			var top = Config.Space[TOP] + 1 + lineInColumn; // the "1" is to avoid the title line
			return new int[] { left, top };
		}
	}
}