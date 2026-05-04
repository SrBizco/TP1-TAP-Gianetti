namespace Cafe.Domain
{
    public interface IDiscountStrategy
    {
        string Name { get; }
        decimal Apply(decimal subtotal);
    }
}
