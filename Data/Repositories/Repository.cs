using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Domain.Models;
using Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Reflection;

namespace Data.Repositories
{
    public class Repository<T, TDbContext> : IRepository<T>
            where T : Entity
            where TDbContext : DbContext
    {
        protected readonly TDbContext _context;
        protected readonly DbSet<T> _currentSet;
        protected readonly AutoMapper.IConfigurationProvider _mapperConfigProvider;

        protected event BeforeChangeDelegate BeforeDelete;
        protected event BeforeChangeDelegate BeforeUpdate;

        public delegate void BeforeChangeDelegate(ref EntityEntry<T> entity);

        public Repository(TDbContext ctx, IMapper mapper)
        {
            _context = ctx;
            _currentSet = _context.Set<T>();
            _mapperConfigProvider = mapper.ConfigurationProvider;
        }

        public T Get(long id)
            => _currentSet.FirstOrDefault(e => e.Id == id);

        public T GetAsNoTracking(long id)
            => _currentSet.AsNoTracking().FirstOrDefault(e => e.Id == id);

        public TViewModel GetProjected<TViewModel>(long id)
            => _currentSet.Where(e => e.Id == id).UseAsDataSource(_mapperConfigProvider).For<TViewModel>().FirstOrDefault();

        public IQueryable<T> GetAll()
            => _currentSet;

        public IQueryable<TViewModel> GetAllProjected<TViewModel>()
            => _currentSet.UseAsDataSource(_mapperConfigProvider).For<TViewModel>();

        public long Insert(T entity)
        {
            _currentSet.Add(entity);
            _context.SaveChanges();
            return entity.Id;
        }

        public void Remove(long id)
        {
            var entityEntry = _context.Entry(CreateInstance<T>());
            entityEntry.Property<long>(nameof(Entity.Id)).CurrentValue = id;
            entityEntry.State = EntityState.Deleted;
            BeforeDelete?.Invoke(ref entityEntry);
            _context.SaveChanges();
        }

        public void Update(T entity)
        {
            var entityEntry = _context.Entry(entity);
            BeforeUpdate?.Invoke(ref entityEntry);
            UpdateWithoutChangeCreatedDate(in entityEntry);
            _context.SaveChanges();
        }

        private static TEntity CreateInstance<TEntity>()
            => (TEntity)Activator.CreateInstance(typeof(TEntity), BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, null, null);

        protected static void UpdateWithoutChangeCreatedDate<TEntity>(in EntityEntry<TEntity> entityEntry)
            where TEntity : Entity
        {
            entityEntry.State = EntityState.Modified;
            entityEntry.Property(t => t.CreatedDate).IsModified = false;
        }
    }
}
