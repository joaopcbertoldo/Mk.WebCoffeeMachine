using Microsoft.Practices.Unity;
using Mkafeina.Domain.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Mkafeina.Domain
{
	public abstract class Dashboard
	{
		[DllImport("kernel32.dll", ExactSpelling = true)]
		private static extern IntPtr GetConsoleWindow();

		private static IntPtr ThisConsole = GetConsoleWindow();

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		private const string LOG = "log";

		protected long _logCounter = 0;

		protected Dictionary<string, Panel> _panels = new Dictionary<string, Panel>();
		protected CommandInterpreter _commandInterpreter;

		public string Title { set { Console.Title = value; } }

		public IEnumerable<string> PanelsNames { get => _panels.Keys.AsEnumerable(); }

		protected Dashboard()
		{
			ShowWindow(ThisConsole, 3);
			_commandInterpreter = AppDomain.CurrentDomain.UnityContainer().Resolve<CommandInterpreter>();
			var task = _commandInterpreter.KeyListenerTask; // gambiarrinha
		}

		public void CreatePanels(IDictionary<string, PanelConfig> panelsConfigs)
			=> panelsConfigs.ToList().ForEach(kv => _panels.Add(kv.Key, Panel.Factory.CreatePanel(kv.Value)));

		public void AddFixedLinesToPanels(IDictionary<string, IEnumerable<string>> panelsLinesCollections)
			=> _panels.ToList().ForEach(kv => kv.Value.AddManyFixedLinesAsync(panelsLinesCollections[kv.Key], wait: true));

		public Action<string> UpdateEventHandler(string panelName)
			=> _panels[panelName].UpdateEventHandler;

		public void ReloadAllPanelsAsync(IDictionary<string, PanelConfig> newConfigs)
			=> Task.Factory.StartNew(() =>
			{
				lock (this)
				{
					lock (_panels)
					{
						_panels.ToList().ForEach(kv =>
						{
							kv.Value.Unregister();
							kv.Value.CleanUp();
						});
						_panels.ToList().ForEach(kv => kv.Value.UpdateConfig(newConfigs[kv.Key]));
					}
				}
			});

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