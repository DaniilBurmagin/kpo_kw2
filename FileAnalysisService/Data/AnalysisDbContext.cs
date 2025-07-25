﻿using Microsoft.EntityFrameworkCore;
using FileAnalysisService.Models;

namespace FileAnalysisService.Data
{
    public class AnalysisDbContext : DbContext
    {
        public AnalysisDbContext(DbContextOptions<AnalysisDbContext> options) : base(options) { }
        public DbSet<AnalysisResult> AnalysisResults { get; set; }
    }
}
