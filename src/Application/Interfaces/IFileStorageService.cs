namespace StealAllTheCats.Application.Interfaces;

public interface IFileStorageService
{
    Task<bool> UploadImageAsync(byte[] imageBytes, string objectName, string contentType);
    Task<byte[]> GetImageAsync(string objectName);
}
