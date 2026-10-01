using System.Text.Json;
using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cookbook.Infrastructure.Persistence.Configurations;

internal sealed class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.ToTable("Recipes");
        builder.HasKey(r => r.Id);
        builder.Ignore(r => r.RecipeId);
        builder.Ignore(r => r.AverageRating);
        builder.Ignore(r => r.CommentsCount);
        builder.Ignore(r => r.Tags);

        builder.Property(r => r.Title).HasMaxLength(200).IsRequired();
        builder.Property(r => r.Description).HasMaxLength(4000).IsRequired();
        builder.Property(r => r.Photo).HasMaxLength(500).IsRequired();
        builder.Property(r => r.CookingTimeMinutes).IsRequired();
        builder.Property(r => r.Difficulty).HasConversion<string>().HasMaxLength(32);
        builder.Property(r => r.Visibility).HasConversion<string>().HasMaxLength(32);
        builder.Property(r => r.CreatedAt).IsRequired();

        builder.Property(r => r.AuthorId)
            .HasConversion(id => id.Value, value => new UserId(value))
            .HasColumnName("AuthorId");

        builder.Property(r => r.Category)
            .HasConversion(c => c.Value, value => new Category(value))
            .HasColumnName("Category")
            .HasMaxLength(100);

        builder.Property(r => r.BasePortions)
            .HasConversion(p => p.Value, value => new Portions(value))
            .HasColumnName("BasePortions");

        var tagsComparer = new ValueComparer<List<Tag>>(
            (left, right) => left!.SequenceEqual(right!),
            list => list.Aggregate(0, (hash, tag) => HashCode.Combine(hash, tag.GetHashCode())),
            list => list.ToList());

        builder.Property<List<Tag>>("_tags")
            .HasColumnName("TagsJson")
            .HasConversion(
                tags => JsonSerializer.Serialize(tags.Select(t => t.Value).ToList(), (JsonSerializerOptions?)null),
                json => JsonSerializer.Deserialize<List<string>>(json, (JsonSerializerOptions?)null)!
                    .Select(value => new Tag(value))
                    .ToList())
            .Metadata.SetValueComparer(tagsComparer);

        builder.Navigation(r => r.Ingredients).HasField("_ingredients").UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(r => r.Steps).HasField("_steps").UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(r => r.Ratings).HasField("_ratings").UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(r => r.Comments).HasField("_comments").UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(r => r.Ingredients, owned =>
        {
            owned.ToTable("IngredientLines");
            owned.WithOwner().HasForeignKey("RecipeId");
            owned.HasKey(i => i.Id);
            owned.Property(i => i.Id).ValueGeneratedNever();
            owned.Property(i => i.Name).HasMaxLength(200).IsRequired();
            owned.Property(i => i.Quantity).HasPrecision(18, 4).IsRequired();
            owned.Property(i => i.Unit).HasConversion<string>().HasMaxLength(32);
        });

        builder.OwnsMany(r => r.Steps, owned =>
        {
            owned.ToTable("CookingSteps");
            owned.WithOwner().HasForeignKey("RecipeId");
            owned.HasKey(s => s.Id);
            owned.Property(s => s.Id).ValueGeneratedNever();
            owned.Property(s => s.Order).IsRequired();
            owned.Property(s => s.Instruction).HasMaxLength(4000).IsRequired();
        });

        builder.OwnsMany(r => r.Ratings, owned =>
        {
            owned.ToTable("Ratings");
            owned.WithOwner().HasForeignKey("RecipeId");
            owned.HasKey(x => x.Id);
            owned.Property(x => x.Id).ValueGeneratedNever();
            owned.Property(x => x.Stars).IsRequired();
            owned.Property(x => x.UserId)
                .HasConversion(id => id.Value, value => new UserId(value));
        });

        builder.OwnsMany(r => r.Comments, owned =>
        {
            owned.ToTable("Comments");
            owned.WithOwner().HasForeignKey("RecipeId");
            owned.HasKey(c => c.Id);
            owned.Property(c => c.Id).ValueGeneratedNever();
            owned.Property(c => c.Text).HasMaxLength(4000).IsRequired();
            owned.Property(c => c.PostedAt).IsRequired();
            owned.Property(c => c.AuthorId)
                .HasConversion(id => id.Value, value => new UserId(value));
        });
    }
}
