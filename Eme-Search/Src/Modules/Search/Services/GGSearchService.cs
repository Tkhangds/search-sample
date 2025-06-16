using Eme_Search.Modules.Search.DTOs;

namespace Eme_Search.Modules.Search.Services;

public class GGSearchService : ISearchService
{
    public string ProviderName => "google";

    public Task<StandardBusinessSearchResponse> SearchAsync(SearchRequestDto request)
    {
        // gọi API Google hoặc mock
        throw new NotImplementedException();
    }
}