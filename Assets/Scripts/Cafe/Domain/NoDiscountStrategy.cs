namespace Cafe.Domain
{
    public sealed class NoDiscountStrategy : IDiscountStrategy
    {
        public string Name => "Sin descuento";

        public decimal Apply(decimal subtotal)
        {
            return subtotal;
        }
    }
}
