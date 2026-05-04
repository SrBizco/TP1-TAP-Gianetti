using System.Collections.Generic;
using System.Linq;

namespace Cafe.Domain
{
    public sealed class ComboMenuItem : IOrderItem
    {
        private readonly List<IOrderItem> items;

        public ComboMenuItem(string name, IEnumerable<IOrderItem> items)
        {
            Name = name;
            this.items = items.ToList();
        }

        public string Name { get; }
        public IReadOnlyList<IOrderItem> Items => items;
        public decimal Price => items.Sum(item => item.Price);

        public string Describe()
        {
            return $"{Name} ({string.Join(" + ", items.Select(item => item.Name))}) (${Price:0.00})";
        }
    }
}
