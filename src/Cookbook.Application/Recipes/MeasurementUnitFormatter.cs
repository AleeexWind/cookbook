using Cookbook.Domain.Recipes;

namespace Cookbook.Application.Recipes;

internal static class MeasurementUnitFormatter
{
    public static string ToLabel(MeasurementUnit unit) => unit switch
    {
        MeasurementUnit.Grams => "g",
        MeasurementUnit.Units => "units",
        MeasurementUnit.Milliliters => "ml",
        _ => unit.ToString().ToLowerInvariant()
    };
}
