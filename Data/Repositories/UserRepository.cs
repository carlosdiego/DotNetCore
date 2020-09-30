using AutoMapper;
using Domain.Models;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;

namespace Data.Repositories
{
    public class UserRepository : Repository<User, DataContext>, IUserRepository
    {
        public UserRepository(DataContext ctx, IMapper mapper)
           : base(ctx, mapper)
        { }
        public bool ContainsAnotherUserWithSameName(string name)
            => _currentSet.Any(u => u.Name == name);
    }
}
