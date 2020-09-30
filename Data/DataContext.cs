using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var baseType = typeof(IEntityTypeConfiguration<>);

            modelBuilder.UseIdentityColumns();

            var baseEntityType = typeof(Entity);
            var baseStringType = typeof(string);
            modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(x => baseEntityType.IsAssignableFrom(x.ClrType))
                .Select(p => modelBuilder.Entity(p.DeclaringEntityType.ClrType).Property(p.Name))
                .ToList()
                .ForEach(propBuilder => propBuilder.IsRequired().IsUnicode(false).HasMaxLength(255));

            typeof(DataContext)
                .Assembly
                .GetTypes()
                .Where(t => t.GetInterfaces().Any(gi => gi.IsGenericType && gi.GetGenericTypeDefinition() == baseType))
                .Select(t => (dynamic)Activator.CreateInstance(t))
                .ToList()
                .ForEach(configurationInstance => modelBuilder.ApplyConfiguration(configurationInstance));

            base.OnModelCreating(modelBuilder);
        }

    }
}
