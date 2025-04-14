using StealAllTheCats.DTOs;
using StealAllTheCats.External.TheCatApi.Models;

namespace StealAllTheCats.Services;

public interface ICatService
{
    string EnqueueCatFetchJob();
    Task<CatDto?> GetCatByIdAsync(string id);
    Task<PaginatedList<CatDto>> GetCatsAsync(string? tag, int page, int pageSize);
    Task<List<BaseCatApiResponse>> GetCatsFromTheCatAPI();
}
