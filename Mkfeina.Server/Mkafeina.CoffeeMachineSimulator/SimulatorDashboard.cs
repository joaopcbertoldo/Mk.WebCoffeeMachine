using Mkafeina.Domain.Dashboard;

namespace Mkafeina.Simulator
{
	public class SimulatorDashboard : AbstractDashboard
	{
		private static SimulatorDashboard __singleton;

		public static SimulatorDashboard Singleton {
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