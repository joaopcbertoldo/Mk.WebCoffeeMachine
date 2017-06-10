namespace Mkafeina.Server.Domain.CoffeeMachineProxy
{
	public interface IProxyEventObservable
	{
		void Subscribe(IProxyEventObserver observer);
	}
}