using ContentMasterAPI.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContentMasterAPI.Core.Interfaces
{
    /// <summary>
    /// Interface for AI-driven content analysis services
    /// </summary>
    public interface IContentAnalysisService
    {
        /// <summary>
        /// Analyzes the sentiment of text content
        /// </summary>
        /// <param name="text">The text to analyze</param>
        /// <returns>A sentiment score between 0 (negative) and 1 (positive)</returns>
        float AnalyzeSentiment(string text);

        /// <summary>
        /// Analyzes the sentiment of content asynchronously
        /// </summary>
        /// <param name="content">The content to analyze</param>
        /// <returns>A sentiment result</returns>
        Task<SentimentResult> AnalyzeSentimentAsync(Content content);

        /// <summary>
        /// Automatically generates tags for content based on its text
        /// </summary>
        /// <param name="content">The content to analyze</param>
        /// <returns>A list of suggested tags</returns>
        List<string> GenerateTags(Content content);

        /// <summary>
        /// Generates a summary of the content
        /// </summary>
        /// <param name="content">The content to summarize</param>
        /// <param name="maxLength">Maximum length of the summary</param>
        /// <returns>A summary of the content</returns>
        string GenerateSummary(Content content, int maxLength = 200);

        /// <summary>
        /// Categorizes content based on its text
        /// </summary>
        /// <param name="content">The content to categorize</param>
        /// <returns>A suggested category</returns>
        string CategorizeContent(Content content);
    }
}
