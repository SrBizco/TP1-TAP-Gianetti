using System.Collections.Generic;
using System.Linq;

namespace Cafe.Domain
{
    public sealed class OrderManager
    {
        private readonly Queue<Order> pendingOrders = new Queue<Order>();
        private readonly List<Order> orders = new List<Order>();
        private readonly List<Employee> employees;
        private readonly IOrderObserver observer;
        private int nextOrderNumber = 1;

        public OrderManager(IEnumerable<Employee> employees, IOrderObserver observer)
        {
            this.employees = employees.ToList();
            this.observer = observer;
        }

        public IReadOnlyList<Order> Orders => orders;
        public IReadOnlyList<Employee> Employees => employees;

        public Order CreateOrder(OrderBuilder builder)
        {
            var order = builder.Build(nextOrderNumber);
            nextOrderNumber++;
            order.Attach(observer);
            orders.Add(order);
            TryAssign(order);
            return order;
        }

        public IReadOnlyList<Order> CompleteOrder(Order order)
        {
            if (order.Status != OrderStatus.InProcess)
            {
                return new List<Order>();
            }

            order.MarkReady();
            return AssignPendingOrders();
        }

        public void RemoveOrder(Order order)
        {
            orders.Remove(order);
        }

        private void TryAssign(Order order)
        {
            var freeEmployee = employees.FirstOrDefault(employee => !employee.IsBusy);
            if (freeEmployee == null)
            {
                pendingOrders.Enqueue(order);
                return;
            }

            order.AssignTo(freeEmployee);
        }

        private IReadOnlyList<Order> AssignPendingOrders()
        {
            var assigned = new List<Order>();

            while (pendingOrders.Count > 0)
            {
                var freeEmployee = employees.FirstOrDefault(employee => !employee.IsBusy);
                if (freeEmployee == null)
                {
                    break;
                }

                var order = pendingOrders.Dequeue();
                order.AssignTo(freeEmployee);
                assigned.Add(order);
            }

            return assigned;
        }
    }
}
