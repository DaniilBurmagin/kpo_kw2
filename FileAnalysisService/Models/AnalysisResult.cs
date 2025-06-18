using System.ComponentModel.DataAnnotations;

namespace FileAnalysisService.Models
{
    public class AnalysisResult
    {
        [Key]
        public Guid FileId { get; set; }
        public int ParagraphCount { get; set; }
        public int WordCount { get; set; }
        public int CharCount { get; set; }
        public string FileHash { get; set; }
        public DateTime AnalyzedAt { get; set; }
    }
}
