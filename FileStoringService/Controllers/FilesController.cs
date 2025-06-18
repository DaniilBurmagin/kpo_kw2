using FileStoringService.Models;
using FileStoringService.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileStoringService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly FileStorageService _fileStorageService;

        public FilesController(FileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] FileUploadModel model)
        {
            var file = model.File;
            if (file == null || file.Length == 0)
                return BadRequest("Empty file");

            if (!file.FileName.EndsWith(".txt"))
                return BadRequest("Only .txt files allowed");

            if (file.Length > 5 * 1024 * 1024)
                return BadRequest("File too big (max 5 MB)");

            var id = Guid.NewGuid();
            await _fileStorageService.SaveFileAsync(file, id);

            return Ok(new { id });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFile(Guid id)
        {
            try
            {
                var stream = await _fileStorageService.GetFileAsync(id);
                return File(stream, "text/plain", $"{id}.txt");
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
