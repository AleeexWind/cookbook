using Cookbook.Domain.Common;

namespace Cookbook.Domain.Recipes;

/// <summary>
/// An ordered instruction that describes how to prepare the recipe.
/// </summary>
public sealed class CookingStep : Entity
{
    /// <summary>
    /// Gets the step order starting from 1.
    /// </summary>
    public int Order { get; private set; }

    /// <summary>
    /// Gets the instruction text.
    /// </summary>
    public string Instruction { get; private set; }

    internal CookingStep(Guid id, int order, string instruction)
        : base(id)
    {
        Order = ValidateOrder(order);
        Instruction = NormalizeInstruction(instruction);
    }

    /// <summary>
    /// Creates a cooking step.
    /// </summary>
    /// <param name="order">Step order starting from 1.</param>
    /// <param name="instruction">Instruction text.</param>
    /// <returns>A new cooking step.</returns>
    public static CookingStep Create(int order, string instruction)
        => new(Guid.NewGuid(), order, instruction);

    private static int ValidateOrder(int order)
    {
        if (order < 1)
        {
            throw new DomainException("Cooking step order must be at least 1.");
        }

        return order;
    }

    private static string NormalizeInstruction(string instruction)
    {
        if (string.IsNullOrWhiteSpace(instruction))
        {
            throw new DomainException("Cooking step instruction cannot be empty.");
        }

        return instruction.Trim();
    }
}
