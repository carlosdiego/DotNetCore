using Core.Repositories;
using Domain.Models;

namespace Domain.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        bool ContainsAnotherUserWithSameName(string name);
    }
}
