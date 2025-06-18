using FileAnalysisService.Models;
using FileAnalysisService.Services;
using FileAnalysisService.Data; // для доступа к БД
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FileAnalysisService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnalysisController : ControllerBase
    {
        private readonly AnalysisService _analysisService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AnalysisDbContext _db;

        public AnalysisController(
            AnalysisService analysisService,
            IHttpClientFactory httpClientFactory,
            AnalysisDbContext db)
        {
            _analysisService = analysisService;
            _httpClientFactory = httpClientFactory;
            _db = db;
        }

        // Анализирует файл по id и сохраняет результат в базу, проверяя на дубликаты
        [HttpPost("{fileId}")]
        public async Task<IActionResult> Analyze(Guid fileId)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:5201/Files/{fileId}");
            if (!response.IsSuccessStatusCode)
                return NotFound("File not found in storage service.");

            var text = await response.Content.ReadAsStringAsync();

            var result = _analysisService.Analyze(text, fileId);

            // Проверка на антиплагиат (есть ли такой же hash)
            var duplicate = await _db.AnalysisResults
                .FirstOrDefaultAsync(r => r.FileHash == result.FileHash);

            if (duplicate != null)
                return Conflict($"Файл с идентичным содержимым уже анализировался! (Plagiarism detected, fileId: {duplicate.FileId})");

            // Сохраняем результат в базу
            _db.AnalysisResults.Add(result);
            await _db.SaveChangesAsync();

            return Ok(result);
        }

        // Получить результат анализа по id
        [HttpGet("{fileId}")]
        public async Task<IActionResult> GetAnalysis(Guid fileId)
        {
            var analysis = await _db.AnalysisResults.FindAsync(fileId);
            if (analysis == null)
                return NotFound("Анализ не найден.");
            return Ok(analysis);
        }
    }
}
