using Refit;
using StealAllTheCats.External.TheCatApi.Models;

namespace StealAllTheCats.Integrations.TheCatApi;

public interface ICatApiClient
{
    [Get("/v1/images/search")]
    Task<List<BaseCatApiResponse>> GetCatsAsync([Query] int limit = 10, [Query] int has_breeds = 1);

    [Get("/v1/images/{id}")]
    Task<CatApiResponse> GetImageInfoByIdAsync(string id);
}