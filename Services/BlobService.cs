using Azure.Storage.Blobs;
using System.IO;
using System.Threading.Tasks;

namespace let_em_cook.Services;

public class BlobService
{
    private readonly BlobServiceClient _blobServiceClient;

    public BlobService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    // Create a container if it doesn't exist
    public async Task<BlobContainerClient> GetBlobContainerAsync(string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();
        return containerClient;
    }

    // Upload a file to the blob container
    public async Task UploadFileAsync(string containerName, string fileName, Stream fileStream)
    {
        var containerClient = await GetBlobContainerAsync(containerName);
        var blobClient = containerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(fileStream, overwrite: true);
    }

    // Download a file from the blob container
    public async Task<Stream> DownloadFileAsync(string containerName, string fileName)
    {
        var containerClient = await GetBlobContainerAsync(containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        var download = await blobClient.DownloadAsync();
        return download.Value.Content;
    }

    // List blobs in a container
    public async Task ListBlobsAsync(string containerName)
    {
        var containerClient = await GetBlobContainerAsync(containerName);
        await foreach (var blobItem in containerClient.GetBlobsAsync())
        {
            Console.WriteLine(blobItem.Name);
        }
    }
}
