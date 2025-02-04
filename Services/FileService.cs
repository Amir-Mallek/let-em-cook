using Azure.Storage.Blobs;
using let_em_cook.Services.ServiceContracts;

namespace let_em_cook.Services;

public class FileService : IFileService
{
    const string ContainerName = "images";
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobContainerClient _containerClient;
    
    public FileService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
        _containerClient = GetBlobContainerAsync().Result;
    }
    
    private async Task<BlobContainerClient> GetBlobContainerAsync()
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
        await containerClient.CreateIfNotExistsAsync();
        return containerClient;
    }
    
    public async Task UploadFile(string path, Stream file)
    {
        var blobClient = _containerClient.GetBlobClient(path);
        await blobClient.UploadAsync(file, overwrite: true);
    }
    
    public async Task<Stream> DownloadFile(string path)
    {
        var blobClient = _containerClient.GetBlobClient(path);
        var download = await blobClient.DownloadAsync();
        return download.Value.Content;
    }
    
    public async Task DeleteFile(string path)
    {
        var blobClient = _containerClient.GetBlobClient(path);
        await blobClient.DeleteIfExistsAsync();
    }
}