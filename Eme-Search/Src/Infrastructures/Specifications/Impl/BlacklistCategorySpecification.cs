using Eme_Search.Common;
using Eme_Search.Database.Models;

namespace Eme_Search.Infrastructures.Specifications.Impl;

public class BlacklistCategorySpecification
{
    public static BaseSpecification<BlacklistCategory> GetByAlias(string alias)
    {
        return new BaseSpecification<BlacklistCategory>(x => x.Alias == alias);
    }
    
    public static BaseSpecification<BlacklistCategory> GetById(int id)
    {
        return new BaseSpecification<BlacklistCategory>(x => x.Id == id);
    }
    
    public static BaseSpecification<BlacklistCategory> GetByTitle(string title)
    {
        return new BaseSpecification<BlacklistCategory>(x => x.Title == title);
    }
    
    public static BaseSpecification<BlacklistCategory> GetPagedBlacklistCategory(PaginationDto paginationDto)
    {
        var spec = new BaseSpecification<BlacklistCategory>(null);
        spec.ApplyPaging((paginationDto.Page - 1) * paginationDto.Limit, paginationDto.Limit);
        return spec;
    }
}