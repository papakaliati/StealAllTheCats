using Microsoft.EntityFrameworkCore;
using StealAllTheCats.Api.Features.Cats.Shared;
using StealAllTheCats.Application;
using StealAllTheCats.Infrastructure.Data.Repositories;

namespace StealAllTheCats.Api.Features.Cats.ListCats;

public class ListCatsService(ICatRepository catRepository) : IListCatsService
{
    public async Task<PaginatedList<CatDto>> GetCatsAsync(string? tag, int page, int pageSize)
    {
        IQueryable<CatEntity> query = catRepository.GetCatsAsync();

        if (!string.IsNullOrWhiteSpace(tag))
        {
            query = query.Where(c => c.CatTags.Any(ct => ct.Tag.Name.ToLower().Equals(tag)));
        }

        int totalCount = await query.CountAsync();
    
        List<CatDto> items = await query
            .OrderByDescending(c => c.Created)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CatDto(
                c.Id,
                c.CatId,
                c.Width,
                c.Height,
                $"http://localhost:9001/browser/{Config.BUCKET_NAME}/{c.Image}",
                c.CatTags.Select(ct => ct.Tag.Name).ToList(),
                c.Created
            ))
            .ToListAsync();

        return new PaginatedList<CatDto>(items, totalCount);
    }
}
