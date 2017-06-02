using Mkafeina.Domain.Dashboard;

namespace Mkafeina.Server
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

		public Dashboard() : base()
		{
		}
	}
}