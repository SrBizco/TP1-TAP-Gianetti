namespace Cafe.Domain
{
    public sealed class DiscountStrategyFactory
    {
        public const string ValidCode = "TAP30";

        public IDiscountStrategy CreateFromCode(string code)
        {
            if (!string.IsNullOrWhiteSpace(code) && code.Trim().ToUpperInvariant() == ValidCode)
            {
                return new DiscountCodeStrategy();
            }

            return new NoDiscountStrategy();
        }
    }
}
