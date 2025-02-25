using AutoMapper;
using Fido2Apis.Application.Apis.User;
using Fido2Apis.Domain.User;

namespace Fido2Apis.Infra.ObjectMapper
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<User, UserDto>();
        }
    }
}
