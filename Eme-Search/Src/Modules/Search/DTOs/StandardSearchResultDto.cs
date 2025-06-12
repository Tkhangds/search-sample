using System.Text.Json.Serialization;
using Google.Protobuf.Reflection;

namespace Eme_Search.Modules.Search.DTOs;

public class StandardSearchResultDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("alias")]
    public string Alias { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("image_url")]
    public string ImageUrl { get; set; } = string.Empty;

    [JsonPropertyName("is_closed")]
    public bool IsClosed { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("review_count")]
    public int ReviewCount { get; set; }

    [JsonPropertyName("categories")]
    public List<Category> Categories { get; set; } = new();

    [JsonPropertyName("rating")]
    public double Rating { get; set; }

    [JsonPropertyName("coordinates")]
    public Coordinates Coordinates { get; set; } = new();

    [JsonPropertyName("transactions")]
    public List<string> Transactions { get; set; } = new();

    [JsonPropertyName("price")]
    public string? Price { get; set; }

    [JsonPropertyName("location")]
    public SourceCodeInfo.Types.Location Location { get; set; } = new();

    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;

    [JsonPropertyName("display_phone")]
    public string DisplayPhone { get; set; } = string.Empty;

    [JsonPropertyName("distance")]
    public double Distance { get; set; }

    [JsonPropertyName("business_hours")]
    public List<BusinessHour> BusinessHours { get; set; } = new();

    [JsonPropertyName("attributes")]
    public BusinessAttributes Attributes { get; set; } = new();
}

public class Category
    {
        [JsonPropertyName("alias")]
        public string Alias { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
    }

    public class Coordinates
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }

    public class Location
    {
        [JsonPropertyName("address1")]
        public string Address1 { get; set; } = string.Empty;

        [JsonPropertyName("address2")]
        public string? Address2 { get; set; }

        [JsonPropertyName("address3")]
        public string? Address3 { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; } = string.Empty;

        [JsonPropertyName("zip_code")]
        public string ZipCode { get; set; } = string.Empty;

        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;

        [JsonPropertyName("state")]
        public string State { get; set; } = string.Empty;

        [JsonPropertyName("display_address")]
        public List<string> DisplayAddress { get; set; } = new();
    }

    public class BusinessHour
    {
        [JsonPropertyName("open")]
        public List<OpenHour> Open { get; set; } = new();

        [JsonPropertyName("hours_type")]
        public string HoursType { get; set; } = string.Empty;

        [JsonPropertyName("is_open_now")]
        public bool IsOpenNow { get; set; }
    }

    public class OpenHour
    {
        [JsonPropertyName("is_overnight")]
        public bool IsOvernight { get; set; }

        [JsonPropertyName("start")]
        public string Start { get; set; } = string.Empty;

        [JsonPropertyName("end")]
        public string End { get; set; } = string.Empty;

        [JsonPropertyName("day")]
        public int Day { get; set; }
    }

    public class BusinessAttributes
    {
        [JsonPropertyName("business_temp_closed")]
        public bool? BusinessTempClosed { get; set; }

        [JsonPropertyName("menu_url")]
        public string? MenuUrl { get; set; }

        [JsonPropertyName("open24_hours")]
        public bool? Open24Hours { get; set; }

        [JsonPropertyName("waitlist_reservation")]
        public bool? WaitlistReservation { get; set; }
    }

    public class StandardBusinessSearchResponse
    {
        [JsonPropertyName("businesses")]
        public List<StandardSearchResultDto> Businesses { get; set; } = new();

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("region")]
        public Region? Region { get; set; }
    }

    public class Region
    {
        [JsonPropertyName("center")]
        public Coordinates Center { get; set; } = new();
    }

    public enum TransactionType
    {
        Pickup,
        Delivery,
        RestaurantReservation
    }

    public enum PriceLevel
    {
        OneDollar,
        TwoDollar,
        ThreeDollar,
        FourDollar
    }