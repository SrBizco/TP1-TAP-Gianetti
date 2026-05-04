using System.Collections.Generic;

namespace Cafe.Domain
{
    public sealed class OrderBuilder
    {
        private readonly List<IOrderItem> items = new List<IOrderItem>();
        private string customerName;
        private IDiscountStrategy discountStrategy = new NoDiscountStrategy();

        public IReadOnlyList<IOrderItem> Items => items;
        public string CustomerName => customerName;
        public IDiscountStrategy DiscountStrategy => discountStrategy;
        public decimal Subtotal
        {
            get
            {
                decimal subtotal = 0m;
                foreach (var item in items)
                {
                    subtotal += item.Price;
                }

                return subtotal;
            }
        }

        public decimal Total => discountStrategy.Apply(Subtotal);

        public OrderBuilder WithCustomer(string name)
        {
            customerName = name;
            return this;
        }

        public OrderBuilder AddItem(IOrderItem item)
        {
            items.Add(item);
            return this;
        }

        public OrderBuilder WithDiscount(IDiscountStrategy strategy)
        {
            discountStrategy = strategy;
            return this;
        }

        public Order Build(int number)
        {
            return new Order(number, customerName, items, discountStrategy);
        }

        public void Clear()
        {
            customerName = string.Empty;
            items.Clear();
            discountStrategy = new NoDiscountStrategy();
        }
    }
}
