using AutoMapper;
using Outmatch.API.Dtos;
using Outmatch.API.Models;

namespace Outmatch.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForRegisterDto>();
            CreateMap<UserForRegisterDto, User>();
            CreateMap<User, ClientForListDto>();
            CreateMap<UserForEditDto, User>();
            CreateMap<LocationCreateDto, Locations>();
            CreateMap<Locations, LocationCreateDto>();
        }
    }
}