using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static WebCoffeeMachine.Domain.Extensions;
using static WebCoffeeMachine.Domain.Panels.Constants;

namespace WebCoffeeMachine.Domain.Panels
{
    public class Panel
    {
        public static PanelFactory Factory = new PanelFactory();

        protected static object __consoleWriteSyncObj = new object();

        protected Dictionary<string, string> _fixedLines = new Dictionary<string, string>();

        protected LinkedList<string> _rollingLines = new LinkedList<string>();

        protected readonly int _columnWidth;

        public string Title { get; private set; }

        public int[] Origin { get; private set; }

        public int[] Size { get; private set; }

        /// <summary>
        /// Number of colums
        /// </summary>
        public int Columns { get; private set; }

        public int VisibleLines { get => Columns * (Size[SIZE_HEIGHT] - 1); }

        public int VisibleLinesPerColumn { get => VisibleLines / Columns; }

        /// <summary>
        /// Number of lines
        /// </summary>
        public int Lines { get => FixedLines + RollingLines + EMPTY_LINES_BETWEEN_ROLLING_AND_FIXED; }

        /// <summary>
        /// Number of fixed lines
        /// </summary>
        public int FixedLines { get => _fixedLines.Count; }

        /// <summary>
        /// Number of rolling lines
        /// </summary>
        public int RollingLines { get => _rollingLines.Count; }

        internal Panel(PanelConfig config)
        {
            Title = config.Title;
            Origin = new int[] { config.OriginLeft, config.OriginTop };
            Size = new int[] { config.Width, config.Hight };
            Columns = config.Columns;
            _columnWidth = Size[SIZE_WIDTH] / Columns - (Columns - 1) * MARGIN_BETWEEN_COLUMNS;
            // write the title line
            lock (__consoleWriteSyncObj) {
                Console.SetCursorPosition(Origin[POSITION_LEFT], Origin[POSITION_TOP]);
                Console.WriteLine(Title.AdjustLength(Console.WindowWidth, TITLE_FULFILLER, FulfillStringMode.Centered));
                Console.SetCursorPosition(CURSOR_ORIGIN[POSITION_LEFT], CURSOR_ORIGIN[POSITION_TOP]);
            }
        }

        public void AddFixedLineAsync(string name, string content = null)
        {
            Task.Factory.StartNew(() => {
                if (content == null)
                    content = $"{name.ToUpper()}...";

                lock (_fixedLines) {
                    _fixedLines.Add(name, content);
                }

#warning manter isto ???????????????????????????????????????????????????????????????????????????????????????????????????????????
                ReprintEverythingAsync();
            });
        }

        public void AddManyFixedLinesAsync(IEnumerable<KeyValuePair<string, string>> lines)
        {
            Task.Factory.StartNew(() => {
                lock (_fixedLines) {
                    foreach (var line in lines) {
                        var content = line.Value;
                        if (content == null)
                            content = $"{line.Key.ToUpper()}...";
                        _fixedLines.Add(line.Key, content);
                    }
                }
#warning manter isto ???????????????????????????????????????????????????????????????????????????????????????????????????????????
                ReprintEverythingAsync();
            });
        }

        public void RefreshFixedLineAsync(string name, string content)
        {
            Task.Factory.StartNew(() => {
                lock (_fixedLines) {
                    _fixedLines[name] = content.AdjustLength(_columnWidth);
                    var position = GetCursorPositionForFixedLine(name);
                    if (position == NOT_VISIBLE_LINE)
                        return;
                    lock (__consoleWriteSyncObj) {
                        Console.SetCursorPosition(position[POSITION_LEFT], position[POSITION_TOP]);
                        Console.Write(_fixedLines[name]);
                        Console.SetCursorPosition(CURSOR_ORIGIN[POSITION_LEFT], CURSOR_ORIGIN[POSITION_TOP]);
                    }
                }
            });
        }

        protected void ReprintAllFixedLinesAsync()
        {
            Task.Factory.StartNew(() => {
                lock (_fixedLines) {
                    foreach (var line in _fixedLines) {
                        var position = GetCursorPositionForFixedLine(line.Key);
                        if (position == NOT_VISIBLE_LINE)
                            break;
                        lock (__consoleWriteSyncObj) {
                            Console.SetCursorPosition(position[POSITION_LEFT], position[POSITION_TOP]);
                            Console.Write(_fixedLines[line.Key]);
                            Console.SetCursorPosition(CURSOR_ORIGIN[POSITION_LEFT], CURSOR_ORIGIN[POSITION_TOP]);
                        }
                    }
                }
            });
        }

        public void AddRollingLineAsync(string content)
        {
            Task.Factory.StartNew(() => {
                if (content == null)
                    return;

                lock (_rollingLines) {
                    _rollingLines.AddFirst(content.AdjustLength(_columnWidth));
                }
                ReprintAllRollingLinesAsync();
            });
        }

        protected void ReprintAllRollingLinesAsync()
        {
            Task.Factory.StartNew(() => {
                lock (_rollingLines) {
                    for (var rollingLineIndex = 0; rollingLineIndex < _rollingLines.Count; rollingLineIndex++) {
                        var position = GetCursorPositionForRollingLine(rollingLineIndex);
                        if (position == NOT_VISIBLE_LINE) {
                            // delete all those that are not visible
                            var firstNotVisibleIndex = rollingLineIndex;
                            string firstNotVisible;
                            do {
                                _rollingLines.RemoveLast();
                                firstNotVisible = _rollingLines.ElementAtOrDefault(firstNotVisibleIndex);
                            } while (firstNotVisible != null);
                        } else {
                            lock (__consoleWriteSyncObj) {
                                Console.SetCursorPosition(position[POSITION_LEFT], position[POSITION_TOP]);
                                Console.Write(_rollingLines.ElementAt(rollingLineIndex));
                                Console.SetCursorPosition(CURSOR_ORIGIN[POSITION_LEFT], CURSOR_ORIGIN[POSITION_TOP]);
                            }
                        }
                    }
                }
            });
        }

#warning keep this public ?????????????????????????????????

        public void ReprintEverythingAsync()
        {
            ReprintAllFixedLinesAsync();
            ReprintAllRollingLinesAsync();
        }

        protected int[] GetCursorPositionForFixedLine(string name)
            => GetCursorPosition(_fixedLines.Keys.ToList().IndexOf(name));

        protected int[] GetCursorPositionForRollingLine(int rollingLineRelativeIndex)
            => GetCursorPosition(FixedLines + rollingLineRelativeIndex + EMPTY_LINES_BETWEEN_ROLLING_AND_FIXED);

        protected int[] GetCursorPosition(int absoluteLineIndex)
        {
            if (absoluteLineIndex >= VisibleLines)
                return NOT_VISIBLE_LINE;
            var column = absoluteLineIndex / VisibleLinesPerColumn;
            var lineInColumn = absoluteLineIndex % VisibleLinesPerColumn;
            var left = Origin[POSITION_LEFT] + column * (_columnWidth + MARGIN_BETWEEN_COLUMNS);
            var top = Origin[POSITION_TOP] + 1 + lineInColumn; // the "1" is to avoid the title line
            return new int[] { left, top };
        }

        public void Unregister()
            => ConsoleSpaceManager.UnregisterSpaceUsage(Origin[POSITION_LEFT], Origin[POSITION_TOP]);

        public void CleanUp()
        {
            lock (__consoleWriteSyncObj) {
                for (var top = Origin[POSITION_TOP]; top < Origin[POSITION_TOP] + Size[SIZE_HEIGHT]; top++) {
                    Console.SetCursorPosition(Origin[POSITION_LEFT], top);
                    Console.Write(" ".AdjustLength(Size[SIZE_WIDTH]));
                }
            }
        }

        public Panel TransferLines(Panel destination)
        {
            destination._rollingLines = _rollingLines;
            destination._fixedLines = _fixedLines;
            return destination;
        }
    }
}