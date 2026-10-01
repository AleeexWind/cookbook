using Cookbook.Domain.Favourites;
using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cookbook.Infrastructure.Persistence.Configurations;

internal sealed class FavouriteConfiguration : IEntityTypeConfiguration<Favourite>
{
    public void Configure(EntityTypeBuilder<Favourite> builder)
    {
        builder.ToTable("Favourites");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.UserId)
            .HasConversion(id => id.Value, value => new UserId(value))
            .IsRequired();

        builder.Property(f => f.RecipeId)
            .HasConversion(id => id.Value, value => new RecipeId(value))
            .IsRequired();

        builder.Property(f => f.IsActive).IsRequired();

        builder.HasIndex(f => new { f.UserId, f.RecipeId }).IsUnique();
    }
}
