using System.Collections.Generic;

namespace Cafe.Domain
{
    public sealed class NotificationLog : IOrderObserver
    {
        private readonly List<string> messages = new List<string>();

        public IReadOnlyList<string> Messages => messages;

        public void OnOrderReady(Order order)
        {
            var employeeName = order.AssignedEmployee != null ? order.AssignedEmployee.Name : "sin empleado";
            messages.Add($"{order.Id} listo: se notifico a {order.CustomerName} y al empleado {employeeName}.");
        }

        public void RemoveMessagesForOrder(string orderId)
        {
            messages.RemoveAll(message => message.StartsWith(orderId));
        }
    }
}
