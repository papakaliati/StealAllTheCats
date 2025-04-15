using StealAllTheCats.Application.Interfaces;
using Minio;
using Minio.DataModel.Args;
using Minio.DataModel.Response;
using StealAllTheCats.Application;

namespace StealAllTheCats.Infrastructure.Minio;

public class MinioFileStorageClient(IMinioClient minio,
                                    ILogger<MinioFileStorageClient> logger) : IFileStorageService
{
    public async Task<bool> UploadImageAsync(byte[] imageBytes, string objectName, string contentType)
    {
        await EnsureBucketExists();

        // Convert byte array to stream
        using MemoryStream stream = new MemoryStream(imageBytes);
        stream.Position = 0; // Ensure the stream position is at the beginning

        logger.LogWarning($"Uploading image to Minio {objectName}, {contentType}");
        PutObjectResponse? r = await minio.PutObjectAsync(new PutObjectArgs()
                .WithBucket(Config.BUCKET_NAME)
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType(contentType));
      
        return r?.Etag != string.Empty;
    }


    public async Task<byte[]> GetImageAsync(string objectName)
    {
        await EnsureBucketExists();

        // Create a memory stream to hold the downloaded data
        using MemoryStream memoryStream = new MemoryStream();

        // Define the arguments for the GetObjectAsync call
        GetObjectArgs getObjectArgs = new GetObjectArgs()
            .WithBucket(Config.BUCKET_NAME)
            .WithObject(objectName)
            .WithCallbackStream(stream =>
            {
                // Copy the data from the response stream to the memory stream
                stream.CopyTo(memoryStream);
            });

        // Execute the GetObjectAsync call
        await minio.GetObjectAsync(getObjectArgs);

        // Return the byte array
        return memoryStream.ToArray();
    }
    private async Task EnsureBucketExists()
    {
        // Check if the bucket exists
        bool found = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(Config.BUCKET_NAME));
        if (!found)
        {
            logger.LogWarning("Minio Bucket not found");
            await minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(Config.BUCKET_NAME));
        }
        else
        {
            logger.LogWarning("Minio Bucket found");
        }
    }
}
