using Hangfire;
using StealAllTheCats.Application.Interfaces;
using StealAllTheCats.Infrastructure.Data.Repositories;
using StealAllTheCats.Integrations.TheCatApi;
using StealAllTheCats.Integrations.TheCatApi.Models;

namespace StealAllTheCats.Api.Features.Cats.Fetch;

public class FetchCatsService(ICatApiClient catApiClient,
                              ICatRepository catRepository,
                              ITagRepository tagRepository,
                              IFileDownloader fileDownloader,
                              IFileStorageService fileStorageService,
                              IBackgroundJobClient backgroundJobClient) : IFetchCatsService
{
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

            if (imageBytes == null || imageBytes.Length == 0) continue;

            string contentType = await fileDownloader.GetContentType(apiCat.Url);
            byte[] data = await fileDownloader.DownloadFileAsync(apiCat.Url);
            bool wasUploaded = await fileStorageService.UploadImageAsync(data, apiCat.Id, contentType);

            CatEntity catEntity = new()
            {
                CatId = apiCat.Id,
                Width = apiCat.Width,
                Height = apiCat.Height,
                Image = apiCat.Id,
                Created = DateTime.UtcNow
            };

            List<TagEntity> tags = [];

            CatApiResponse catApiResponse = await catApiClient.GetImageInfoByIdAsync(apiCat.Id);

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

    private async Task<List<BaseCatApiResponse>> GetCatsFromTheCatAPI()
    {
        List<BaseCatApiResponse> apiCats = await catApiClient.GetCatsAsync(limit: 25, has_breeds: 1);

        // If we don't have enough cats, it could mean that we don't have a valid API key
        // Without a valid Key, we always fetch 10 cats (api query parameters are not really working)
        // Run multiple iterations to get more unique cats, stop at 5 runs.
        int maxIterations = 5;
        int run = 0;
        while (apiCats.Count < 25)
        {
            apiCats.AddRange(await catApiClient.GetCatsAsync(limit: 10, has_breeds: 1));

            apiCats = [.. apiCats.DistinctBy(c => c.Id).Take(25)];
            run++;
            if (run >= maxIterations)
                break;
        }

        return apiCats;
    }
}
