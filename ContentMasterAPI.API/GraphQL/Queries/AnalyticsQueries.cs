using ContentMasterAPI.Core.Interfaces;
using ContentMasterAPI.Core.Models;

namespace ContentMasterAPI.API.GraphQL.Queries
{
    [ExtendObjectType("Query")]
    public class AnalyticsQueries
    {
        [GraphQLDescription("Get sentiment analysis for a content item")]
        public async Task<SentimentResult> GetSentiment(
            [Service] IContentAnalysisService analysisService,
            [Service] IContentRepository repository,
            Guid contentId)
        {
            var content = repository.GetContentById(contentId);
            if (content == null)
                return null;

            return await analysisService.AnalyzeSentimentAsync(content);
        }
    }
}
