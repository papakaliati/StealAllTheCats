using StealAllTheCats.Api.Features.Cats.Shared;

namespace StealAllTheCats.Api.Features.Cats.ListCats;

public interface IListCatsService
{
    Task<PaginatedList<CatDto>> GetCatsAsync(string? tag, int page, int pageSize);
}