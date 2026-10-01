namespace Cookbook.Application.Exceptions;

/// <summary>
/// Thrown when a requested entity is missing or not visible to the caller.
/// </summary>
public sealed class EntityNotFoundException : Exception
{
    /// <summary>
    /// Creates the exception.
    /// </summary>
    /// <param name="message">Error message.</param>
    public EntityNotFoundException(string message)
        : base(message)
    {
    }
}
