namespace Cafe.Domain
{
    public interface IOrderObserver
    {
        void OnOrderReady(Order order);
    }
}
