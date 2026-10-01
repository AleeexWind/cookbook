namespace Cookbook.Infrastructure.Persistence;

/// <summary>
/// Fixed seed identifiers for demo users and recipes.
/// </summary>
public static class SeedIds
{
    /// <summary>Alice user id.</summary>
    public static readonly Guid Alice = Guid.Parse("11111111-1111-1111-1111-111111111111");

    /// <summary>Bob user id.</summary>
    public static readonly Guid Bob = Guid.Parse("22222222-2222-2222-2222-222222222222");

    /// <summary>Pasta Carbonara recipe id.</summary>
    public static readonly Guid PastaCarbonara = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    /// <summary>Chicken Stir Fry recipe id.</summary>
    public static readonly Guid ChickenStirFry = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    /// <summary>Chocolate Cake recipe id.</summary>
    public static readonly Guid ChocolateCake = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
}
