using AutoMapper;
using Eme_Search.Database.Models;
using Eme_Search.Modules.Blacklist.DTOs;

namespace Eme_Search.Modules.Blacklist.Profiles;

public class BlacklistCategoryResponseProfile: Profile
{
    public BlacklistCategoryResponseProfile()
    {
        CreateMap<BlacklistCategory, BlacklistCategoryResponseDto>()
            .ConstructUsing(entity => new BlacklistCategoryResponseDto
            {
                Id = entity.Id.ToString(),
                Alias = entity.Alias,
                Title = entity.Title
            });
        CreateMap<BlacklistCategoryResponseDto, BlacklistCategory>().ReverseMap();

    }
}