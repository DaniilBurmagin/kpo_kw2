using FileAnalysisService.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace FileAnalysisService.Services
{
    public class AnalysisService
    {
        public AnalysisResult Analyze(string text, Guid fileId)
        {
            // Разделяем по двойным переводам строк — абзацы
            int paragraphCount = Regex.Split(text.Trim(), @"(\r?\n){2,}").Where(s => !string.IsNullOrWhiteSpace(s) && !s.All(char.IsWhiteSpace)).Count();

            // Разделяем по словам — \w+ учитывает только алфавитные и цифры
            int wordCount = Regex.Matches(text, @"\b\w+\b").Count;

            int charCount = text.Length;

            string fileHash;
            using (var sha = SHA256.Create())
            {
                var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
                fileHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }

            return new AnalysisResult
            {
                FileId = fileId,
                ParagraphCount = paragraphCount,
                WordCount = wordCount,
                CharCount = charCount,
                FileHash = fileHash,
                AnalyzedAt = DateTime.UtcNow
            };
        }
    }
}
