namespace Cafe.Domain
{
    public interface IMenuItemFactory
    {
        IOrderItem CreateItem(MenuItemType type);
        IOrderItem CreateCombo(ComboType type);
    }
}
