using Eme_Search.Common;
using Eme_Search.Modules.Blacklist.DTOs;
using Pagination.EntityFrameworkCore.Extensions;

namespace Eme_Search.Modules.Blacklist.Services;

public interface IBlacklistService
{
    Task<int> AddBlacklistedBusiness(BlacklistBusinessRequestDto request);
    Task RemoveBlacklistedBusiness(int id);
    Task<int> AddBlacklistedCategory(BlacklistCategoryRequestDto request);
    Task RemoveBlacklistedCategory(int id);
    Task<Pagination<BlacklistBusinessResponseDto>> GetBlacklistedBusinesses(PaginationDto pagination);
    Task<Pagination<BlacklistCategoryResponseDto>> GetBlacklistedCategories(PaginationDto pagination);
}