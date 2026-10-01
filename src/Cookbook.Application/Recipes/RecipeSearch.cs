using Cookbook.Domain.Recipes;

namespace Cookbook.Application.Recipes;

internal static class RecipeSearch
{
    public static bool Matches(Recipe recipe, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return true;
        }

        var term = search.Trim();
        if (recipe.Title.Contains(term, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (recipe.Tags.Any(t => t.Value.Contains(term, StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        var tokens = term.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (tokens.Length == 0)
        {
            return true;
        }

        return tokens.All(token =>
            recipe.Ingredients.Any(i => i.Name.Contains(token, StringComparison.OrdinalIgnoreCase)));
    }
}
