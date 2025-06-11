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
    private readonly ISearchService _searchService;   
    private readonly IMemoryCache _cache;

    public SearchController(ISearchService searchService, IMemoryCache cache)
    {
        _cache = cache;
        _searchService = searchService;
    }
    
    [HttpGet()]
    public async Task<ActionResult> Search([FromQuery] YelpSearchRequest request)
    {
        string cacheKey = request.ToQueryString();

        Console.WriteLine(cacheKey);
        
        if (_cache.TryGetValue(cacheKey, out YelpBusinessSearchResponse? cachedProduct))
        {
            return Ok(new SuccessResponse
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Search successful (Cached)",
                Data = cachedProduct
            });
        }
        
        var result = await _searchService.SearchAsync(request);
        
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