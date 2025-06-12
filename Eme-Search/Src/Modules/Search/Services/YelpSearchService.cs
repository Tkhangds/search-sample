using System.Net;
using System.Text.Json;
using Eme_Search.Modules.Search.DTOs;
using RestSharp;

namespace Eme_Search.Modules.Search.Services;

public class YelpSearchService: ISearchService
{
    public string ProviderName => "yelp";
    
    private readonly IConfiguration _configuration;
    private readonly RestClient _client;
    
    public YelpSearchService( IConfiguration configuration)
    {
        _configuration = configuration;
        
        var secretKey = _configuration.GetValue<string>("YelpAPIKey");

        var options = new RestClientOptions("https://api.yelp.com/v3/businesses/search")
        {
            ThrowOnAnyError = true,
            Timeout = TimeSpan.FromSeconds(10),
        };

        _client = new RestClient(options);
        _client.AddDefaultHeader("accept", "application/json");
        _client.AddDefaultHeader("authorization", $"Bearer {secretKey}");
    }
    
    public async Task<StandardBusinessSearchResponse> SearchAsync(StandardSearchRequestDto yelpRequestDto)
    {
        var request = new RestRequest("");

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
        request.AddQueryParameter("limit", yelpRequestDto.Limit.ToString());
        request.AddQueryParameter("offset", yelpRequestDto.Offset.ToString());
        
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

                    var yelpData = JsonSerializer.Deserialize<StandardBusinessSearchResponse>(
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
}