using System.Net;
using Eme_Search.Common;
using Eme_Search.Modules.Search.DTOs;
using Eme_Search.Modules.Search.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Eme_Search.Utils;
using Microsoft.Extensions.Caching.Memory;

namespace Eme_Search.Modules.Search;

public class SearchController: ApiController
{
    private readonly SearchServiceResolver _resolver;
    private readonly IMemoryCache _cache;

    public SearchController(SearchServiceResolver resolver, IMemoryCache cache)
    {
        _cache = cache;
        _resolver = resolver;
    }
    
    [HttpGet()]
    public async Task<ActionResult> Search([FromQuery] StandardSearchRequestDto requestDto, string provider = "yelp")
    {
        var searchService = _resolver.Resolve(provider);
        
        string cacheKey = provider + requestDto.ToQueryString();
        
        if (_cache.TryGetValue(cacheKey, out StandardBusinessSearchResponse? cachedProduct))
        {
            return Ok(new SuccessResponse
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Search successful (Cached)",
                Data = cachedProduct
            });
        }
        
        var result = await searchService.SearchAsync(requestDto);
        
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(5))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(20));
        
        _cache.Set(cacheKey, result, cacheEntryOptions);
        
        return Ok(new SuccessResponse
        {
            StatusCode = HttpStatusCode.OK
            , Message = "Search successful"
            , Data = result
        });
    }
}