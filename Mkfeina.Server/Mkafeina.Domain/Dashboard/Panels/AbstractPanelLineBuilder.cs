namespace Mkafeina.Domain.Dashboard.Panels
{
	public abstract class AbstractPanelLineBuilder
	{
		public abstract string Build(string lineName);
		
		public abstract string UpdateEventHandler(string lineName, object caller);
	}
}