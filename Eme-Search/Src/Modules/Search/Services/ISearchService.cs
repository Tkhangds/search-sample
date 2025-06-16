using Eme_Search.Modules.Search.DTOs;

namespace Eme_Search.Modules.Search.Services;

public interface ISearchService
{
    string ProviderName { get; }
    Task<StandardBusinessSearchResponse> SearchAsync(SearchRequestDto requestDto);
    Task<StandardSearchResultDto?> SearchBusinessAsync(string idOrAlias);
}