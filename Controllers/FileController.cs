using Microsoft.AspNetCore.Mvc;
using let_em_cook.Services;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace let_em_cook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly BlobService _blobService;

        public FileController(BlobService blobService)
        {
            _blobService = blobService;
        }

        // POST api/file/upload
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var containerName = "container";  // Define your container name
            var fileName = file.FileName;

            // Open the file stream
            using var stream = file.OpenReadStream();

            // Upload the file to Blob Storage
            await _blobService.UploadFileAsync(containerName, fileName, stream);

            return Ok(new { Message = "File uploaded successfully." });
        }

        // GET api/file/download/{fileName}
        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            var containerName = "container";  // Define your container name
            var fileStream = await _blobService.DownloadFileAsync(containerName, fileName);

            return File(fileStream, "application/octet-stream", fileName);  // Return the file for download
        }
    }
}
