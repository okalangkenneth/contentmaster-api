using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContentMasterAPI.Core.Interfaces;
using ContentMasterAPI.Core.Models;
using Microsoft.Extensions.Logging;

namespace ContentMasterAPI.Infrastructure.Services
{
    /// <summary>
    /// In-memory implementation of the content repository for development and testing
    /// </summary>
    public class InMemoryContentRepository : IContentRepository
    {
        private readonly List<Content> _contents;
        private readonly ILogger<InMemoryContentRepository> _logger;

        public InMemoryContentRepository(ILogger<InMemoryContentRepository> logger)
        {
            _logger = logger;
            _contents = new List<Content>();

            // Add some sample data
            SeedData();
        }

        public async Task<IEnumerable<Content>> GetAllAsync()
        {
            _logger.LogInformation("Getting all content items");
            return await Task.FromResult(_contents);
        }

        public async Task<Content?> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting content with ID {Id}", id);
            return await Task.FromResult(_contents.FirstOrDefault(c => c.Id == id));
        }

        public async Task<Content> CreateAsync(Content content)
        {
            _logger.LogInformation("Creating new content with title {Title}", content.Title);

            if (content.Id == Guid.Empty)
            {
                content.Id = Guid.NewGuid();
            }

            content.CreatedAt = DateTime.UtcNow;
            content.UpdatedAt = DateTime.UtcNow;
            content.Version = 1;

            _contents.Add(content);
            return await Task.FromResult(content);
        }

        public async Task<Content?> UpdateAsync(Content content)
        {
            _logger.LogInformation("Updating content with ID {Id}", content.Id);

            var existingContent = _contents.FirstOrDefault(c => c.Id == content.Id);
            if (existingContent == null)
            {
                return null;
            }

            // Remove the existing item
            _contents.Remove(existingContent);

            // Update the version and timestamps
            content.Version = existingContent.Version + 1;
            content.UpdatedAt = DateTime.UtcNow;
            content.CreatedAt = existingContent.CreatedAt;

            // Add the updated item
            _contents.Add(content);

            return await Task.FromResult(content);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            _logger.LogInformation("Deleting content with ID {Id}", id);

            var content = _contents.FirstOrDefault(c => c.Id == id);
            if (content == null)
            {
                return await Task.FromResult(false);
            }

            _contents.Remove(content);
            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<Content>> SearchAsync(string searchTerm, string contentType = null, List<string> tags = null)
        {
            _logger.LogInformation("Searching for content with term {SearchTerm}", searchTerm);

            var query = _contents.AsQueryable();

            // Apply search term filter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c =>
                    c.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    c.Body.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            // Apply content type filter
            if (!string.IsNullOrEmpty(contentType))
            {
                query = query.Where(c => c.ContentType.Equals(contentType, StringComparison.OrdinalIgnoreCase));
            }

            // Apply tags filter
            if (tags != null && tags.Count > 0)
            {
                query = query.Where(c => c.Tags != null && c.Tags.Any(t => tags.Contains(t, StringComparer.OrdinalIgnoreCase)));
            }

            return await Task.FromResult(query.ToList());
        }

        public async Task<IEnumerable<Content>> GetContentsByTypeAsync(string contentType)
        {
            _logger.LogInformation("Getting content by type {ContentType}", contentType);
            return await SearchAsync(null, contentType);
        }

        public async Task<IEnumerable<Content>> GetContentsByStatusAsync(string status)
        {
            _logger.LogInformation("Getting content by status {Status}", status);
            return await Task.FromResult(_contents.Where(c =>
                c.Status.Equals(status, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<IEnumerable<Content>> GetContentsByCreatorAsync(string createdBy)
        {
            _logger.LogInformation("Getting content by creator {CreatedBy}", createdBy);
            return await Task.FromResult(_contents.Where(c =>
                c.CreatedBy.Equals(createdBy, StringComparison.OrdinalIgnoreCase)));
        }



        private void SeedData()
        {
            // Add some sample content items
            _contents.Add(new Content
            {
                Id = Guid.NewGuid(),
                Title = "Getting Started with ContentMaster API",
                Body = "This is a guide to help you get started with the ContentMaster API. It covers the basics of content management.",
                ContentType = "article",
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-10),
                CreatedBy = "system",
                Status = "published",
                Tags = new List<string> { "guide", "getting-started", "api" },
                Metadata = new Dictionary<string, string>
                {
                    { "readTime", "5 minutes" },
                    { "category", "documentation" }
                },
                Version = 1
            });

            _contents.Add(new Content
            {
                Id = Guid.NewGuid(),
                Title = "Advanced Content Management Techniques",
                Body = "Learn advanced techniques for managing content with the ContentMaster API, including versioning and metadata.",
                ContentType = "article",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-3),
                CreatedBy = "system",
                Status = "published",
                Tags = new List<string> { "advanced", "techniques", "metadata" },
                Metadata = new Dictionary<string, string>
                {
                    { "readTime", "10 minutes" },
                    { "category", "tutorial" }
                },
                Version = 2
            });

            _contents.Add(new Content
            {
                Id = Guid.NewGuid(),
                Title = "ContentMaster API Security Best Practices",
                Body = "Security is critical for any API. This guide covers best practices for securing your ContentMaster API implementation.",
                ContentType = "article",
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-2),
                CreatedBy = "system",
                Status = "draft",
                Tags = new List<string> { "security", "best-practices", "api" },
                Metadata = new Dictionary<string, string>
                {
                    { "readTime", "8 minutes" },
                    { "category", "security" }
                },
                Version = 1
            });
        }
    }
}