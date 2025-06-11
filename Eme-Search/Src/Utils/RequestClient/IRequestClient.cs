namespace Eme_Search.Utils.RequestClient;

public interface IRequestClient
{
    public Task<T> PostAsync<T>(string uri, HttpContent content, string? contentType, Dictionary<string, object>? headers = null);
    public Task<Dictionary<string, object>> PostAsync(string uri, HttpContent content, string? contentType = "application/json", Dictionary<string, object>? headers = null);
    public Task<T> PostFormAsync<T>(string uri, Dictionary<string, string> data, string? contentType = "application/x-www-form-urlencoded", Dictionary<string, object>? headers = null);
    public Task<Dictionary<string, object>> PostFormAsync(string uri, Dictionary<string, string> data, string? contentType = "application/x-www-form-urlencoded", Dictionary<string, object>? headers = null);
    public Task<T> GetJson<T>(string uri, Dictionary<string, object>? headers = null);
    public Task<Dictionary<string, object>> GetJson(string uri, Dictionary<string, object>? headers = null);
}