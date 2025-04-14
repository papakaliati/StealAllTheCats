namespace StealAllTheCats.Infrastructure.Repositories;

public interface ITagRepository
{
    Task<TagEntity> FindTagByNameAsync(string name);
    Task AddTagAsync(TagEntity cat);
}
