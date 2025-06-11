using Eme_Search.Modules.Search.DTOs;

namespace Eme_Search.Modules.Search.Services;

public interface ISearchService
{
    Task<YelpBusinessSearchResponse> SearchAsync(YelpSearchRequest request);

}