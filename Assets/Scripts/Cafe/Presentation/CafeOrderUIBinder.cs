using System.Linq;
using Cafe.Application;
using Cafe.Domain;
using TMPro;
using UnityEngine;

namespace Cafe.Presentation
{
    public sealed class CafeOrderUIBinder : MonoBehaviour
    {
        [Header("System")]
        [SerializeField] private CafeOrderSystem orderSystem;

        [Header("Cliente")]
        [SerializeField] private TMP_InputField customerNameInput;
        [SerializeField] private TMP_InputField discountCodeInput;
        [SerializeField] private TMP_Dropdown productDropdown;
        [SerializeField] private TMP_Dropdown comboDropdown;
        [SerializeField] private TMP_Text currentOrderText;

        [Header("Local")]
        [SerializeField] private TMP_Text localStatusText;
        [SerializeField] private TMP_Text notificationsText;

        private void Awake()
        {
            if (orderSystem == null)
            {
                orderSystem = FindFirstObjectByType<CafeOrderSystem>();
            }
        }

        private void OnEnable()
        {
            if (orderSystem != null)
            {
                orderSystem.Changed += Refresh;
            }

            ConfigureDropdowns();
            Refresh();
        }

        private void OnDisable()
        {
            if (orderSystem != null)
            {
                orderSystem.Changed -= Refresh;
            }
        }

        public void OnCustomerNameChanged()
        {
            if (orderSystem == null)
            {
                return;
            }

            orderSystem.SetCustomerName(customerNameInput != null ? customerNameInput.text : string.Empty);
        }

        public void OnDiscountCodeChanged()
        {
            if (orderSystem == null)
            {
                return;
            }

            orderSystem.SetDiscountCode(discountCodeInput != null ? discountCodeInput.text : string.Empty);
        }

        public void ApplyDiscountCode()
        {
            if (orderSystem == null)
            {
                return;
            }

            orderSystem.ApplyDiscountCode(discountCodeInput != null ? discountCodeInput.text : string.Empty);
            Refresh();
        }

        public void AddSelectedProduct()
        {
            if (orderSystem == null || productDropdown == null)
            {
                return;
            }

            orderSystem.AddItem((MenuItemType)productDropdown.value);
        }

        public void AddSelectedCombo()
        {
            if (orderSystem == null || comboDropdown == null)
            {
                return;
            }

            orderSystem.AddCombo((ComboType)comboDropdown.value);
        }

        public void CreateOrder()
        {
            if (orderSystem == null)
            {
                return;
            }

            OnCustomerNameChanged();
            orderSystem.CreateOrder();
            if (discountCodeInput != null)
            {
                discountCodeInput.text = string.Empty;
            }

            Refresh();
        }

        public void ClearCurrentOrder()
        {
            if (orderSystem == null)
            {
                return;
            }

            orderSystem.ClearCurrentOrder();
            Refresh();
        }

        private void ConfigureDropdowns()
        {
            if (productDropdown != null && productDropdown.options.Count == 0)
            {
                productDropdown.AddOptions(new[]
                {
                    "Cafe - $1200",
                    "Te - $1000",
                    "Medialuna - $800",
                    "Tostado - $1800"
                }.ToList());
            }

            if (comboDropdown != null && comboDropdown.options.Count == 0)
            {
                comboDropdown.AddOptions(new[]
                {
                    "Combo Desayuno - $2000",
                    "Combo Merienda - $3600"
                }.ToList());
            }
        }

        private void Refresh()
        {
            if (orderSystem == null)
            {
                return;
            }

            RefreshCurrentOrder();
            RefreshLocalStatus();
            RefreshNotifications();
        }

        private void RefreshCurrentOrder()
        {
            if (currentOrderText == null)
            {
                return;
            }

            var builder = orderSystem.CurrentBuilder;
            if (builder.Items.Count == 0)
            {
                currentOrderText.text = "Pedido actual: sin items";
                return;
            }

            currentOrderText.text =
                "Pedido actual\n" +
                string.Join("\n", builder.Items.Select(item => "- " + item.Describe())) +
                $"\nSubtotal: ${builder.Subtotal:0.00}" +
                $"\nDescuento: {builder.DiscountStrategy.Name}" +
                $"\nMonto descontado: ${builder.Subtotal - builder.Total:0.00}" +
                $"\nTotal: ${builder.Total:0.00}";
        }

        private void RefreshLocalStatus()
        {
            if (localStatusText == null)
            {
                return;
            }

            if (orderSystem.Orders.Count == 0)
            {
                localStatusText.text = "Local\nPedidos creados: ninguno";
                return;
            }

            var orders = string.Join("\n", orderSystem.Orders.Select(DescribeOrder));

            localStatusText.text = $"Local\n{orders}";
        }

        private void RefreshNotifications()
        {
            if (notificationsText == null)
            {
                return;
            }

            notificationsText.text = orderSystem.Notifications.Count == 0
                ? "Notificaciones: ninguna"
                : "Notificaciones\n" + string.Join("\n", orderSystem.Notifications);
        }

        private static string DescribeOrder(Order order)
        {
            var employeeName = order.AssignedEmployee != null ? order.AssignedEmployee.Name : "Sin asignar";
            return $"{order.Id} | Cliente: {order.CustomerName} | Estado: {FormatStatus(order.Status)} | Empleado: {employeeName} | Total: ${order.Total:0.00}";
        }

        private static string FormatStatus(OrderStatus status)
        {
            switch (status)
            {
                case OrderStatus.PendingAssignment:
                    return "Pendiente de asignacion";
                case OrderStatus.InProcess:
                    return "En proceso";
                case OrderStatus.Ready:
                    return "Pedido listo";
                default:
                    return status.ToString();
            }
        }
    }
}
