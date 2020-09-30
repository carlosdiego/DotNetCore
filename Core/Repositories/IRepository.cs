using Domain.Models;
using System.Linq;

namespace Core.Repositories
{
    public interface IRepository<T>
         where T : Entity
    {
        long Insert(T entity);
        void Update(T entity);
        T Get(long id);
        T GetAsNoTracking(long id);
        TViewModel GetProjected<TViewModel>(long id);
        IQueryable<T> GetAll();
        IQueryable<TViewModel> GetAllProjected<TViewModel>();
        void Remove(long id);
    }
}
