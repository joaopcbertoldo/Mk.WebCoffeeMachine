using Mkafeina.Domain.Dashboard;

namespace Mkafeina.Simulator
{
	public class Dashboard : AbstractDashboard
	{
		private static Dashboard __sgt;

		public static Dashboard Sgt {
			get {
				if (__sgt == null)
					__sgt = new Dashboard();
				return __sgt;
			}
		}

		private Dashboard() : base()
		{
		}
	}
}