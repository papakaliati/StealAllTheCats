namespace StealAllTheCats.Infrastructure.Data.Repositories;

public interface ITagRepository
{
    Task<TagEntity> FindTagByNameAsync(string name);
    Task AddTagAsync(TagEntity cat);
}
