namespace Cookbook.Domain.Recipes;

/// <summary>
/// How an ingredient quantity is measured.
/// </summary>
public enum MeasurementUnit
{
    /// <summary>
    /// Grams.
    /// </summary>
    Grams = 0,

    /// <summary>
    /// Discrete pieces.
    /// </summary>
    Units = 1,

    /// <summary>
    /// Milliliters.
    /// </summary>
    Milliliters = 2
}
