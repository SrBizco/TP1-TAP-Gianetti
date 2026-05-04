using Cafe.Domain;
using NUnit.Framework;

namespace Cafe.Tests.EditMode
{
    public sealed class CafeDomainTests
    {
        [Test]
        public void FactoryCreatesSimpleItemsAndCompositeCombos()
        {
            var factory = new CafeMenuItemFactory();

            var cafe = factory.CreateItem(MenuItemType.Cafe);
            var combo = factory.CreateCombo(ComboType.Desayuno);

            Assert.AreEqual("Cafe", cafe.Name);
            Assert.AreEqual(1200m, cafe.Price);
            Assert.AreEqual("Combo Desayuno", combo.Name);
            Assert.AreEqual(2000m, combo.Price);
            StringAssert.Contains("Cafe", combo.Describe());
            StringAssert.Contains("Medialuna", combo.Describe());
        }

        [Test]
        public void DiscountCodeStrategyAppliesThirtyPercentDiscount()
        {
            var discountFactory = new DiscountStrategyFactory();
            var strategy = discountFactory.CreateFromCode("TAP30");

            var total = strategy.Apply(1000m);

            Assert.AreEqual("Codigo TAP30", strategy.Name);
            Assert.AreEqual(700m, total);
        }

        [Test]
        public void CreatedOrderReceivesFirstFreeEmployeeAndSequentialNumber()
        {
            var factory = new CafeMenuItemFactory();
            var manager = CreateManager(out _);
            var builder = new OrderBuilder()
                .WithCustomer("Gianetti")
                .AddItem(factory.CreateItem(MenuItemType.Cafe))
                .WithDiscount(new NoDiscountStrategy());

            var order = manager.CreateOrder(builder);

            Assert.AreEqual("Pedido 1", order.Id);
            Assert.AreEqual("Gianetti", order.CustomerName);
            Assert.AreEqual(OrderStatus.InProcess, order.Status);
            Assert.AreEqual("Maximo", order.AssignedEmployee.Name);
            Assert.IsTrue(manager.Employees[0].IsBusy);
        }

        [Test]
        public void FourthOrderWaitsUntilAnEmployeeFinishesAndThenGetsAssigned()
        {
            var factory = new CafeMenuItemFactory();
            var manager = CreateManager(out _);

            var first = manager.CreateOrder(NewOrder(factory, "Cliente 1"));
            var second = manager.CreateOrder(NewOrder(factory, "Cliente 2"));
            var third = manager.CreateOrder(NewOrder(factory, "Cliente 3"));
            var fourth = manager.CreateOrder(NewOrder(factory, "Cliente 4"));

            Assert.AreEqual("Maximo", first.AssignedEmployee.Name);
            Assert.AreEqual("Valeria", second.AssignedEmployee.Name);
            Assert.AreEqual("Martina", third.AssignedEmployee.Name);
            Assert.AreEqual(OrderStatus.PendingAssignment, fourth.Status);
            Assert.IsNull(fourth.AssignedEmployee);

            var newlyAssigned = manager.CompleteOrder(first);

            Assert.AreEqual(OrderStatus.Ready, first.Status);
            Assert.AreEqual(OrderStatus.InProcess, fourth.Status);
            Assert.AreEqual("Maximo", fourth.AssignedEmployee.Name);
            Assert.AreEqual(1, newlyAssigned.Count);
            Assert.AreSame(fourth, newlyAssigned[0]);
        }

        [Test]
        public void ReadyOrderNotifiesCustomerAndEmployee()
        {
            var factory = new CafeMenuItemFactory();
            var manager = CreateManager(out var notifications);
            var order = manager.CreateOrder(NewOrder(factory, "Ana"));

            manager.CompleteOrder(order);

            Assert.AreEqual(1, notifications.Messages.Count);
            StringAssert.Contains("Pedido 1", notifications.Messages[0]);
            StringAssert.Contains("Ana", notifications.Messages[0]);
            StringAssert.Contains("Maximo", notifications.Messages[0]);
        }

        [Test]
        public void ManagerCanRemoveReadyOrderFromVisibleList()
        {
            var factory = new CafeMenuItemFactory();
            var manager = CreateManager(out _);
            var order = manager.CreateOrder(NewOrder(factory, "Ana"));
            manager.CompleteOrder(order);

            manager.RemoveOrder(order);

            Assert.AreEqual(0, manager.Orders.Count);
        }

        [Test]
        public void NotificationLogCanRemoveMessagesForOrder()
        {
            var factory = new CafeMenuItemFactory();
            var manager = CreateManager(out var notifications);
            var order = manager.CreateOrder(NewOrder(factory, "Ana"));
            manager.CompleteOrder(order);

            notifications.RemoveMessagesForOrder(order.Id);

            Assert.AreEqual(0, notifications.Messages.Count);
        }

        private static OrderManager CreateManager(out NotificationLog notifications)
        {
            notifications = new NotificationLog();
            return new OrderManager(
                new[]
                {
                    new Employee("Maximo"),
                    new Employee("Valeria"),
                    new Employee("Martina")
                },
                notifications);
        }

        private static OrderBuilder NewOrder(CafeMenuItemFactory factory, string customer)
        {
            return new OrderBuilder()
                .WithCustomer(customer)
                .AddItem(factory.CreateItem(MenuItemType.Cafe))
                .WithDiscount(new NoDiscountStrategy());
        }
    }
}
