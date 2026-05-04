using System.Collections.Generic;
using System.Linq;

namespace Cafe.Domain
{
    public sealed class Order
    {
        private readonly List<IOrderItem> items;
        private readonly List<IOrderObserver> observers = new List<IOrderObserver>();

        public Order(int number, string customerName, IEnumerable<IOrderItem> items, IDiscountStrategy discountStrategy)
        {
            Number = number;
            Id = $"Pedido {number}";
            CustomerName = string.IsNullOrWhiteSpace(customerName) ? "Cliente sin nombre" : customerName.Trim();
            this.items = items.ToList();
            DiscountStrategy = discountStrategy;
            Status = OrderStatus.PendingAssignment;
        }

        public int Number { get; }
        public string Id { get; }
        public string CustomerName { get; }
        public IReadOnlyList<IOrderItem> Items => items;
        public IDiscountStrategy DiscountStrategy { get; }
        public Employee AssignedEmployee { get; private set; }
        public OrderStatus Status { get; private set; }
        public decimal Subtotal => items.Sum(item => item.Price);
        public decimal Total => DiscountStrategy.Apply(Subtotal);
        public decimal DiscountAmount => Subtotal - Total;

        public void Attach(IOrderObserver observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }
        }

        public void AssignTo(Employee employee)
        {
            AssignedEmployee = employee;
            AssignedEmployee.Assign();
            Status = OrderStatus.InProcess;
        }

        public void MarkReady()
        {
            Status = OrderStatus.Ready;
            AssignedEmployee?.Release();

            foreach (var observer in observers)
            {
                observer.OnOrderReady(this);
            }
        }

        public string DescribeItems()
        {
            return string.Join(", ", items.Select(item => item.Describe()));
        }
    }
}
