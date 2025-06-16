using System.ComponentModel.DataAnnotations;

namespace Eme_Search.Modules.Search.DTOs;

public class BlacklistRequestDto
{
    [StringLength(250, MinimumLength = 1)]
    public string Location { get; set; } = string.Empty;
    [Range(-90, 90)]
    public int? Latitude { get; set; }
    [Range(-180, 180)]
    public int? Longitude { get; set; }
    public string Term { get; set; } = string.Empty;
    [Range(0,40000)]
    public int? Radius { get; set; }
    public string[] Categories { get; set; } = [];
    public string Locale { get; set; } = string.Empty;
    public int[] Price { get; set; } = [];
    public bool? OpenNow { get; set; }
    public int? OpenAt { get; set; }
    public string[] Attributes { get; set; } = [];
    public string SortBy { get; set; } = "best_match";
    public string DevicePlatform { get; set; } = string.Empty;
    public string ReservationDate { get; set; } = string.Empty;
    public string ReservationTime { get; set; } = string.Empty;
    [Range(1, 10)]
    public int? ReservationCovers { get; set; }
    public bool? MatchesPartySizeParam { get; set; }
    [Range(0, 50)]
    public int Limit { get; set; } = 20; 
    [Range(0, 1000)]
    public int Offset { get; set; } = 0;

    public string ToQueryString()
{
    var queryParams = new List<string>();

    if (!string.IsNullOrWhiteSpace(Location))
        queryParams.Add($"location={Uri.EscapeDataString(Location)}");

    if (Latitude.HasValue)
        queryParams.Add($"latitude={Latitude.Value}");

    if (Longitude.HasValue)
        queryParams.Add($"longitude={Longitude.Value}");

    if (!string.IsNullOrWhiteSpace(Term))
        queryParams.Add($"term={Uri.EscapeDataString(Term)}");

    if (Radius.HasValue)
        queryParams.Add($"radius={Radius.Value}");

    if (Categories is { Length: > 0 })
        queryParams.Add($"categories={Uri.EscapeDataString(string.Join(",", Categories))}");

    if (!string.IsNullOrWhiteSpace(Locale))
        queryParams.Add($"locale={Uri.EscapeDataString(Locale)}");

    if (Price is { Length: > 0 })
        queryParams.Add($"price={string.Join(",", Price)}");

    if (OpenNow.HasValue && OpenNow.Value)
        queryParams.Add("open_now=true");

    if (OpenAt > 0)
        queryParams.Add($"open_at={OpenAt}");

    if (Attributes is { Length: > 0 })
        queryParams.Add($"attributes={Uri.EscapeDataString(string.Join(",", Attributes))}");

    if (!string.IsNullOrWhiteSpace(SortBy))
        queryParams.Add($"sort_by={Uri.EscapeDataString(SortBy)}");

    if (!string.IsNullOrWhiteSpace(DevicePlatform))
        queryParams.Add($"device_platform={Uri.EscapeDataString(DevicePlatform)}");

    if (!string.IsNullOrWhiteSpace(ReservationDate))
        queryParams.Add($"reservation_date={Uri.EscapeDataString(ReservationDate)}");

    if (!string.IsNullOrWhiteSpace(ReservationTime))
        queryParams.Add($"reservation_time={Uri.EscapeDataString(ReservationTime)}");

    if (ReservationCovers.HasValue)
        queryParams.Add($"reservation_covers={ReservationCovers.Value}");

    if (MatchesPartySizeParam.HasValue && MatchesPartySizeParam.Value)
        queryParams.Add("matches_party_size_param=true");

    queryParams.Add($"limit={Limit}");
    queryParams.Add($"offset={Offset}");

    return string.Join("&", queryParams);
}
}