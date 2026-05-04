using System;
using System.Collections;
using System.Collections.Generic;
using Cafe.Domain;
using UnityEngine;

namespace Cafe.Application
{
    public sealed class CafeOrderSystem : MonoBehaviour
    {
        [SerializeField] private float preparationSeconds = 5f;
        [SerializeField] private float readyVisibleSeconds = 4f;
        [SerializeField] private float notificationVisibleSeconds = 4f;

        private readonly HashSet<Order> processingOrders = new HashSet<Order>();
        private CafeMenuItemFactory itemFactory;
        private DiscountStrategyFactory discountFactory;
        private OrderBuilder currentBuilder;
        private NotificationLog notificationLog;
        private OrderManager orderManager;
        private string currentCustomerName = string.Empty;
        private string currentDiscountCode = string.Empty;
        private bool initialized;

        public event Action Changed;

        public OrderBuilder CurrentBuilder
        {
            get
            {
                EnsureInitialized();
                return currentBuilder;
            }
        }

        public IReadOnlyList<Order> Orders
        {
            get
            {
                EnsureInitialized();
                return orderManager.Orders;
            }
        }

        public IReadOnlyList<Employee> Employees
        {
            get
            {
                EnsureInitialized();
                return orderManager.Employees;
            }
        }

        public IReadOnlyList<string> Notifications
        {
            get
            {
                EnsureInitialized();
                return notificationLog.Messages;
            }
        }

        private void Awake()
        {
            EnsureInitialized();
        }

        private void EnsureInitialized()
        {
            if (initialized)
            {
                return;
            }

            itemFactory = new CafeMenuItemFactory();
            discountFactory = new DiscountStrategyFactory();
            currentBuilder = new OrderBuilder();
            notificationLog = new NotificationLog();
            orderManager = new OrderManager(
                new[]
                {
                    new Employee("Maximo"),
                    new Employee("Valeria"),
                    new Employee("Martina")
                },
                notificationLog);

            initialized = true;
        }

        public void SetCustomerName(string customerName)
        {
            EnsureInitialized();
            currentCustomerName = customerName;
            currentBuilder.WithCustomer(currentCustomerName);
            NotifyChanged();
        }

        public void SetDiscountCode(string code)
        {
            EnsureInitialized();
            currentDiscountCode = code;
            NotifyChanged();
        }

        public void ApplyDiscountCode(string code)
        {
            EnsureInitialized();
            currentDiscountCode = code;
            currentBuilder.WithDiscount(discountFactory.CreateFromCode(currentDiscountCode));
            NotifyChanged();
        }

        public void AddCafe()
        {
            AddItem(MenuItemType.Cafe);
        }

        public void AddTe()
        {
            AddItem(MenuItemType.Te);
        }

        public void AddMedialuna()
        {
            AddItem(MenuItemType.Medialuna);
        }

        public void AddTostado()
        {
            AddItem(MenuItemType.Tostado);
        }

        public void AddComboDesayuno()
        {
            AddCombo(ComboType.Desayuno);
        }

        public void AddComboMerienda()
        {
            AddCombo(ComboType.Merienda);
        }

        public void AddItem(MenuItemType type)
        {
            EnsureInitialized();
            currentBuilder.AddItem(itemFactory.CreateItem(type));
            NotifyChanged();
        }

        public void AddCombo(ComboType type)
        {
            EnsureInitialized();
            currentBuilder.AddItem(itemFactory.CreateCombo(type));
            NotifyChanged();
        }

        public void CreateOrder()
        {
            EnsureInitialized();
            if (currentBuilder.Items.Count == 0)
            {
                return;
            }

            currentBuilder.WithCustomer(currentCustomerName);

            var order = orderManager.CreateOrder(currentBuilder);
            currentDiscountCode = string.Empty;
            currentBuilder = new OrderBuilder().WithCustomer(currentCustomerName);

            StartProcessingIfNeeded(order);
            NotifyChanged();
        }

        public void ClearCurrentOrder()
        {
            EnsureInitialized();
            currentBuilder.Clear();
            currentBuilder.WithCustomer(currentCustomerName);
            currentBuilder.WithDiscount(discountFactory.CreateFromCode(currentDiscountCode));
            NotifyChanged();
        }

        public void CompleteOrderManually(int orderNumber)
        {
            EnsureInitialized();
            foreach (var order in orderManager.Orders)
            {
                if (order.Number == orderNumber)
                {
                    CompleteOrder(order);
                    return;
                }
            }
        }

        private void StartProcessingIfNeeded(Order order)
        {
            if (order.Status == OrderStatus.InProcess && !processingOrders.Contains(order))
            {
                processingOrders.Add(order);
                StartCoroutine(CompleteAfterDelay(order));
            }
        }

        private IEnumerator CompleteAfterDelay(Order order)
        {
            yield return new WaitForSeconds(preparationSeconds);
            CompleteOrder(order);
        }

        private void CompleteOrder(Order order)
        {
            processingOrders.Remove(order);
            var newlyAssigned = orderManager.CompleteOrder(order);

            foreach (var assignedOrder in newlyAssigned)
            {
                StartProcessingIfNeeded(assignedOrder);
            }

            StartCoroutine(RemoveReadyOrderAfterDelay(order));
            StartCoroutine(RemoveNotificationsAfterDelay(order));
            NotifyChanged();
        }

        private IEnumerator RemoveReadyOrderAfterDelay(Order order)
        {
            yield return new WaitForSeconds(readyVisibleSeconds);
            orderManager.RemoveOrder(order);
            NotifyChanged();
        }

        private IEnumerator RemoveNotificationsAfterDelay(Order order)
        {
            yield return new WaitForSeconds(notificationVisibleSeconds);
            notificationLog.RemoveMessagesForOrder(order.Id);
            NotifyChanged();
        }

        private void NotifyChanged()
        {
            Changed?.Invoke();
        }
    }
}
