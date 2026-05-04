namespace Cafe.Domain
{
    public sealed class DiscountCodeStrategy : IDiscountStrategy
    {
        private const decimal DiscountMultiplier = 0.70m;

        public string Name => "Codigo TAP30";

        public decimal Apply(decimal subtotal)
        {
            return subtotal * DiscountMultiplier;
        }
    }
}
