namespace Cafe.Domain
{
    public interface IOrderItem
    {
        string Name { get; }
        decimal Price { get; }
        string Describe();
    }
}
