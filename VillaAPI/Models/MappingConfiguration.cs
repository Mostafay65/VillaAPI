using AutoMapper;
using VillaAPI.Models.DTO;

namespace VillaAPI.Models;

public class MappingConfiguration : Profile
{
    public MappingConfiguration()
    {
        CreateMap<Villa, VillaDTO>().ReverseMap();
        CreateMap<VillaNumber, VillaNumberDto>().ReverseMap();
        CreateMap<LocalUser, LocalUserDTO>().ReverseMap();
        CreateMap<ApplicationUser, LocalUserDTO>().ReverseMap();
    }
}