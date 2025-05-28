using ContentMasterAPI.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ContentMasterAPI.Infrastructure.Data
{
    /// <summary>
    /// Entity Framework DbContext for ContentMaster API
    /// </summary>
    public class ContentMasterDbContext : DbContext
    {
        public ContentMasterDbContext(DbContextOptions<ContentMasterDbContext> options) : base(options)
        {
        }

        public DbSet<Content> Contents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Content entity
            modelBuilder.Entity<Content>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(500);
                
                entity.Property(e => e.Body)
                    .IsRequired();
                
                entity.Property(e => e.ContentType)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(256);
                
                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50);

                // Configure Tags as JSON (SQL Server 2016+)
                entity.Property(e => e.Tags)
                    .HasConversion(
                        v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                        v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions)null));

                // Configure Metadata as JSON (SQL Server 2016+)
                entity.Property(e => e.Metadata)
                    .HasConversion(
                        v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                        v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions)null));

                // Indexes for better performance
                entity.HasIndex(e => e.ContentType);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.CreatedBy);
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var sampleContents = new[]
            {
                new Content
                {
                    Id = Guid.NewGuid(),
                    Title = "Getting Started with ContentMaster API",
                    Body = "This is a comprehensive guide to help you get started with the ContentMaster API. It covers the basics of content management, AI-driven analytics, and GraphQL integration.",
                    ContentType = "article",
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    UpdatedAt = DateTime.UtcNow.AddDays(-10),
                    CreatedBy = "system",
                    Status = "published",
                    Tags = new List<string> { "guide", "getting-started", "api" },
                    Metadata = new Dictionary<string, string>
                    {
                        { "readTime", "5 minutes" },
                        { "category", "documentation" },
                        { "featured", "true" }
                    },
                    Version = 1
                },
                new Content
                {
                    Id = Guid.NewGuid(),
                    Title = "Advanced AI Analytics Features",
                    Body = "Explore the powerful AI-driven features of ContentMaster API including sentiment analysis, automatic tagging, content categorization, and intelligent summarization. These features help you understand and organize your content automatically.",
                    ContentType = "article",
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatedAt = DateTime.UtcNow.AddDays(-3),
                    CreatedBy = "system",
                    Status = "published",
                    Tags = new List<string> { "ai", "analytics", "sentiment-analysis", "auto-tagging" },
                    Metadata = new Dictionary<string, string>
                    {
                        { "readTime", "8 minutes" },
                        { "category", "tutorial" },
                        { "difficulty", "intermediate" }
                    },
                    Version = 2
                },
                new Content
                {
                    Id = Guid.NewGuid(),
                    Title = "Security Best Practices",
                    Body = "Security is paramount for any API. This guide covers JWT authentication, RapidAPI integration, rate limiting, and other security best practices for the ContentMaster API.",
                    ContentType = "article",
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2),
                    CreatedBy = "system",
                    Status = "draft",
                    Tags = new List<string> { "security", "authentication", "best-practices" },
                    Metadata = new Dictionary<string, string>
                    {
                        { "readTime", "12 minutes" },
                        { "category", "security" },
                        { "priority", "high" }
                    },
                    Version = 1
                }
            };

            modelBuilder.Entity<Content>().HasData(sampleContents);
        }
    }
}
