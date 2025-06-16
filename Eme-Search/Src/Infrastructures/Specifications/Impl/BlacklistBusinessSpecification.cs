using Eme_Search.Common;
using Eme_Search.Database.Models;

namespace Eme_Search.Infrastructures.Specifications.Impl;

public class BlacklistBusinessSpecification
{
    public static BaseSpecification<BlacklistBusiness> GetByAlias(string alias)
    {
        return new BaseSpecification<BlacklistBusiness>(x => x.Alias == alias);
    }
    
    public static BaseSpecification<BlacklistBusiness> GetById(int id)
    {
        return new BaseSpecification<BlacklistBusiness>(x => x.Id == id);
    }
    
    public static BaseSpecification<BlacklistBusiness> GetByTitle(string name)
    {
        return new BaseSpecification<BlacklistBusiness>(x => x.Name == name);
    }
    
    public static BaseSpecification<BlacklistBusiness> GetPagedBlacklistBusiness(PaginationDto paginationDto)
    {
        var spec = new BaseSpecification<BlacklistBusiness>(null);
        spec.ApplyPaging((paginationDto.Page - 1) * paginationDto.Limit, paginationDto.Limit);
        return spec;
    }
}