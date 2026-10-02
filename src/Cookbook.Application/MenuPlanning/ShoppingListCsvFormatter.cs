using System.Globalization;
using System.Text;

namespace Cookbook.Application.MenuPlanning;

/// <summary>
/// Formats a shopping list as CSV with Ingredient, Quantity, Unit columns.
/// </summary>
public static class ShoppingListCsvFormatter
{
    /// <summary>
    /// Formats shopping list items as CSV text.
    /// </summary>
    /// <param name="items">Shopping list items.</param>
    /// <returns>CSV content including header.</returns>
    public static string Format(IReadOnlyList<ShoppingListItemDto> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        var builder = new StringBuilder();
        builder.AppendLine("Ingredient,Quantity,Unit");
        foreach (var item in items)
        {
            builder.Append(Escape(item.Ingredient));
            builder.Append(',');
            builder.Append(FormatQuantity(item.Quantity));
            builder.Append(',');
            builder.AppendLine(Escape(item.Unit));
        }

        return builder.ToString();
    }

    private static string FormatQuantity(decimal quantity)
    {
        var formatted = quantity.ToString("0.####", CultureInfo.InvariantCulture);
        return formatted;
    }

    private static string Escape(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }
}
