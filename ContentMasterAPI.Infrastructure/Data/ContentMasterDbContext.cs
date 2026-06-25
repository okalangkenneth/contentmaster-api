using ContentMasterAPI.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ContentMasterAPI.Infrastructure.Data
{
    public class ContentMasterDbContext : DbContext
    {
        public ContentMasterDbContext(DbContextOptions<ContentMasterDbContext> options) : base(options)
        {
        }

        public DbSet<Content> Contents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

                entity.Property(e => e.Tags)
                    .HasColumnType("jsonb");

                entity.Property(e => e.Metadata)
                    .HasColumnType("jsonb");

                entity.HasIndex(e => e.ContentType);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.CreatedBy);
            });

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Content>().HasData(
                new Content
                {
                    Id = new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
                    Title = "Getting Started with ContentMaster API",
                    Body = "A comprehensive guide to content management, AI-driven analytics, and GraphQL integration.",
                    ContentType = "article",
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
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
                    Id = new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
                    Title = "Advanced AI Analytics Features",
                    Body = "Sentiment analysis, automatic tagging, content categorization, and intelligent summarization.",
                    ContentType = "article",
                    CreatedAt = new DateTime(2025, 1, 6, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 8, 0, 0, 0, DateTimeKind.Utc),
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
                    Id = new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012"),
                    Title = "Security Best Practices",
                    Body = "RapidAPI key authentication, rate limiting, and other security best practices.",
                    ContentType = "article",
                    CreatedAt = new DateTime(2025, 1, 9, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 9, 0, 0, 0, DateTimeKind.Utc),
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
            );
        }
    }
}
