using ContentMasterAPI.Core.Interfaces;
using ContentMasterAPI.Core.Models;

namespace ContentMasterAPI.API.GraphQL.Queries
{
    [ExtendObjectType("Query")]
    public class ContentQueries
    {
        [GraphQLDescription("Get a specific content item by ID")]
        public Task<Content> GetContent([Service] IContentRepository repository, Guid id)
        {
            return repository.GetByIdAsync(id);
        }

        [GraphQLDescription("Get all content items")]
        public Task<IEnumerable<Content>> GetContents([Service] IContentRepository repository)
        {
            return repository.GetAllAsync();
        }
    }
}
