using AutoMapper;
using Eme_Search.Database.Models;
using Eme_Search.Modules.Blacklist.DTOs;

namespace Eme_Search.Modules.Blacklist.Profiles;

public class BlacklistBusinessRequestProfile: Profile
{
    public BlacklistBusinessRequestProfile()
    {
        CreateMap<BlacklistBusinessRequestDto, BlacklistBusiness>()
            .ConstructUsing(dto => new BlacklistBusiness
            {
                Alias = dto.Alias,
                Name = dto.Name,
                YelpId = dto.YelpId
            });
        CreateMap<BlacklistBusiness, BlacklistBusinessRequestDto>().ReverseMap();
    }
}