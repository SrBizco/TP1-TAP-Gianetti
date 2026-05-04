namespace Cafe.Domain
{
    public sealed class CafeMenuItemFactory : IMenuItemFactory
    {
        public IOrderItem CreateItem(MenuItemType type)
        {
            switch (type)
            {
                case MenuItemType.Cafe:
                    return new SimpleMenuItem("Cafe", 1200m);
                case MenuItemType.Te:
                    return new SimpleMenuItem("Te", 1000m);
                case MenuItemType.Medialuna:
                    return new SimpleMenuItem("Medialuna", 800m);
                case MenuItemType.Tostado:
                    return new SimpleMenuItem("Tostado", 1800m);
                default:
                    return new SimpleMenuItem("Producto desconocido", 0m);
            }
        }

        public IOrderItem CreateCombo(ComboType type)
        {
            switch (type)
            {
                case ComboType.Desayuno:
                    return new ComboMenuItem(
                        "Combo Desayuno",
                        new[] { CreateItem(MenuItemType.Cafe), CreateItem(MenuItemType.Medialuna) });
                case ComboType.Merienda:
                    return new ComboMenuItem(
                        "Combo Merienda",
                        new[] { CreateItem(MenuItemType.Te), CreateItem(MenuItemType.Tostado), CreateItem(MenuItemType.Medialuna) });
                default:
                    return new ComboMenuItem("Combo desconocido", new IOrderItem[0]);
            }
        }
    }
}
