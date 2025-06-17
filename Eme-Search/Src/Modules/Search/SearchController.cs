using System.Net;
using Eme_Search.Common;
using Eme_Search.Common.Cache;
using Eme_Search.Modules.Blacklist.Services;
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
    public SearchController(SearchServiceResolver resolver, IMemoryCache cache, ICacheService cacheService, IBlacklistService blacklistService)
    {
        _resolver = resolver;
    }
    
    [HttpGet()]
    public async Task<ActionResult> Search([FromQuery] SearchRequestDto requestDto, string provider = "yelp")
    {
        // Resolve the search service based on the provider
        var searchService = _resolver.Resolve(provider);
        
        var result = await searchService.SearchAsync(requestDto);
        
        return Ok(new SuccessResponse
        {
            StatusCode = HttpStatusCode.OK
            , Message = "Search successful"
            , Data = result
        });
    }
    
    // [HttpGet("/{idOrAlias}")]
    // public async Task<ActionResult> SearchBusinessByIdOrAlias([FromRoute] string idOrAlias,string provider = "yelp")
    // {
    //     var searchService = _resolver.Resolve(provider);
    //     
    //     string cacheKey = provider + idOrAlias;
    //     
    //     var cachedProduct = await _cacheService.GetAsync<StandardSearchResultDto>(cacheKey);
    //     
    //     if (cachedProduct != null)
    //     {
    //         return Ok(new SuccessResponse
    //         {
    //             StatusCode = HttpStatusCode.OK,
    //             Message = "Search successful (Cached)",
    //             Data = cachedProduct
    //         });
    //     }
    //     
    //     var result = await searchService.SearchBusinessAsync(idOrAlias);
    //     
    //     var options = new DistributedCacheEntryOptions()
    //         .SetSlidingExpiration(TimeSpan.FromMinutes(10))
    //         .SetAbsoluteExpiration(TimeSpan.FromHours(1));
    //     
    //     await _cacheService.SetAsync<StandardSearchResultDto>(cacheKey, result, options);
    //     
    //     return Ok(new SuccessResponse
    //     {
    //         StatusCode = HttpStatusCode.OK
    //         , Message = "Search business successful"
    //         , Data = result
    //     });
    // }
}