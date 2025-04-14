using Hangfire;
using Microsoft.EntityFrameworkCore;
using StealAllTheCats.DTOs;
using StealAllTheCats.External.TheCatApi.Models;
using StealAllTheCats.Infrastructure.Repositories;
using StealAllTheCats.Integrations.TheCatApi;

namespace StealAllTheCats.Services;

public class CatService(ICatApiClient catApiClient,
                        ICatRepository catRepository,
                        ITagRepository tagRepository,
                        IFileDownloader fileDownloader, 
                        IBackgroundJobClient backgroundJobClient) : ICatService
{
    public async Task<CatDto?> GetCatByIdAsync(string id)
    {
        CatEntity cat = await catRepository.GetCatAsync(id);
        if (cat == null)
            return null;

        return new CatDto(
            cat.Id,
            cat.CatId,
            cat.Width,
            cat.Height,
            cat.Image,
            [.. cat.CatTags.Select(ct => ct.Tag.Name)],
            cat.Created
        );
    }

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
                c.Image,
                c.CatTags.Select(ct => ct.Tag.Name).ToList(),
                c.Created
            ))
            .ToListAsync();

        return new PaginatedList<CatDto>(items, totalCount);
    }


    public string EnqueueCatFetchJob()
    {
        string jobId = backgroundJobClient.Enqueue(() => FetchAndStoreCatsAsync());
        return jobId;
    }

    public async Task FetchAndStoreCatsAsync()
    {
        List<BaseCatApiResponse> apiCats = await GetCatsFromTheCatAPI();

        foreach (BaseCatApiResponse apiCat in apiCats)
        {
            if (await catRepository.ExistsAsync(apiCat.Id)) continue;

            byte[] imageBytes = await fileDownloader.DownloadFileAsync(apiCat.Url);

            Task<CatApiResponse> catApiResponseTask = catApiClient.GetImageInfoByIdAsync(apiCat.Id);

            if (imageBytes == null || imageBytes.Length == 0) continue;

            CatEntity catEntity = new()
            {
                CatId = apiCat.Id,
                Width = apiCat.Width,
                Height = apiCat.Height,
                Image = imageBytes,
                Created = DateTime.UtcNow
            };

            List<TagEntity> tags = [];

            CatApiResponse catApiResponse = await catApiResponseTask;

            foreach (Breed breed in catApiResponse.Breeds)
            {
                if (string.IsNullOrWhiteSpace(breed.Temperament)) continue;

                string[] tagNames = breed.Temperament.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (string? name in tagNames.Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    TagEntity? tag = await tagRepository.FindTagByNameAsync(name);
                    if (tag == null)
                    {
                        tag = new TagEntity
                        {
                            Name = name,
                            Created = DateTime.UtcNow
                        };
                        await tagRepository.AddTagAsync(tag);
                    }
                    tags.Add(tag);
                }
            }

            foreach (TagEntity? tag in tags.DistinctBy(t => t.Name))
            {
                catEntity.CatTags.Add(new CatTag
                {
                    Cat = catEntity,
                    Tag = tag
                });
            }

            await catRepository.AddCatAsync(catEntity);
        }

        await catRepository.SaveChangesAsync();
    }

    public async Task<List<BaseCatApiResponse>> GetCatsFromTheCatAPI()
    {
        List<BaseCatApiResponse> apiCats = await catApiClient.GetCatsAsync(limit: 25, has_breeds: 1);

        // If we don't have enough cats, it could mean that we don't have a valid API key
        // Without a valid Key, we always fetch 10 cats (api query parameters are not really working)
        // Run multiple iterations to get more unique cats, stop at 5 runs.
        int maxIterations = 5;
        int run = 0;
        while (apiCats.Count < 25 || maxIterations <= run)
        {
            apiCats.AddRange(await catApiClient.GetCatsAsync(limit: 10, has_breeds: 1));

            apiCats = [.. apiCats.DistinctBy(c => c.Id).Take(25)];
            run++;
        }

        return apiCats;
    }
}
