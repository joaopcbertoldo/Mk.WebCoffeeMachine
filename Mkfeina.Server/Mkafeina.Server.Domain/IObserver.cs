namespace Mkfeina.Server.Domain
{
    public interface IObserver<T>
    {
        void Notify(T notifier);
    }
}