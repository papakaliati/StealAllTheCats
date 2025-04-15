using StealAllTheCats.Api.Features.Cats.Shared;
using StealAllTheCats.Application;
using StealAllTheCats.Application.Interfaces;
using StealAllTheCats.Infrastructure.Data.Repositories;

namespace StealAllTheCats.Api.Features.Cats.GetbyID;

public class GetCatByIdService(ICatRepository catRepository) : IGetCatByIdService
{
    public async Task<CatDto?> GetCatByIdAsync(string id)
    {
        CatEntity? cat = await catRepository.GetCatAsync(id);
        if (cat == null)
            return null;

        return new CatDto(
            cat.Id,
            cat.CatId,
            cat.Width,
            cat.Height,
            $"http://localhost:9001/browser/{Config.BUCKET_NAME}/{cat.Image}",
            [.. cat.CatTags.Select(ct => ct.Tag.Name)],
            cat.Created
        );
    }
}
