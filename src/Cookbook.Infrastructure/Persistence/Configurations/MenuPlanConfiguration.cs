using Cookbook.Domain.Common;
using Cookbook.Domain.MenuPlanning;
using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cookbook.Infrastructure.Persistence.Configurations;

internal sealed class MenuPlanConfiguration : IEntityTypeConfiguration<MenuPlan>
{
    public void Configure(EntityTypeBuilder<MenuPlan> builder)
    {
        builder.ToTable("MenuPlans");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.OwnerId)
            .HasConversion(id => id.Value, value => new UserId(value))
            .IsRequired();

        builder.Property(p => p.Portions)
            .HasConversion(p => p.Value, value => new Portions(value))
            .IsRequired();

        builder.HasIndex(p => p.OwnerId).IsUnique();

        builder.Navigation(p => p.Slots).HasField("_slots").UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(p => p.Slots, owned =>
        {
            owned.ToTable("MealSlots");
            owned.WithOwner().HasForeignKey("MenuPlanId");
            owned.HasKey(s => s.Id);
            owned.Property(s => s.Id).ValueGeneratedNever();
            owned.Property(s => s.Day).HasConversion<string>().HasMaxLength(16).IsRequired();
            owned.Property(s => s.MealType).HasConversion<string>().HasMaxLength(16).IsRequired();
            owned.Property(s => s.RecipeId)
                .HasConversion(
                    id => id.HasValue ? id.Value.Value : (Guid?)null,
                    value => value.HasValue ? new RecipeId(value.Value) : null);
        });
    }
}
