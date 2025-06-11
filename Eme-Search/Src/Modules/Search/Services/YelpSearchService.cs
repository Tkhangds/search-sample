using System.Net;
using System.Text.Json;
using Eme_Search.Modules.Search.DTOs;
using RestSharp;

namespace Eme_Search.Modules.Search.Services;

public class YelpSearchService: ISearchService
{
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
    
    public async Task<YelpBusinessSearchResponse?> SearchAsync(YelpSearchRequest yelpRequest)
    {
        var request = new RestRequest("");

        if (!string.IsNullOrWhiteSpace(yelpRequest.Location))
        {
            request.AddQueryParameter("location", yelpRequest.Location.Trim());
        }

        if(yelpRequest.Longitude.HasValue && yelpRequest.Latitude.HasValue)
        {
            request.AddQueryParameter("longitude", yelpRequest.Longitude.Value);
            request.AddQueryParameter("latitude", yelpRequest.Latitude.Value);
        }
        
        if (!string.IsNullOrWhiteSpace(yelpRequest.Term))
        {
            request.AddQueryParameter("term", yelpRequest.Term.Trim());
        }
        
        if (yelpRequest.Radius.HasValue)
        {
            request.AddQueryParameter("radius", yelpRequest.Radius.Value);
        }
        
        if (yelpRequest.Price is { Length: > 0 })
        {
            foreach (var item in yelpRequest.Price)
            {
                request.AddQueryParameter($"price", item.ToString());
            }
        }
        
        if (yelpRequest.Categories is { Length: > 0 })
        {
            foreach (var item in yelpRequest.Categories)
            {
                request.AddQueryParameter($"categories", item.ToString());
            }
        }
        
        if (!string.IsNullOrWhiteSpace(yelpRequest.Locale))
        {
            request.AddQueryParameter("locale", yelpRequest.Locale.Trim());
        }

        if (yelpRequest.OpenNow.HasValue)
        {
            request.AddQueryParameter("open_now", yelpRequest.OpenNow.Value);
        }

        if (yelpRequest.OpenAt.HasValue)
        {
            request.AddQueryParameter("open_at", yelpRequest.OpenAt.Value);
        }
        
        if(!string.IsNullOrWhiteSpace(yelpRequest.DevicePlatform))
        {
            request.AddQueryParameter("device_platform", yelpRequest.DevicePlatform.Trim());
        }

        if(!string.IsNullOrWhiteSpace(yelpRequest.ReservationDate))
        {
            request.AddQueryParameter("reservation_date", yelpRequest.ReservationDate.Trim());
        }

        if (!string.IsNullOrWhiteSpace(yelpRequest.ReservationTime))
        {
            request.AddQueryParameter("reservation_time", yelpRequest.ReservationTime.Trim());
        }
        
        if (yelpRequest.MatchesPartySizeParam.HasValue )
        {
            request.AddQueryParameter("matches_party_size_param", yelpRequest.MatchesPartySizeParam.Value);
        }
        
        if (yelpRequest.ReservationCovers.HasValue)
        {
            request.AddQueryParameter("reservation_covers", yelpRequest.ReservationCovers.Value);
        }
        
        request.AddQueryParameter("sort_by", yelpRequest.SortBy);
        request.AddQueryParameter("limit", yelpRequest.Limit.ToString());
        request.AddQueryParameter("offset", yelpRequest.Offset.ToString());
        
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

                    var yelpData = JsonSerializer.Deserialize<YelpBusinessSearchResponse>(
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