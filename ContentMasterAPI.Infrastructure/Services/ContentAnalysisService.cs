using ContentMasterAPI.Core.Interfaces;
using ContentMasterAPI.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Text.RegularExpressions;

namespace ContentMasterAPI.Infrastructure.Services
{
    /// <summary>
    /// Enhanced service for AI-driven content analysis with improved negative sentiment detection
    /// </summary>
    public class ContentAnalysisService : IContentAnalysisService
    {
        private readonly ILogger<ContentAnalysisService> _logger;
        private readonly MLContext _mlContext;
        private readonly PredictionEngine<SentimentData, SentimentPrediction> _sentimentPredictionEngine;

        // Enhanced negative word lists for better detection
        private readonly HashSet<string> _strongNegativeWords;
        private readonly HashSet<string> _negativeWords;
        private readonly HashSet<string> _positiveWords;

        public ContentAnalysisService(ILogger<ContentAnalysisService> logger)
        {
            _logger = logger;
            _mlContext = new MLContext(seed: 0);

            // Initialize word lists for enhanced sentiment analysis
            InitializeSentimentWordLists(out _strongNegativeWords, out _negativeWords, out _positiveWords);

            // Build and train sentiment analysis model
            _sentimentPredictionEngine = CreateSentimentAnalysisModel();

            _logger.LogInformation("Enhanced ContentAnalysisService initialized");
        }

        public async Task<SentimentResult> AnalyzeSentimentAsync(Content content)
        {
            // This is a simple implementation that wraps the synchronous method
            float score = AnalyzeSentiment(content.Body);

            return await Task.FromResult(new SentimentResult
            {
                ContentId = content.Id,
                Title = content.Title,
                SentimentScore = score,
                SentimentLabel = score > 0.5 ? "positive" : "negative"
            });
        }


        private void InitializeSentimentWordLists(out HashSet<string> strongNegativeWords, out HashSet<string> negativeWords, out HashSet<string> positiveWords)
        {
            // Strong negative words (higher weight in sentiment calculation)
            strongNegativeWords = new HashSet<string>
            {
                "terrible", "horrible", "awful", "dreadful", "abysmal",
                "unusable", "worthless", "useless", "pathetic", "abhorrent",
                "catastrophic", "disastrous", "appalling", "atrocious", "deplorable",
                "frustrating", "infuriating", "disappointing", "disgusting", "unacceptable"
            };

            // Regular negative words
            negativeWords = new HashSet<string>
            {
                "bad", "poor", "negative", "slow", "unreliable", "faulty",
                "problem", "issue", "error", "bug", "glitch", "defect",
                "confusing", "difficult", "complicated", "complex", "hard",
                "lacking", "missing", "insufficient", "inadequate", "incomplete",
                "broken", "fails", "failing", "failed", "crash", "crashes",
                "laggy", "delay", "delayed", "sluggish", "unresponsive",
                "inconsistent", "unstable", "unpredictable", "irregular",
                "costly", "expensive", "overpriced", "waste", "inefficient"
            };

            // Positive words
            positiveWords = new HashSet<string>
            {
                "good", "great", "excellent", "amazing", "wonderful", "fantastic",
                "outstanding", "exceptional", "brilliant", "superb", "terrific",
                "helpful", "useful", "beneficial", "valuable", "worthwhile",
                "easy", "simple", "straightforward", "intuitive", "user-friendly",
                "reliable", "stable", "consistent", "dependable", "robust",
                "fast", "quick", "speedy", "responsive", "efficient",
                "innovative", "creative", "novel", "unique", "original",
                "comprehensive", "complete", "thorough", "detailed", "extensive",
                "effective", "successful", "powerful", "strong", "impressive"
            };
        }

        private PredictionEngine<SentimentData, SentimentPrediction> CreateSentimentAnalysisModel()
        {
            try
            {
                // Create an enhanced training dataset with more negative examples and better weighting
                var sentimentData = new List<SentimentData>
                {
                    // Strong negative examples (with higher weight)
                    new SentimentData { Text = "This API is terrible and completely unusable", Sentiment = false, Weight = 2.0f },
                    new SentimentData { Text = "Horrible experience with constant bugs and crashes", Sentiment = false, Weight = 2.0f },
                    new SentimentData { Text = "Absolutely awful service with unresponsive support", Sentiment = false, Weight = 2.0f },
                    new SentimentData { Text = "The documentation is a complete disaster", Sentiment = false, Weight = 2.0f },
                    new SentimentData { Text = "Frustrating and disappointing in every aspect", Sentiment = false, Weight = 2.0f },
                    
                    // Regular negative examples
                    new SentimentData { Text = "This API is disappointing and unreliable", Sentiment = false },
                    new SentimentData { Text = "Poor performance and confusing documentation", Sentiment = false },
                    new SentimentData { Text = "The service has many bugs and issues", Sentiment = false },
                    new SentimentData { Text = "Slow response times and frequent errors", Sentiment = false },
                    new SentimentData { Text = "Lacking essential features and difficult to use", Sentiment = false },
                    new SentimentData { Text = "Not worth the money and time invested", Sentiment = false },
                    new SentimentData { Text = "Support team never responds to critical issues", Sentiment = false },
                    new SentimentData { Text = "Complicated setup with insufficient documentation", Sentiment = false },
                    new SentimentData { Text = "Unstable and breaks frequently in production", Sentiment = false },
                    new SentimentData { Text = "The worst API service I have ever used", Sentiment = false },
                    new SentimentData { Text = "Constantly failing with unpredictable errors", Sentiment = false },
                    new SentimentData { Text = "Waste of time and resources for our project", Sentiment = false },
                    new SentimentData { Text = "Missing critical functionality and inconsistent", Sentiment = false },
                    new SentimentData { Text = "This API is frustratingly slow and unreliable. The documentation is confusing, lacks proper examples, and contains several errors.", Sentiment = false, Weight = 1.5f },
                    
                    // Neutral examples
                    new SentimentData { Text = "This is an API for content management", Sentiment = true, Weight = 0.5f },
                    new SentimentData { Text = "Documentation describes the available endpoints", Sentiment = true, Weight = 0.5f },
                    new SentimentData { Text = "The service provides both basic and advanced features", Sentiment = true, Weight = 0.5f },
                    new SentimentData { Text = "API requires authentication token for access", Sentiment = true, Weight = 0.5f },
                    new SentimentData { Text = "Multiple pricing tiers are available", Sentiment = true, Weight = 0.5f },
                    
                    // Positive examples
                    new SentimentData { Text = "Excellent API with comprehensive documentation", Sentiment = true },
                    new SentimentData { Text = "Great performance and reliability", Sentiment = true },
                    new SentimentData { Text = "Easy to integrate and user-friendly", Sentiment = true },
                    new SentimentData { Text = "Responsive support team and helpful community", Sentiment = true },
                    new SentimentData { Text = "Fantastic service that saved us development time", Sentiment = true },
                    new SentimentData { Text = "Highly recommended for production applications", Sentiment = true },
                    new SentimentData { Text = "Well-designed API with intuitive endpoints", Sentiment = true },
                    new SentimentData { Text = "Stable and consistent performance under load", Sentiment = true },
                    new SentimentData { Text = "The documentation is outstanding and complete", Sentiment = true },
                    new SentimentData { Text = "Worth every penny, significantly improved our workflow", Sentiment = true }
                };

                // Create training data
                var trainingData = _mlContext.Data.LoadFromEnumerable(sentimentData);

                // Define a simpler training pipeline that should work with ML.NET
                var pipeline = _mlContext.Transforms.Text.FeaturizeText("Features", nameof(SentimentData.Text))
                    .Append(_mlContext.Transforms.NormalizeMinMax("Features"))
                    .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                        labelColumnName: "Label",
                        featureColumnName: "Features",
                        exampleWeightColumnName: "Weight"));

                // Train the model
                var model = pipeline.Fit(trainingData);

                // Create prediction engine
                return _mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating sentiment analysis model: {Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Analyzes the sentiment of text content with enhanced negative detection
        /// </summary>
        /// <param name="text">The text to analyze</param>
        /// <returns>A sentiment score between 0 (negative) and 1 (positive)</returns>
        public float AnalyzeSentiment(string text)
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

                // Hybrid approach: combine ML prediction with rule-based analysis

                // 1. Get ML.NET prediction
                var prediction = _sentimentPredictionEngine.Predict(new SentimentData { Text = text });
                float mlScore = prediction.Probability;

                _logger.LogInformation("ML model sentiment score: {Score}", mlScore);

                // 2. Perform rule-based analysis for better negative detection
                float ruleBasedScore = CalculateRuleBasedSentiment(text);
                _logger.LogInformation("Rule-based sentiment score: {Score}", ruleBasedScore);

                // 3. Combine scores with weights favoring negative detection
                // If rule-based score is significantly lower than ML score, give it more weight
                float finalScore;
                if (ruleBasedScore < 0.4 && mlScore > 0.5)
                {
                    // When rule-based analysis detects strong negative sentiment
                    // but ML doesn't, favor the rule-based approach
                    finalScore = (ruleBasedScore * 0.7f) + (mlScore * 0.3f);
                    _logger.LogInformation("Favoring rule-based negative sentiment");
                }
                else if (Math.Abs(mlScore - ruleBasedScore) < 0.2)
                {
                    // When scores are close, take a balanced approach
                    finalScore = (mlScore + ruleBasedScore) / 2;
                    _logger.LogInformation("Using balanced sentiment calculation");
                }
                else
                {
                    // Otherwise, slightly favor ML approach
                    finalScore = (mlScore * 0.6f) + (ruleBasedScore * 0.4f);
                    _logger.LogInformation("Favoring ML-based sentiment with rule adjustment");
                }

                _logger.LogInformation("Final sentiment score: {Score}", finalScore);
                return finalScore;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing sentiment, falling back to simple analysis");
                return SimpleTextBasedSentiment(text);
            }
        }

        private float CalculateRuleBasedSentiment(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0.5f;

            string normalizedText = text.ToLower();

            // Break text into words
            var words = Regex.Split(normalizedText, @"\W+")
                .Where(w => !string.IsNullOrEmpty(w))
                .ToList();

            // Count sentiment words
            int strongNegativeCount = 0;
            int negativeCount = 0;
            int positiveCount = 0;

            foreach (var word in words)
            {
                if (_strongNegativeWords.Contains(word))
                    strongNegativeCount++;
                else if (_negativeWords.Contains(word))
                    negativeCount++;
                else if (_positiveWords.Contains(word))
                    positiveCount++;
            }

            // Check for negation patterns that could reverse sentiment
            int negationCount = CountNegations(normalizedText);

            // Log word counts for debugging
            _logger.LogInformation("Word counts - Strong Negative: {StrongNeg}, Negative: {Neg}, Positive: {Pos}, Negations: {Negations}",
                strongNegativeCount, negativeCount, positiveCount, negationCount);

            // If no sentiment words found, return neutral
            if (strongNegativeCount + negativeCount + positiveCount == 0)
                return 0.5f;

            // Calculate weighted sentiment score with strong negative words having higher impact
            float totalNegative = (strongNegativeCount * 2.5f) + negativeCount;

            // Adjust positive count based on negations
            // Simple approach: assume some negations flip positive to negative
            float adjustedPositive = Math.Max(0, positiveCount - (negationCount * 0.5f));
            float adjustedNegative = totalNegative + (negationCount * 0.2f); // Some negations amplify negativity

            // Calculate score (0 = negative, 1 = positive)
            float total = adjustedPositive + adjustedNegative;
            if (total == 0)
                return 0.5f;

            float score = adjustedPositive / total;

            // Apply sigmoid function to make the curve steeper in the middle
            // This pushes mild sentiment more clearly toward positive or negative
            score = 1.0f / (1.0f + (float)Math.Exp(-10 * (score - 0.5f)));

            return score;
        }

        private int CountNegations(string text)
        {
            // Count common negation words and phrases
            string[] negationPatterns = {
                "not ", "n't ", "no ", "never ", "neither ", "nor ", "barely ",
                "hardly ", "doesn't ", "isn't ", "wasn't ", "shouldn't ",
                "wouldn't ", "couldn't ", "won't ", "can't ", "don't "
            };

            int count = 0;
            foreach (var pattern in negationPatterns)
            {
                count += Regex.Matches(text, pattern).Count;
            }

            return count;
        }

        private float SimpleTextBasedSentiment(string text)
        {
            return CalculateRuleBasedSentiment(text);
        }

        /// <summary>
        /// Automatically generates tags for content based on its text
        /// </summary>
        /// <param name="content">The content to analyze</param>
        /// <returns>A list of suggested tags</returns>
        public List<string> GenerateTags(Content content)
        {
            try
            {
                _logger.LogInformation("Generating tags for content: {Title}", content.Title);

                // Combine title and body for analysis
                string text = $"{content.Title} {content.Body}";

                // This is a simplified approach using keyword extraction
                var tags = ExtractKeywords(text);

                _logger.LogInformation("Generated {Count} tags for content", tags.Count);
                return tags;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating tags");
                return new List<string>();
            }
        }

        /// <summary>
        /// Generates a summary of the content
        /// </summary>
        /// <param name="content">The content to summarize</param>
        /// <param name="maxLength">Maximum length of the summary</param>
        /// <returns>A summary of the content</returns>
        public string GenerateSummary(Content content, int maxLength = 200)
        {
            try
            {
                _logger.LogInformation("Generating summary for content: {Title}", content.Title);

                // In a real implementation, you would use a trained model for summarization
                // This is a simplified approach using the first few sentences
                string text = content.Body;
                string summary = text.Length <= maxLength
                    ? text
                    : text.Substring(0, maxLength) + "...";

                _logger.LogInformation("Generated summary of length {Length}", summary.Length);
                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating summary");
                return content.Title; // Fallback to title
            }
        }

        /// <summary>
        /// Categorizes content based on its text
        /// </summary>
        /// <param name="content">The content to categorize</param>
        /// <returns>A suggested category</returns>
        public string CategorizeContent(Content content)
        {
            try
            {
                _logger.LogInformation("Categorizing content: {Title}", content.Title);

                // Combine title and body for analysis
                string text = $"{content.Title} {content.Body}".ToLower();

                // In a real implementation, you would use a trained model for categorization
                // This is a simplified approach using keyword matching
                if (text.Contains("tutorial") || text.Contains("guide") || text.Contains("how to"))
                {
                    return "Tutorial";
                }
                else if (text.Contains("news") || text.Contains("announcement") || text.Contains("update"))
                {
                    return "News";
                }
                else if (text.Contains("review") || text.Contains("analysis") || text.Contains("comparison"))
                {
                    return "Review";
                }
                else if (text.Contains("api") || text.Contains("code") || text.Contains("programming"))
                {
                    return "Technical";
                }
                else
                {
                    return "General";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error categorizing content");
                return "Uncategorized"; // Fallback category
            }
        }

        #region Helper Methods

        private List<string> ExtractKeywords(string text)
        {
            // This is a simplified approach to extract keywords
            // In a real implementation, you would use a trained model or NLP techniques

            var keywords = new HashSet<string>();
            var words = text.ToLower()
                .Replace(".", " ")
                .Replace(",", " ")
                .Replace("!", " ")
                .Replace("?", " ")
                .Replace(";", " ")
                .Replace(":", " ")
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // Filter out common stop words and short words
            var stopWords = new HashSet<string> { "the", "and", "a", "an", "in", "on", "at", "to", "for", "with", "by", "is", "are", "was", "were" };

            foreach (var word in words)
            {
                if (word.Length > 3 && !stopWords.Contains(word))
                {
                    keywords.Add(word);
                }
            }

            // Count keyword frequencies
            var keywordFrequency = new Dictionary<string, int>();
            foreach (var word in keywords)
            {
                if (!keywordFrequency.ContainsKey(word))
                    keywordFrequency[word] = 0;
                keywordFrequency[word]++;
            }

            // Return top 5 keywords by frequency
            return keywordFrequency
                .OrderByDescending(kv => kv.Value)
                .Take(5)
                .Select(kv => kv.Key)
                .ToList();
        }

        #endregion
    }

    public class SentimentData
    {
        [LoadColumn(0)]
        public string Text { get; set; }

        [LoadColumn(1), ColumnName("Label")]
        public bool Sentiment { get; set; }

        [LoadColumn(2)]
        public float Weight { get; set; } = 1.0f;
    }

    public class SentimentPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }

        public float Probability { get; set; }
    }





}