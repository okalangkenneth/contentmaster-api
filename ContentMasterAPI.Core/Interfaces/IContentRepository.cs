using ContentMasterAPI.Core.Models;

namespace ContentMasterAPI.Core.Interfaces
{
    /// <summary>
    /// Repository interface for content persistence operations.
    /// All methods are async. GraphQL queries use these same methods directly.
    /// </summary>
    public interface IContentRepository
    {
        Task<IEnumerable<Content>> GetAllAsync();
        Task<Content?> GetByIdAsync(Guid id);
        Task<Content> CreateAsync(Content content);
        Task<Content?> UpdateAsync(Content content);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<Content>> SearchAsync(string searchTerm, string contentType = null, List<string> tags = null);
        Task<IEnumerable<Content>> GetContentsByTypeAsync(string contentType);
        Task<IEnumerable<Content>> GetContentsByStatusAsync(string status);
        Task<IEnumerable<Content>> GetContentsByCreatorAsync(string createdBy);
    }
}
