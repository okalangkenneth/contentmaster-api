using ContentMasterAPI.Core.Interfaces;
using ContentMasterAPI.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Chat;

namespace ContentMasterAPI.Infrastructure.Services
{
    /// <summary>
    /// OpenAI-powered content analysis service
    /// </summary>
    public class OpenAiContentAnalysisService : IContentAnalysisService
    {
        private readonly OpenAIClient _openAiClient;
        private readonly ILogger<OpenAiContentAnalysisService> _logger;
        private readonly IConfiguration _configuration;

        public OpenAiContentAnalysisService(IConfiguration configuration, ILogger<OpenAiContentAnalysisService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var apiKey = _configuration["OpenAI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("OpenAI API key not found in configuration. Please add 'OpenAI:ApiKey' to your appsettings.json");
            }

            _openAiClient = new OpenAIClient(apiKey);
            _logger.LogInformation("OpenAI Content Analysis Service initialized");
        }

        public float AnalyzeSentiment(string text)
        {
            // For synchronous calls, we'll run the async version
            return AnalyzeSentimentAsync(text).GetAwaiter().GetResult();
        }

        public async Task<SentimentResult> AnalyzeSentimentAsync(Content content)
        {
            var sentimentScore = await AnalyzeSentimentAsync(content.Body);

            return new SentimentResult
            {
                ContentId = content.Id,
                Title = content.Title,
                SentimentScore = sentimentScore,
                SentimentLabel = GetSentimentLabel(sentimentScore)
            };
        }

        private async Task<float> AnalyzeSentimentAsync(string text)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    _logger.LogWarning("Empty text provided for sentiment analysis");
                    return 0.5f; // Neutral
                }

                _logger.LogInformation("Analyzing sentiment for text: '{TextSample}'",
                    text.Length > 50 ? text.Substring(0, 50) + "..." : text);

                var messages = new[]
                {
                    new ChatMessage(ChatRole.System, "You are a sentiment analysis expert. Analyze the sentiment of the given text and respond with only a number between 0 and 1, where 0 is very negative, 0.5 is neutral, and 1 is very positive. Consider context, tone, and overall emotional sentiment."),
                    new ChatMessage(ChatRole.User, $"Analyze the sentiment of this text: {text}")
                };

                var chatRequest = new ChatRequest(messages, "gpt-3.5-turbo")
                {
                    MaxTokens = 10,
                    Temperature = 0.1f // Low temperature for consistent results
                };

                var response = await _openAiClient.ChatApi.GetChatCompletionsAsync(chatRequest);

                if (response?.Choices?.Count > 0)
                {
                    var responseText = response.Choices[0].Message.Content.Trim();
                    
                    if (float.TryParse(responseText, out var score))
                    {
                        // Ensure score is within valid range
                        score = Math.Max(0f, Math.Min(1f, score));
                        _logger.LogInformation("OpenAI sentiment analysis result: {Score}", score);
                        return score;
                    }

                    _logger.LogWarning("Unable to parse sentiment score from OpenAI response: {Response}", responseText);
                }

                _logger.LogWarning("No valid response from OpenAI for sentiment analysis");
                return 0.5f; // Default to neutral
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing sentiment with OpenAI");
                // Fallback to simple rule-based analysis
                return AnalyzeSentimentFallback(text);
            }
        }

        public List<string> GenerateTags(Content content)
        {
            try
            {
                _logger.LogInformation("Generating tags for content: {Title}", content.Title);

                var messages = new[]
                {
                    new ChatMessage(ChatRole.System, "You are a content tagging expert. Generate 3-7 relevant tags for the given content. Respond with only the tags separated by commas, no explanations. Focus on key topics, themes, and categories."),
                    new ChatMessage(ChatRole.User, $"Title: {content.Title}\n\nContent: {content.Body}")
                };

                var chatRequest = new ChatRequest(messages, "gpt-3.5-turbo")
                {
                    MaxTokens = 100,
                    Temperature = 0.3f
                };

                var response = _openAiClient.ChatApi.GetChatCompletionsAsync(chatRequest).GetAwaiter().GetResult();

                if (response?.Choices?.Count > 0)
                {
                    var tagsText = response.Choices[0].Message.Content.Trim();
                    var tags = tagsText.Split(',')
                        .Select(tag => tag.Trim().ToLower())
                        .Where(tag => !string.IsNullOrEmpty(tag))
                        .Take(7)
                        .ToList();

                    _logger.LogInformation("Generated {Count} tags for content", tags.Count);
                    return tags;
                }

                _logger.LogWarning("No tags generated by OpenAI");
                return GenerateTagsFallback(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating tags with OpenAI");
                return GenerateTagsFallback(content);
            }
        }

        public string GenerateSummary(Content content, int maxLength = 200)
        {
            try
            {
                _logger.LogInformation("Generating summary for content: {Title}", content.Title);

                var messages = new[]
                {
                    new ChatMessage(ChatRole.System, $"You are a content summarization expert. Create a concise summary of the given content in {maxLength} characters or less. Focus on the main points and key information."),
                    new ChatMessage(ChatRole.User, $"Title: {content.Title}\n\nContent: {content.Body}")
                };

                var chatRequest = new ChatRequest(messages, "gpt-3.5-turbo")
                {
                    MaxTokens = Math.Max(50, maxLength / 3), // Rough token estimation
                    Temperature = 0.2f
                };

                var response = _openAiClient.ChatApi.GetChatCompletionsAsync(chatRequest).GetAwaiter().GetResult();

                if (response?.Choices?.Count > 0)
                {
                    var summary = response.Choices[0].Message.Content.Trim();
                    
                    // Truncate if necessary
                    if (summary.Length > maxLength)
                    {
                        summary = summary.Substring(0, maxLength - 3) + "...";
                    }

                    _logger.LogInformation("Generated summary of length {Length}", summary.Length);
                    return summary;
                }

                _logger.LogWarning("No summary generated by OpenAI");
                return GenerateSummaryFallback(content, maxLength);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating summary with OpenAI");
                return GenerateSummaryFallback(content, maxLength);
            }
        }

        public string CategorizeContent(Content content)
        {
            try
            {
                _logger.LogInformation("Categorizing content: {Title}", content.Title);

                var messages = new[]
                {
                    new ChatMessage(ChatRole.System, "You are a content categorization expert. Categorize the given content into one of these categories only: Technology, Business, Science, Health, Entertainment, Sports, Politics, Education, Lifestyle, News, Tutorial, Documentation, Review, Opinion, or General. Respond with only the category name."),
                    new ChatMessage(ChatRole.User, $"Title: {content.Title}\n\nContent: {content.Body}")
                };

                var chatRequest = new ChatRequest(messages, "gpt-3.5-turbo")
                {
                    MaxTokens = 20,
                    Temperature = 0.1f
                };

                var response = _openAiClient.ChatApi.GetChatCompletionsAsync(chatRequest).GetAwaiter().GetResult();

                if (response?.Choices?.Count > 0)
                {
                    var category = response.Choices[0].Message.Content.Trim();
                    _logger.LogInformation("Categorized content as: {Category}", category);
                    return category;
                }

                _logger.LogWarning("No category generated by OpenAI");
                return CategorizeContentFallback(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error categorizing content with OpenAI");
                return CategorizeContentFallback(content);
            }
        }

        #region Fallback Methods

        private float AnalyzeSentimentFallback(string text)
        {
            // Simple rule-based sentiment analysis as fallback
            var positiveWords = new[] { "good", "great", "excellent", "amazing", "wonderful", "fantastic", "love", "best" };
            var negativeWords = new[] { "bad", "terrible", "awful", "hate", "worst", "horrible", "disappointing", "poor" };

            var words = text.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var positiveCount = words.Count(word => positiveWords.Contains(word));
            var negativeCount = words.Count(word => negativeWords.Contains(word));

            if (positiveCount + negativeCount == 0) return 0.5f;
            return (float)positiveCount / (positiveCount + negativeCount);
        }

        private List<string> GenerateTagsFallback(Content content)
        {
            var tags = new List<string>();
            var text = $"{content.Title} {content.Body}".ToLower();

            // Simple keyword extraction
            if (text.Contains("api")) tags.Add("api");
            if (text.Contains("tutorial")) tags.Add("tutorial");
            if (text.Contains("guide")) tags.Add("guide");
            if (text.Contains("technology")) tags.Add("technology");
            if (text.Contains("programming")) tags.Add("programming");

            return tags.Any() ? tags : new List<string> { "general", content.ContentType };
        }

        private string GenerateSummaryFallback(Content content, int maxLength)
        {
            var summary = content.Body.Length <= maxLength 
                ? content.Body 
                : content.Body.Substring(0, maxLength - 3) + "...";
            
            return summary;
        }

        private string CategorizeContentFallback(Content content)
        {
            var text = $"{content.Title} {content.Body}".ToLower();

            if (text.Contains("api") || text.Contains("programming")) return "Technology";
            if (text.Contains("tutorial") || text.Contains("guide")) return "Tutorial";
            if (text.Contains("news")) return "News";
            if (text.Contains("review")) return "Review";
            
            return "General";
        }

        private string GetSentimentLabel(double score)
        {
            return score switch
            {
                < 0.2 => "Very Negative",
                < 0.4 => "Negative",
                < 0.6 => "Neutral",
                < 0.8 => "Positive",
                _ => "Very Positive"
            };
        }

        #endregion
    }
}
