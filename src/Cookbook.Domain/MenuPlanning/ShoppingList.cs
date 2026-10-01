namespace Cookbook.Domain.MenuPlanning;

/// <summary>
/// An aggregated list of ingredient quantities derived from a menu plan.
/// </summary>
public sealed class ShoppingList
{
    /// <summary>
    /// Gets the shopping list items.
    /// </summary>
    public IReadOnlyList<ShoppingListItem> Items { get; }

    /// <summary>
    /// Creates a shopping list from items.
    /// </summary>
    /// <param name="items">Aggregated items.</param>
    public ShoppingList(IEnumerable<ShoppingListItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        Items = items.ToList();
    }

    /// <summary>
    /// Gets whether the list has any items.
    /// </summary>
    public bool HasItems => Items.Count > 0;
}
