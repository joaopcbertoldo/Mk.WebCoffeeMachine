using Microsoft.Practices.Unity;
using Mkafeina.Domain.Dashboard.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Mkafeina.Domain.Dashboard
{
	public abstract class AbstractDashboard
	{
		#region Maximize Window Stuff

		[DllImport("kernel32.dll", ExactSpelling = true)]
		private static extern IntPtr GetConsoleWindow();

		private static IntPtr ThisConsole = GetConsoleWindow();

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		#endregion Maximize Window Stuff

		private const string LOG = "log";

		protected long _logCounter = 0;

		protected Dictionary<string, Panel> _panels;

		public string Title { set { Console.Title = value; } }

		public IEnumerable<string> PanelsNames { get => _panels.Keys.AsEnumerable(); }

		protected AbstractDashboard()
		{
			_panels = new Dictionary<string, Panel>();
			ShowWindow(ThisConsole, 3); //  maximize window
										// start listening keys
			var commandInterpreter = AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractCommandInterpreter>();
			Task.Factory.StartNew(() =>
			{
				while (true)
					commandInterpreter.HandleCommand(Console.ReadKey(intercept: true));
			});
		}

		public void CreatePanels(IDictionary<string, PanelConfig> panelsConfigs)
			=> panelsConfigs.ToList().ForEach(kv => _panels.Add(kv.Key, new Panel(kv.Value)));

		public void AddFixedLinesToPanels(IDictionary<string, IEnumerable<string>> panelsLinesCollections)
			=> _panels.ToList().ForEach(kv => kv.Value.AddFixedLinesAsync(panelsLinesCollections[kv.Key], wait: true));

		public Action<string, object> UpdateEventHandlerOfPanel(string panelName)
			=> _panels[panelName].UpdateEventHandler;

#warning fazer o log jogar em BD

		public void LogAsync(string message)
		{
			Task.Factory.StartNew(() =>
			{
				if (_panels != null && _panels.ContainsKey(LOG))
					_panels[LOG].AddRollingLineAsync(message.ToLogMessage(++_logCounter));
			});
		}

		public void ReprintAllPanelsAsync()
			=> _panels.ToList().ForEach(kv => kv.Value.ReprintEverythingAsync());
	}
}