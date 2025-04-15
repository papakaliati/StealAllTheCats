using StealAllTheCats.Api.Features.Cats.Shared;

namespace StealAllTheCats.Api.Features.Cats.GetbyID;

public interface IGetCatByIdService
{
    Task<CatDto?> GetCatByIdAsync(string id);
}
