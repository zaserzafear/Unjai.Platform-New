using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Unjai.Platform.Domain.Abstractions;

namespace Unjai.Platform.Infrastructure.Persistent.Database.Conventions;

internal static class SoftDeleteConvention
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                     .Where(t => typeof(EntityBase).IsAssignableFrom(t.ClrType)))
        {
            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var prop = Expression.Property(parameter, nameof(EntityBase.IsDeleted));
            var compare = Expression.Equal(prop, Expression.Constant(false));
            var lambda = Expression.Lambda(compare, parameter);

            modelBuilder.Entity(entityType.ClrType)
                        .HasQueryFilter(lambda);
        }
    }
}
