using let_em_cook.Models;

namespace let_em_cook.Services.ServiceContracts;

public interface IImageService
{
    public Task<Image> UploadImage(int recipeId, IFormFile imageForm, string userId);
    
    public Task<IEnumerable<Image>> GetRecipeImages(int recipeId);
    
    public Task<Image> DownloadImage(int imageId);
    
    public Task DeleteImage(int imageId, string userId);
}