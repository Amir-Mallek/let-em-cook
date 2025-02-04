namespace let_em_cook.Services.ServiceContracts;

public interface IFileService
{
    public Task UploadFile(string path, Stream file);
    
    public Task<Stream> DownloadFile(string path);
    
    public Task DeleteFile(string path);
}