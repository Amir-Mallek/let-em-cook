using Microsoft.AspNetCore.Mvc;
using let_em_cook.Services.ServiceContracts;

namespace let_em_cook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var fileName = file.FileName;
            Console.WriteLine(file.ContentType);
            using var stream = file.OpenReadStream();

            await _fileService.UploadFile(fileName, stream);

            return Ok(new { Message = "File uploaded successfully." });
        }

        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            var fileStream = await _fileService.DownloadFile(fileName);

            return File(fileStream, "application/octet-stream", fileName);
        }
    }
}
