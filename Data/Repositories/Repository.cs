using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Core.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class Repository<T, TDbContext> : IRepository<T>
           where T : Entity
           where TDbContext : DbContext
    {
        protected readonly TDbContext _context;
        protected readonly DbSet<T> _currentSet;
        protected readonly IConfigurationProvider _mapperConfigProvider;

        protected event BeforeChangeDelegate BeforeDelete;

        protected event BeforeChangeDelegate BeforeUpdate;

        public delegate void BeforeChangeDelegate(ref EntityEntry<T> entity);

        public Repository(TDbContext ctx, IMapper mapper)
        {
            _context = ctx;
            _currentSet = _context.Set<T>();
            _mapperConfigProvider = mapper.ConfigurationProvider;
        }


        public Task<T> GetByIdAsNoTrackingAsync(long id)
            => _currentSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);

        public TViewModel GetProjected<TViewModel>(long id)
            => _currentSet.Where(e => e.Id == id).UseAsDataSource(_mapperConfigProvider).For<TViewModel>().FirstOrDefault();

        public IQueryable<T> GetAll()
            => _currentSet;


        public Task<List<T>> GetAllAsync()
            => _currentSet.ToListAsync();

        public IQueryable<TViewModel> GetAllProjected<TViewModel>()
            => _currentSet.UseAsDataSource(_mapperConfigProvider).For<TViewModel>();


        public async Task<long> InsertAsync(T entity)
        {
            await _currentSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }
        public async Task UpdateAsync(T entity)
        {
            var entityEntry = _context.Entry(entity);
            BeforeUpdate?.Invoke(ref entityEntry);
            UpdateWithoutChangeCreatedDate(in entityEntry);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(long id)
        {
            var entityEntry = _context.Entry(CreateInstance<T>());
            entityEntry.Property<long>(nameof(Entity.Id)).CurrentValue = id;
            entityEntry.State = EntityState.Deleted;
            BeforeDelete?.Invoke(ref entityEntry);
            await _context.SaveChangesAsync();
        }

        private static TEntity CreateInstance<TEntity>()
            => (TEntity)Activator.CreateInstance(typeof(TEntity), BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, null, null);

        protected static void UpdateWithoutChangeCreatedDate<TEntity>(in EntityEntry<TEntity> entityEntry)
            where TEntity : Entity
        {
            entityEntry.State = EntityState.Modified;
            entityEntry.Property(t => t.CreatedDate).IsModified = false;
        }
        public async Task<bool> ExistAsync(Expression<Func<T, bool>> predicate)
        {
            return await _currentSet.AnyAsync(predicate);
        }
        public async Task<T> GetByIdAsync(long id)
        {
            return await _currentSet.FindAsync(id);
        }
        public async Task<T> FindAsync(Expression<Func<T, bool>> match)
        {
            return await _currentSet.SingleOrDefaultAsync(match);
        }
        public async Task<ICollection<T>> FindAllAsync(Expression<Func<T, bool>> match)
        {
            return await _currentSet.Where(match).ToListAsync();
        }
        public async Task<int> CountAsync()
        {
            return await _currentSet.CountAsync();
        }
        public async Task<IList<T>> GetAllIncludeAsync(params Expression<Func<T, object>>[] navigationProperties)
        {
            IQueryable<T> dbQuery = _currentSet;

            //Apply eager loading
            foreach (var navigationProperty in navigationProperties)
                dbQuery = dbQuery.Include(navigationProperty);

            return await dbQuery.AsNoTracking().ToListAsync();
        }
        public async Task<IList<T>> GetIncludeListAsync(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties)
        {
            IQueryable<T> dbQuery = _currentSet;

            //Apply eager loading
            foreach (var navigationProperty in navigationProperties)
                dbQuery = dbQuery.Include(navigationProperty);

            dbQuery = dbQuery.AsNoTracking().Where(where).AsQueryable();

            return await dbQuery.ToListAsync();
        }
        public async Task<T> GetSingleIncludeAsync(Expression<Func<T, bool>> where,
            params Expression<Func<T, object>>[] navigationProperties)
        {
            T item = null;
            IQueryable<T> dbQuery = _currentSet;

            //Apply eager loading
            if (navigationProperties.Any())
                foreach (var navigationProperty in navigationProperties)
                    dbQuery = dbQuery.Include(navigationProperty);
            else
                foreach (var property in _context.Model.FindEntityType(typeof(T)).GetNavigations())
                    dbQuery = dbQuery.Include(property.Name);

            item = await dbQuery
                .AsNoTracking() //Don't track any changes for the selected item
                .SingleOrDefaultAsync(where); //Apply where clause
            return item;
        }
        public async Task<ICollection<T>> PaggedListAsync(int? pageSize, int? page, params Expression<Func<T, object>>[] navigationProperties)
        {
            IQueryable<T> query = _context.Set<T>();

            foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                query = query.Include<T, object>(navigationProperty);

            if (page != null && pageSize != null)
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);

            return await query.ToListAsync();
        }
    }

}
