using Microsoft.EntityFrameworkCore;

namespace StealAllTheCats.Infrastructure.Data.Repositories;

public class TagRepository(ApplicationDbContext context, ILogger<CatRepository> logger) : ITagRepository
{
    public async Task<TagEntity> FindTagByNameAsync(string name) 
        => await context.Tags.FirstOrDefaultAsync(c => c.Name == name);

    public async Task AddTagAsync(TagEntity tag)
    {
        await context.Tags.AddAsync(tag);
        await context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(string catId) =>
        await context.Cats.AnyAsync(c => c.CatId == catId);

    public async Task AddCatAsync(CatEntity cat)
    {
        await context.Cats.AddAsync(cat);
        await context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync() => await context.SaveChangesAsync();

    public IQueryable<CatEntity> Query() => context.Cats.Include(c => c.CatTags).ThenInclude(ct => ct.Tag);

    public async Task<CatEntity> GetCatAsync(string catId)
    {
        CatEntity catEntity = await context.Cats?.Include(c => c.CatTags).ThenInclude(ct => ct.Tag).FirstOrDefaultAsync(c => c.CatId == catId);

        return catEntity;
    }

}
