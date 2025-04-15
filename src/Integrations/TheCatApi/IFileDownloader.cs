public interface IFileDownloader
{
    Task<byte[]> DownloadFileAsync(string url);

    Task<string> GetContentType(string url);
}
