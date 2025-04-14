namespace StealAllTheCats.Integrations.TheCatApi;

public class FileDownloader(IHttpClientFactory httpClientFactory) : IFileDownloader
{
    public async Task<byte[]> DownloadFileAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) {
            return [];
        }

        try
        {
            using HttpClient client = httpClientFactory.CreateClient();
            return await client.GetByteArrayAsync(url);
        }
        catch {
            return [];
        }
    }
}