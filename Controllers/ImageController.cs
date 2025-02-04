using System.Security.Claims;
using let_em_cook.Models;
using let_em_cook.Services.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace let_em_cook.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImageController : Controller
{
    private readonly IImageService _imageService;
    
    public ImageController(IImageService imageService)
    {
        _imageService = imageService;
    }
    
    [HttpPost("{recipeId:int}")]
    [Authorize]
    public async Task<Image> UploadImage(int recipeId, IFormFile image)
    {
        var userId = User.Claims.First(c => c.Type == ClaimTypes.Name).Value;
        return await _imageService.UploadImage(recipeId, image, userId);
    }
    
    [HttpGet("/all/{recipeId:int}")]
    [AllowAnonymous]
    public async Task<IEnumerable<Image>> DownloadRecipeImages(int recipeId)
    {
        return await _imageService.GetRecipeImages(recipeId);
    }
    
    [HttpGet("{imageId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> DownloadImage(int imageId)
    {
        var image = await _imageService.DownloadImage(imageId);
        return File(image.DataStream, image.ContentType);
    }
    
    [HttpDelete("{imageId:int}")]
    [Authorize]
    public async Task DeleteImage(int imageId)
    {
        var userId = User.Claims.First(c => c.Type == ClaimTypes.Name).Value;
        await _imageService.DeleteImage(imageId, userId);
    }
}