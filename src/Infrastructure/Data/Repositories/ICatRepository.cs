namespace StealAllTheCats.Infrastructure.Data.Repositories;

public interface ICatRepository
{
    Task<bool> ExistsAsync(string catId);
    Task AddCatAsync(CatEntity cat);
    Task SaveChangesAsync();
    IQueryable<CatEntity> GetCatsAsync();
    Task<CatEntity> GetCatAsync(string catId);
}