using Microsoft.EntityFrameworkCore;

namespace StealAllTheCats.Infrastructure.Data.Repositories;

public class CatRepository(ICatDBContext context, ILogger<CatRepository> logger) : ICatRepository
{
    public async Task<bool> ExistsAsync(string catId) => 
        await context.Cats.AnyAsync(c => c.CatId == catId);

    public async Task AddCatAsync(CatEntity cat)
    {
        await context.Cats.AddAsync(cat);
        await context.SaveAsync();
    }

    public async Task SaveChangesAsync() => await context.SaveAsync();

    public IQueryable<CatEntity> GetCatsAsync() => context.Cats.Include(c => c.CatTags).ThenInclude(ct => ct.Tag);

    public async Task<CatEntity> GetCatAsync(string catId)
    {
        CatEntity catEntity = await context.Cats?.Include(c => c.CatTags).ThenInclude(ct => ct.Tag).FirstOrDefaultAsync(c => c.CatId == catId);

        return catEntity;
    }
}
