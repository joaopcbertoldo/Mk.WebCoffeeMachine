namespace Mkafeina.Domain.Dashboard.Panels
{
	public abstract class AbstractPanelLineBuilder
	{
		public abstract string BuildOrUpdate(string lineName, object caller = null);
	}
}