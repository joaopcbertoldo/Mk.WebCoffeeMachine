using Mkfeina.Domain;

namespace Mkfeina.Simulator
{
	public class SimulatorDashboard : Dashboard
	{
		private static Dashboard __singleton;

		public static Dashboard Singleton {
			get {
				if (__singleton == null)
					__singleton = new SimulatorDashboard();
				return __singleton;
			}
		}

		public SimulatorDashboard() : base()
		{
		}
	}
}