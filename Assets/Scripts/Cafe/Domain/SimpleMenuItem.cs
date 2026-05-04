namespace Cafe.Domain
{
    public sealed class SimpleMenuItem : IOrderItem
    {
        public SimpleMenuItem(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        public string Name { get; }
        public decimal Price { get; }

        public string Describe()
        {
            return $"{Name} (${Price:0.00})";
        }
    }
}
