namespace StealAllTheCats.Integrations.TheCatApi;

public class FileDownloader(IHttpClientFactory httpClientFactory) : IFileDownloader
{
    public async Task<string> GetContentType(string url)
    {
        using HttpClient client = httpClientFactory.CreateClient();
        // Create an HttpRequestMessage with HttpMethod.Head
        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, url);
        // Send the HEAD request asynchronously
        using HttpResponseMessage response = await client.SendAsync(request);
        response.Content.Headers.TryGetValues("Content-Type", out IEnumerable<string>? contentType);
        return contentType?.FirstOrDefault() ?? string.Empty;
    }

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