using ContentMasterAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContentMasterAPI.Core.Interfaces
{
    /// <summary>
    /// Interface for content repository operations.
    /// </summary>
    public interface IContentRepository
    {
        /// <summary>
        /// Retrieves all content items asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of content items.</returns>
        Task<IEnumerable<Content>> GetAllAsync();

        /// <summary>
        /// Retrieves a content item by its unique identifier asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the content item.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the content item.</returns>
        Task<Content> GetByIdAsync(Guid id);

        /// <summary>
        /// Creates a new content item asynchronously.
        /// </summary>
        /// <param name="content">The content item to create.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created content item.</returns>
        Task<Content> CreateAsync(Content content);

        /// <summary>
        /// Updates an existing content item asynchronously.
        /// </summary>
        /// <param name="content">The content item to update.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated content item.</returns>
        Task<Content> UpdateAsync(Content content);

        /// <summary>
        /// Deletes a content item by its unique identifier asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the content item to delete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the deletion was successful.</returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Searches for content items based on a search term, content type, and tags asynchronously.
        /// </summary>
        /// <param name="searchTerm">The search term to use.</param>
        /// <param name="contentType">The type of content to filter by (optional).</param>
        /// <param name="tags">The tags to filter by (optional).</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of content items that match the search criteria.</returns>
        Task<IEnumerable<Content>> SearchAsync(string searchTerm, string contentType = null, List<string> tags = null);

        /// <summary>
        /// Retrieves all content items for GraphQL queries asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of content items.</returns>
        Task<IEnumerable<Content>> GetAllContentsAsync();

        /// <summary>
        /// Retrieves a content item by its unique identifier for GraphQL queries asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the content item.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the content item.</returns>
        Task<Content> GetContentByIdAsync(Guid id);

        /// <summary>
        /// Retrieves content items by their type for GraphQL queries asynchronously.
        /// </summary>
        /// <param name="contentType">The type of content to filter by.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of content items that match the specified type.</returns>
        Task<IEnumerable<Content>> GetContentsByTypeAsync(string contentType);

        /// <summary>
        /// Retrieves content items by their status for GraphQL queries asynchronously.
        /// </summary>
        /// <param name="status">The status of the content items to filter by.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of content items that match the specified status.</returns>
        Task<IEnumerable<Content>> GetContentsByStatusAsync(string status);

        /// <summary>
        /// Retrieves content items by their creator for GraphQL queries asynchronously.
        /// </summary>
        /// <param name="createdBy">The creator of the content items to filter by.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of content items that match the specified creator.</returns>
        Task<IEnumerable<Content>> GetContentsByCreatorAsync(string createdBy);

        /// <summary>
        /// Retrieves a content item by its unique identifier synchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the content item.</param>
        /// <returns>The content item.</returns>
        Content GetContentById(Guid id);

        /// <summary>
        /// Retrieves all content items synchronously.
        /// </summary>
        /// <returns>A collection of content items.</returns>
        IEnumerable<Content> GetAllContent();
    }
}
