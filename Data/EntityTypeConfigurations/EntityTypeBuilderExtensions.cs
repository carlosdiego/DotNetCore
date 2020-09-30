using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntityTypeConfigurations
{
    public static class EntityTypeBuilderExtensions
    {
        public static void ApplyDefaultConfig<T>(this EntityTypeBuilder<T> builder)
            where T : Entity => builder.HasKey(i => i.Id);
    }
}
