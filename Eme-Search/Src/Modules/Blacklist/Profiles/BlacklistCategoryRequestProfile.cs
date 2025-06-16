using AutoMapper;
using Eme_Search.Database.Models;
using Eme_Search.Modules.Blacklist.DTOs;

namespace Eme_Search.Modules.Blacklist.Profiles;

public class BlacklistCategoryRequestProfile: Profile
{
    public BlacklistCategoryRequestProfile()
    {
        CreateMap<BlacklistCategoryRequestDto, BlacklistCategory>()
            .ConstructUsing(dto => new BlacklistCategory
            {
                Alias = dto.Alias,
                Title = dto.Title
            });
        CreateMap<BlacklistCategory, BlacklistCategoryRequestDto>().ReverseMap();
    }
}