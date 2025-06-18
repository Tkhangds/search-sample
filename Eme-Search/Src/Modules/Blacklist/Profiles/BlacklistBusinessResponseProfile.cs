using AutoMapper;
using Eme_Search.Database.Models;
using Eme_Search.Modules.Blacklist.DTOs;

namespace Eme_Search.Modules.Blacklist.Profiles;

public class BlacklistBusinessResponseProfile: Profile
{
    public BlacklistBusinessResponseProfile()
    {
        CreateMap<BlacklistBusiness, BlacklistBusinessResponseDto>()
            .ConstructUsing(entity => new BlacklistBusinessResponseDto
                {
                    Id = entity.Id.ToString(),
                    Alias = entity.Alias,
                    Provider = entity.Provider
                }
            );
        CreateMap<BlacklistBusinessResponseDto, BlacklistBusiness>().ReverseMap();
    }
}