using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Eme_Search.Utils.RequestClient;

public class RequestClient : IRequestClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _requestClient;

    public RequestClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _requestClient = _httpClientFactory.CreateClient();
    }

    public async Task<T> PostAsync<T>(string uri, HttpContent content, string? contentType = "application/json",
        Dictionary<string, object>? headers = null)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(uri),
            Content = content,
        };
        if (contentType != null)
        {
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        }

        if (headers != null)
        {
            foreach (var (key, value) in headers)
            {
                request.Headers.Add(key, value.ToString());
            }
        }

        var response = await _requestClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(response.ReasonPhrase);
        }
        
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(responseString);
    }

    public Task<Dictionary<string, object>> PostAsync(string uri, HttpContent content, string? contentType = "application/json",
        Dictionary<string, object>? headers = null)
    {
        return PostAsync<Dictionary<string, object>>(uri, content, contentType, headers);
    }

    public Task<T> PostFormAsync<T>(string uri, Dictionary<string, string> data, string? contentType = "application/x-www-form-urlencoded",
        Dictionary<string, object>? headers = null)
    {
        return PostAsync<T>(uri, new FormUrlEncodedContent(data), contentType, headers);
    }

    public Task<Dictionary<string, object>> PostFormAsync(string uri, Dictionary<string, string> data,
        string? contentType = "application/x-www-form-urlencoded", Dictionary<string, object>? headers = null)
    {
        return PostFormAsync<Dictionary<string, object>>(uri, data, contentType, headers);
    }

    public async Task<T> GetJson<T>(string uri, Dictionary<string, object>? headers = null)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(uri),
        };
        if (headers != null)
        {
            foreach (var (key, value) in headers)
            {
                request.Headers.Add(key, value.ToString());
            }
        }

        var response = await _requestClient.SendAsync(request);
        var responseString = await response.Content.ReadAsStringAsync();
        var e = JsonConvert.DeserializeObject<T>(responseString);
        return e;
    }

    public Task<Dictionary<string, object>> GetJson(string uri, Dictionary<string, object>? headers = null)
    {
        return GetJson<Dictionary<string, object>>(uri, headers);
    }
}