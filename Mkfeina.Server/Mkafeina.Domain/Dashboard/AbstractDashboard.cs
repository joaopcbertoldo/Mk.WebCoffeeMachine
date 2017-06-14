using Microsoft.Practices.Unity;
using Mkafeina.Domain.Dashboard.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mkafeina.Domain.Dashboard
{
	public abstract class AbstractDashboard
	{
		private const string LOG = "log";

		protected long _logCounter = 0;

		protected Dictionary<string, Panel> _panels;

		public string Title { set { Console.Title = value; } }

		public IEnumerable<string> PanelsNames { get => _panels.Keys.AsEnumerable(); }

		protected AbstractDashboard()
		{
			_panels = new Dictionary<string, Panel>();

			var commandInterpreter = AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractCommandInterpreter>();
			Task.Factory.StartNew(() =>
			{
				while (true)
					commandInterpreter.HandleCommand(Console.ReadKey(intercept: true));
			});
		}

		public void CreateFixedPanels(IDictionary<string, PanelConfig> panelsConfigs)
			=> panelsConfigs.ToList().ForEach(kv => _panels.Add(kv.Key, new Panel(kv.Value)));

		public void CreateDynamicPanel(string panelName, PanelConfig panelConfigs)
			=> _panels.Add(panelName, new Panel(panelConfigs));

		public void DeleteDynamicPanel(string uniqueName)
			=> _panels.Remove(uniqueName);

		public void AddFixedLinesToFixedPanels(IDictionary<string, IEnumerable<string>> panelsFixedLines)
			=> _panels.ToList().ForEach(kv => kv.Value.AddFixedLines(panelsFixedLines[kv.Key]));

		public void AddFixedLinesToDynamicPanel(string panelName, IEnumerable<string> panelFixedLines)
			=> _panels[panelName].AddFixedLines(panelFixedLines);

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

		public void ReprintEverythingAsync()
		{
			_panels.ToList().ForEach(kv => kv.Value.CleanUp());
			_panels.ToList().ForEach(kv => kv.Value.ReprintEverythingAsync());
		}
	}
}