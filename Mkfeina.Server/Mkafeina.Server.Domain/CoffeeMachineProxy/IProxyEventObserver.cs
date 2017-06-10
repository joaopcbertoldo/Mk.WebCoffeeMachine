namespace Mkafeina.Server.Domain.CoffeeMachineProxy
{
	public interface IProxyEventObserver
	{
		void Notify(ProxyEventEnum action);
	}
}