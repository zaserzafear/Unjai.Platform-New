using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Unjai.Platform.Domain.Primitives;

namespace Unjai.Platform.Infrastructure.Database;

internal class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model
            .GetEntityTypes()
            .Where(t => typeof(EntityBase).IsAssignableFrom(t.ClrType)))
        {
            var entity = modelBuilder.Entity(entityType.ClrType);

            entity.Property(nameof(EntityBase.CreatedAt))
                  .HasColumnName("created_at")
                  .HasDefaultValueSql("NOW()")
                  .ValueGeneratedOnAdd();

            entity.Property(nameof(EntityBase.UpdatedAt))
                  .HasColumnName("updated_at")
                  .HasDefaultValueSql("NOW()")
                  .ValueGeneratedOnAddOrUpdate();

            entity.Property(nameof(EntityBase.IsDeleted))
                  .HasColumnName("is_deleted")
                  .HasDefaultValue(false);

            entity.Property(nameof(EntityBase.DeletedAt))
                  .HasColumnName("deleted_at");

            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var prop = Expression.Property(parameter, nameof(EntityBase.IsDeleted));
            var compare = Expression.Equal(prop, Expression.Constant(false));
            var lambda = Expression.Lambda(compare, parameter);

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
        }

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
