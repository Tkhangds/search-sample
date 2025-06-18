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
    
    public static BaseSpecification<BlacklistBusiness> GetByProvider(string provider)
    {
        return new BaseSpecification<BlacklistBusiness>(x => x.Provider == provider);
    }
    
    public static BaseSpecification<BlacklistBusiness> GetPagedBlacklistBusiness(PaginationDto paginationDto)
    {
        var spec = new BaseSpecification<BlacklistBusiness>(null);
        spec.ApplyPaging((paginationDto.Page - 1) * paginationDto.Limit, paginationDto.Limit);
        return spec;
    }
}