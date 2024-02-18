using AutoMapper;
using CommonLib.Model;

namespace TestServiceGRPC.Utils;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUser, LoginResponse>();
    }
}
