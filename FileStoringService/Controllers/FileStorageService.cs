using Microsoft.AspNetCore.Http;

namespace FileStoringService.Services
{
    public class FileStorageService
    {
        private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "StoredFiles");

        public FileStorageService()
        {
            if (!Directory.Exists(_storagePath))
                Directory.CreateDirectory(_storagePath);
        }

        public virtual async Task SaveFileAsync(IFormFile file, Guid id)
        {
            var filePath = Path.Combine(_storagePath, $"{id}.txt");
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
        }

        public virtual async Task<Stream> GetFileAsync(Guid id)
        {
            var filePath = Path.Combine(_storagePath, $"{id}.txt");
            if (!File.Exists(filePath))
                throw new FileNotFoundException();

            return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
    }
}
