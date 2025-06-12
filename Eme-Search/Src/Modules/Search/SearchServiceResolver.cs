using Eme_Search.Modules.Search.Services;

namespace Eme_Search.Modules.Search;

public class SearchServiceResolver
{
    private readonly IEnumerable<ISearchService> _searchServices;

    public SearchServiceResolver(IEnumerable<ISearchService> searchServices)
    {
        _searchServices = searchServices;
    }

    public ISearchService Resolve(string provider)
    {
        var service = _searchServices.FirstOrDefault(s => 
            string.Equals(s.ProviderName, provider, StringComparison.OrdinalIgnoreCase));

        if (service == null)
        {
            throw new InvalidOperationException($"No search service found for provider: {provider}");
        }

        return service;
    }
}
