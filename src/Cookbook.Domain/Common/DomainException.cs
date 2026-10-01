namespace Cookbook.Domain.Common;

/// <summary>
/// Thrown when a domain invariant is violated.
/// </summary>
public sealed class DomainException : Exception
{
    /// <summary>
    /// Creates a domain exception with the given message.
    /// </summary>
    /// <param name="message">Explanation of the violated invariant.</param>
    public DomainException(string message)
        : base(message)
    {
    }
}
