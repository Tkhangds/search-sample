using System.Net;
using System.Text.Json;
using Eme_Search.Common.Cache;
using Eme_Search.Modules.Blacklist.Services;
using Eme_Search.Modules.Search.DTOs;
using Eme_Search.Utils;
using Google.Rpc.Context;
using Grpc.Core;
using Microsoft.Extensions.Caching.Distributed;
using RestSharp;

namespace Eme_Search.Modules.Search.Services;

public class YelpSearchService: ISearchService
{
    public string ProviderName => "yelp";
    private const int Limit = 50; // Default limit for Yelp API
    
    private readonly IConfiguration _configuration;
    private readonly RestClient _client;
    private readonly ICacheService _cacheService;
    private readonly IBlacklistService _blacklistService;
    
    public YelpSearchService( IConfiguration configuration, ICacheService cacheService, IBlacklistService blacklistService )
    {
        _configuration = configuration;
        _cacheService = cacheService;
        _blacklistService = blacklistService;
        
        var secretKey = _configuration.GetValue<string>("YelpAPIKey");

        var options = new RestClientOptions("https://api.yelp.com/v3/businesses")
        {
            ThrowOnAnyError = true,
            Timeout = TimeSpan.FromSeconds(10),
        };

        _client = new RestClient(options);
        _client.AddDefaultHeader("accept", "application/json");
        _client.AddDefaultHeader("authorization", $"Bearer {secretKey}");
    }
    
    public async Task<StandardBusinessSearchResponse> SearchAsync(SearchRequestDto yelpRequestDto)
    {
        var cacheKey = ProviderName + yelpRequestDto.ToQueryString();

        var allBusinesses = new List<StandardSearchResultDto>();
        var total = 0;
        int offset = 0;

        bool isDone = false;
        
        int maxRetries = 3;
        
        while (!isDone)
        {
            var cacheKeyWithBlock = $"{cacheKey}:{offset}";
            
            var request = await CreateRequestAsync(yelpRequestDto, offset);
            StandardBusinessSearchResponse? yelpData = null;
            
            var cachedBlock = await _cacheService.GetAsync<StandardBusinessSearchResponse>(cacheKeyWithBlock);
            
            if (cachedBlock != null)
            {
                Console.WriteLine("--------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine("-------------------------------------------------------Cache Hit----------------------------------------------------------");
                Console.WriteLine("--------------------------------------------------------------------------------------------------------------------------");
                yelpData = cachedBlock;
                total = cachedBlock.Total;
            }
            else
            {
                for (int attempt = 1; attempt <= maxRetries; attempt++)
                {
                    try
                    {
                        var response = await _client.GetAsync(request);

                        if (response.IsSuccessStatusCode && response.Content != null)
                        {
                            var deserializerOptions = new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            };
                            yelpData = JsonSerializer.Deserialize<StandardBusinessSearchResponse>(
                                response.Content, deserializerOptions);
                            total = yelpData.Total;
                            await _cacheService.SetAsync<StandardBusinessSearchResponse>(
                                cacheKeyWithBlock,
                                yelpData,
                                new DistributedCacheEntryOptions()
                                    .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                                    .SetAbsoluteExpiration(TimeSpan.FromHours(1))
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Attempt {attempt}] Yelp API exception: {ex.Message}");

                        if (attempt == maxRetries)
                            throw new TimeoutException("Failed to fetch data from Yelp API after 3 attempts.");
                    }

                    if (attempt < maxRetries)
                        await Task.Delay(1000);
                }

                if (yelpData == null)
                {
                    isDone = true;
                    break;
                }
            }
            // Console.WriteLine("--------------------------------------------------------------------------------------------------------------------------");
            // Console.WriteLine($"yelpData.Businesses: {yelpData.Businesses.Count}");
            // Console.WriteLine("--------------------------------------------------------------------------------------------------------------------------");

            var filteredBusinesses = await _blacklistService.FilterSearchResults(yelpData.Businesses);
            // Console.WriteLine("--------------------------------------------------------------------------------------------------------------------------");
            // Console.WriteLine($"filteredBusinesses: {filteredBusinesses.Count}");
            // Console.WriteLine("--------------------------------------------------------------------------------------------------------------------------");

            allBusinesses.AddRange(filteredBusinesses);
            
            if (allBusinesses.Count >= (yelpRequestDto.Limit + yelpRequestDto.Offset) ||
                yelpData.Businesses.Count == 0)
            {
                isDone = true;
            }
            else
            {
                // Console.WriteLine("--------------------------------------------------------------------------------------------------------------------------");
                // Console.WriteLine($"All Business: {allBusinesses.Count}");
                // Console.WriteLine("--------------------------------------------------------------------------------------------------------------------------");
                // Console.WriteLine($"Limit: {yelpRequestDto.Limit}, Offset: {yelpRequestDto.Offset}");
                // Console.WriteLine("--------------------------------------------------------------------------------------------------------------------------");

                offset += Limit;
            }
        }

        var result = new StandardBusinessSearchResponse
        {
            Businesses = allBusinesses.Skip(yelpRequestDto.Offset).Take(yelpRequestDto.Limit).ToList(),
            Total = total
        };

        return result;
    }
    
    public async Task<StandardSearchResultDto?> SearchBusinessAsync(string idOrAlias)
    {
        var request = new RestRequest("/{business_id_or_alias}");
        request.AddParameter("business_id_or_alias", idOrAlias, ParameterType.UrlSegment);
        
        const int maxRetries = 3;
        
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                var response = await _client.GetAsync(request);

                if (response.IsSuccessStatusCode && response.Content != null)
                {
                    var deserializerOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var yelpData = JsonSerializer.Deserialize<StandardSearchResultDto>(
                        response.Content, deserializerOptions);

                    return yelpData;
                }

                Console.WriteLine($"[Attempt {attempt}] Yelp API returned status {(int)response.StatusCode}: {response.StatusDescription}");
                
                if (response.StatusCode == HttpStatusCode.Unauthorized ||
                    response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new UnauthorizedAccessException($"Yelp API authorization failed: {(int)response.StatusCode} {response.StatusDescription}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Attempt {attempt}] Yelp API exception: {ex.Message}");
            }

            if (attempt < maxRetries)
                await Task.Delay(1000);
        }

        throw new TimeoutException("Failed to fetch data from Yelp API after 3 attempts.");
    }
    
    private async Task<RestRequest> CreateRequestAsync(SearchRequestDto yelpRequestDto, int offset = 0)
    {
        var request = new RestRequest("/search");
        
        if (!string.IsNullOrWhiteSpace(yelpRequestDto.Location))
        {
            request.AddQueryParameter("location", yelpRequestDto.Location.Trim());
        }

        if(yelpRequestDto.Longitude.HasValue && yelpRequestDto.Latitude.HasValue)
        {
            request.AddQueryParameter("longitude", yelpRequestDto.Longitude.Value);
            request.AddQueryParameter("latitude", yelpRequestDto.Latitude.Value);
        }
        
        if (!string.IsNullOrWhiteSpace(yelpRequestDto.Term))
        {
            request.AddQueryParameter("term", yelpRequestDto.Term.Trim());
        }
        
        if (yelpRequestDto.Radius.HasValue)
        {
            request.AddQueryParameter("radius", yelpRequestDto.Radius.Value);
        }
        
        if (yelpRequestDto.Price is { Length: > 0 })
        {
            foreach (var item in yelpRequestDto.Price)
            {
                request.AddQueryParameter($"price", item.ToString());
            }
        }
        
        if (yelpRequestDto.Categories is { Length: > 0 })
        {
            foreach (var item in yelpRequestDto.Categories)
            {
                request.AddQueryParameter($"categories", item.ToString());
            }
        }
        
        if (!string.IsNullOrWhiteSpace(yelpRequestDto.Locale))
        {
            request.AddQueryParameter("locale", yelpRequestDto.Locale.Trim());
        }

        if (yelpRequestDto.OpenNow.HasValue)
        {
            request.AddQueryParameter("open_now", yelpRequestDto.OpenNow.Value);
        }

        if (yelpRequestDto.OpenAt.HasValue)
        {
            request.AddQueryParameter("open_at", yelpRequestDto.OpenAt.Value);
        }
        
        if(!string.IsNullOrWhiteSpace(yelpRequestDto.DevicePlatform))
        {
            request.AddQueryParameter("device_platform", yelpRequestDto.DevicePlatform.Trim());
        }

        if(!string.IsNullOrWhiteSpace(yelpRequestDto.ReservationDate))
        {
            request.AddQueryParameter("reservation_date", yelpRequestDto.ReservationDate.Trim());
        }

        if (!string.IsNullOrWhiteSpace(yelpRequestDto.ReservationTime))
        {
            request.AddQueryParameter("reservation_time", yelpRequestDto.ReservationTime.Trim());
        }
        
        if (yelpRequestDto.MatchesPartySizeParam.HasValue )
        {
            request.AddQueryParameter("matches_party_size_param", yelpRequestDto.MatchesPartySizeParam.Value);
        }
        
        if (yelpRequestDto.ReservationCovers.HasValue)
        {
            request.AddQueryParameter("reservation_covers", yelpRequestDto.ReservationCovers.Value);
        }
        
        request.AddQueryParameter("sort_by", yelpRequestDto.SortBy);
        request.AddQueryParameter("limit", Limit.ToString());
        request.AddQueryParameter("offset", offset);

        return request;
    }
}
