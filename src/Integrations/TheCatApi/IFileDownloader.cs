public interface IFileDownloader
{
    Task<byte[]> DownloadFileAsync(string url);
}
