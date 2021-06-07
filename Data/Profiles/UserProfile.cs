using AutoMapper;
using Domain.Models;
using Shared.ViewModel;

namespace Data.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
            => CreateMap<User, UserViewModel>();
    }
}
