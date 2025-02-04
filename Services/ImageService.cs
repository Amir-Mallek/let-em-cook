using let_em_cook.Models;
using let_em_cook.Repositories;
using let_em_cook.Services.ServiceContracts;

namespace let_em_cook.Services;

public class ImageService : IImageService
{
    private readonly IRepository<Recipe> _recipeRepository;
    private readonly IRepository<Image> _imageRepository;
    private readonly IFileService _fileService;
    
    public ImageService(
        IRepository<Recipe> recipeRepository,
        IRepository<Image> imageRepository,
        IFileService fileService)
    {
        _recipeRepository = recipeRepository;
        _imageRepository = imageRepository;
        _fileService = fileService;
    }
    
    public async Task<Image> UploadImage(int recipeId, IFormFile imageForm, string userId)
    {
        if (
            imageForm.ContentType != "image/jpeg" && 
            imageForm.ContentType != "image/png") 
        {
            throw new BadHttpRequestException("Invalid image format.");
        }
        
        var recipe = await _recipeRepository.GetByIdAsync(recipeId);
        if (recipe == null)
        {
            throw new BadHttpRequestException("Recipe not found.");
        }
        if (recipe.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not the owner of this recipe.");
        }
        
        var newImage = new Image
        {
            ContentType = imageForm.ContentType,
            RecipeId = recipeId
        };
        await _imageRepository.AddAsync(newImage);
        await _imageRepository.SaveChangesAsync();
        
        var path = $"{recipe.RecipeId}/{newImage.ImageId}";
        var stream = imageForm.OpenReadStream();
        await _fileService.UploadFile(path, stream);
        
        return newImage;
    }
    
    public async Task<IEnumerable<Image>> GetRecipeImages(int recipeId)
    {
        return await _imageRepository.FindAsync(im => im.RecipeId == recipeId);
    }
    
    public async Task<Image> DownloadImage(int imageId)
    {
        var image = await _imageRepository.GetByIdAsync(imageId);
        if (image == null)
        {
            throw new BadHttpRequestException("Image not found.");
        }
        
        var path = $"{image.RecipeId}/{image.ImageId}";
        image.DataStream = await _fileService.DownloadFile(path);
        return image;
    }
    
    public async Task DeleteImage(int imageId, string userId)
    {
        var image = await _imageRepository.GetByIdAsync(imageId);
        if (image == null)
        {
            throw new BadHttpRequestException("Image not found.");
        }
        
        var recipe = await _recipeRepository.GetByIdAsync(image.RecipeId);
        if (recipe.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not the owner of this recipe.");
        }
        
        var path = $"{recipe.RecipeId}/{image.ImageId}";
        await _fileService.DeleteFile(path);
        
        _imageRepository.Delete(image);
        await _imageRepository.SaveChangesAsync();
    }
    
}