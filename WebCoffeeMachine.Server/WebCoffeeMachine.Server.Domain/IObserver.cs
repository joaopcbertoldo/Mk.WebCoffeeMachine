namespace WebCoffeeMachine.Server.Domain
{
    public interface IObserver<T>
    {
        void Notify(T notifier);
    }
}