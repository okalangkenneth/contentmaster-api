using ContentMasterAPI.Core.Interfaces;
using ContentMasterAPI.Core.Models;

namespace ContentMasterAPI.API.GraphQL.Queries
{
    [ExtendObjectType("Query")]
    public class ContentQueries
    {
        [GraphQLDescription("Get a specific content item by ID")]
        public Content GetContent([Service] IContentRepository repository, Guid id)
        {
            return repository.GetContentById(id);
        }

        [GraphQLDescription("Get all content items")]
        public IEnumerable<Content> GetContents([Service] IContentRepository repository)
        {
            return repository.GetAllContent();
        }
    }
}
