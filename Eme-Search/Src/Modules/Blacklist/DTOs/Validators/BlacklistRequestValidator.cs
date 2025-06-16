using FluentValidation;
using Eme_Search.Modules.Blacklist.DTOs;

namespace Eme_Search.Modules.Blacklist.DTOs.Validators;

public class BlacklistRequestValidator : AbstractValidator<BlacklistCategoryRequestDto>
{
    public BlacklistRequestValidator()
    {
        
    }
}