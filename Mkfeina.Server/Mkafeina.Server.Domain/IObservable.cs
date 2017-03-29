namespace Mkfeina.Server.Domain
{
    public interface IObservable<T>
    {
        void RegisterObserver(IObserver<T> newObserver);

        void NotifyObservers();
    }
}