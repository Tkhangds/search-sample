using System.Net;
using Eme_Search.Common;
using Eme_Search.Common.Cache;
using Eme_Search.Modules.Search.DTOs;
using Eme_Search.Modules.Search.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Eme_Search.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Eme_Search.Modules.Search;

public class SearchController: ApiController
{
    private readonly SearchServiceResolver _resolver;
    private readonly ICacheService _cacheService;

    public SearchController(SearchServiceResolver resolver, IMemoryCache cache, ICacheService cacheService)
    {
        _resolver = resolver;
        _cacheService = cacheService;
    }
    
    [HttpGet()]
    public async Task<ActionResult> Search([FromQuery] StandardSearchRequestDto requestDto, string provider = "yelp")
    {
        var searchService = _resolver.Resolve(provider);
        
        string cacheKey = provider + requestDto.ToQueryString();
        
        var cachedProduct = await _cacheService.GetAsync<StandardBusinessSearchResponse>(cacheKey);
        
        if (cachedProduct != null)
        {
            return Ok(new SuccessResponse
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Search successful (Cached)",
                Data = cachedProduct
            });
        }
        
        var result = await searchService.SearchAsync(requestDto);
        
        var options = new DistributedCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10))
            .SetAbsoluteExpiration(TimeSpan.FromHours(1));
        
        await _cacheService.SetAsync<StandardBusinessSearchResponse>(cacheKey, result, options);
        
        return Ok(new SuccessResponse
        {
            StatusCode = HttpStatusCode.OK
            , Message = "Search successful"
            , Data = result
        });
    }
}