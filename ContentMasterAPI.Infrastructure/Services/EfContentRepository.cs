using ContentMasterAPI.Core.Interfaces;
using ContentMasterAPI.Core.Models;
using ContentMasterAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ContentMasterAPI.Infrastructure.Services
{
    public class EfContentRepository : IContentRepository
    {
        private readonly ContentMasterDbContext _context;
        private readonly ILogger<EfContentRepository> _logger;

        public EfContentRepository(ContentMasterDbContext context, ILogger<EfContentRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Content>> GetAllAsync()
        {
            _logger.LogInformation("Getting all content items from database");
            return await _context.Contents
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Content?> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting content with ID {Id} from database", id);
            return await _context.Contents.FindAsync(id);
        }

        public async Task<Content> CreateAsync(Content content)
        {
            _logger.LogInformation("Creating new content with title {Title} in database", content.Title);

            if (content.Id == Guid.Empty)
                content.Id = Guid.NewGuid();

            content.CreatedAt = DateTime.UtcNow;
            content.UpdatedAt = DateTime.UtcNow;
            content.Version = 1;

            _context.Contents.Add(content);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully created content with ID {Id}", content.Id);
            return content;
        }

        public async Task<Content?> UpdateAsync(Content content)
        {
            _logger.LogInformation("Updating content with ID {Id} in database", content.Id);

            var existingContent = await _context.Contents.FindAsync(content.Id);
            if (existingContent == null)
            {
                _logger.LogWarning("Content with ID {Id} not found for update", content.Id);
                return null;
            }

            existingContent.Title = content.Title;
            existingContent.Body = content.Body;
            existingContent.ContentType = content.ContentType;
            existingContent.Status = content.Status;
            existingContent.Tags = content.Tags;
            existingContent.Metadata = content.Metadata;
            existingContent.UpdatedAt = DateTime.UtcNow;
            existingContent.Version++;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully updated content with ID {Id}", content.Id);
            return existingContent;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            _logger.LogInformation("Deleting content with ID {Id} from database", id);

            var content = await _context.Contents.FindAsync(id);
            if (content == null)
            {
                _logger.LogWarning("Content with ID {Id} not found for deletion", id);
                return false;
            }

            _context.Contents.Remove(content);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully deleted content with ID {Id}", id);
            return true;
        }

        public async Task<IEnumerable<Content>> SearchAsync(string searchTerm, string contentType = null, List<string> tags = null)
        {
            _logger.LogInformation("Searching for content with term {SearchTerm}, type {ContentType}, tags {Tags}",
                searchTerm, contentType, string.Join(", ", tags ?? new List<string>()));

            var query = _context.Contents.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var pattern = $"%{searchTerm}%";
                query = query.Where(c =>
                    EF.Functions.ILike(c.Title, pattern) ||
                    EF.Functions.ILike(c.Body, pattern));
            }

            if (!string.IsNullOrEmpty(contentType))
                query = query.Where(c => c.ContentType == contentType);

            if (tags != null && tags.Count > 0)
            {
                foreach (var tag in tags)
                {
                    var currentTag = tag;
                    query = query.Where(c => c.Tags.Contains(currentTag));
                }
            }

            var results = await query
                .OrderByDescending(c => c.UpdatedAt)
                .ToListAsync();

            _logger.LogInformation("Found {Count} content items matching search criteria", results.Count);
            return results;
        }

        public async Task<IEnumerable<Content>> GetContentsByTypeAsync(string contentType)
        {
            _logger.LogInformation("Getting content by type {ContentType} from database", contentType);
            return await _context.Contents
                .Where(c => c.ContentType == contentType)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Content>> GetContentsByStatusAsync(string status)
        {
            _logger.LogInformation("Getting content by status {Status} from database", status);
            return await _context.Contents
                .Where(c => c.Status == status)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Content>> GetContentsByCreatorAsync(string createdBy)
        {
            _logger.LogInformation("Getting content by creator {CreatedBy} from database", createdBy);
            return await _context.Contents
                .Where(c => c.CreatedBy == createdBy)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }
    }
}
