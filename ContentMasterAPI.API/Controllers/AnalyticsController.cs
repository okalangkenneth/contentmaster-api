using ContentMasterAPI.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ContentMasterAPI.Core.Models;

namespace ContentMasterAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IContentRepository _contentRepository;
        private readonly IContentAnalysisService _analysisService;
        private readonly ILogger<AnalyticsController> _logger;

        public AnalyticsController(
            IContentRepository contentRepository,
            IContentAnalysisService analysisService,
            ILogger<AnalyticsController> logger)
        {
            _contentRepository = contentRepository ?? throw new ArgumentNullException(nameof(contentRepository));
            _analysisService = analysisService ?? throw new ArgumentNullException(nameof(analysisService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Analyzes the sentiment of a content item
        /// </summary>
        /// <param name="id">The ID of the content to analyze</param>
        /// <returns>The sentiment score between 0 (negative) and 1 (positive)</returns>
        [HttpGet("{id}/sentiment")]
        public async Task<ActionResult<object>> AnalyzeSentiment(Guid id)
        {
            try
            {
                var content = await _contentRepository.GetByIdAsync(id);
                if (content == null)
                {
                    return NotFound($"Content with ID {id} not found");
                }

                var sentimentScore = _analysisService.AnalyzeSentiment(content.Body);

                return Ok(new
                {
                    ContentId = id,
                    Title = content.Title,
                    SentimentScore = sentimentScore,
                    SentimentLabel = GetSentimentLabel(sentimentScore)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing sentiment for content with ID {Id}", id);
                return StatusCode(500, "Error analyzing sentiment");
            }
        }

        /// <summary>
        /// Generates tags for a content item
        /// </summary>
        /// <param name="id">The ID of the content to analyze</param>
        /// <returns>A list of suggested tags</returns>
        [HttpGet("{id}/tags")]
        public async Task<ActionResult<object>> GenerateTags(Guid id)
        {
            try
            {
                var content = await _contentRepository.GetByIdAsync(id);
                if (content == null)
                {
                    return NotFound($"Content with ID {id} not found");
                }

                var tags = _analysisService.GenerateTags(content);

                return Ok(new
                {
                    ContentId = id,
                    Title = content.Title,
                    SuggestedTags = tags
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating tags for content with ID {Id}", id);
                return StatusCode(500, "Error generating tags");
            }
        }

        /// <summary>
        /// Generates a summary of a content item
        /// </summary>
        /// <param name="id">The ID of the content to summarize</param>
        /// <param name="maxLength">Maximum length of the summary (default: 200)</param>
        /// <returns>A summary of the content</returns>
        [HttpGet("{id}/summary")]
        public async Task<ActionResult<object>> GenerateSummary(Guid id, [FromQuery] int maxLength = 200)
        {
            try
            {
                var content = await _contentRepository.GetByIdAsync(id);
                if (content == null)
                {
                    return NotFound($"Content with ID {id} not found");
                }

                var summary = _analysisService.GenerateSummary(content, maxLength);

                return Ok(new
                {
                    ContentId = id,
                    Title = content.Title,
                    Summary = summary
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating summary for content with ID {Id}", id);
                return StatusCode(500, "Error generating summary");
            }
        }

        /// <summary>
        /// Categorizes a content item
        /// </summary>
        /// <param name="id">The ID of the content to categorize</param>
        /// <returns>A suggested category</returns>
        [HttpGet("{id}/category")]
        public async Task<ActionResult<object>> CategorizeContent(Guid id)
        {
            try
            {
                var content = await _contentRepository.GetByIdAsync(id);
                if (content == null)
                {
                    return NotFound($"Content with ID {id} not found");
                }

                var category = _analysisService.CategorizeContent(content);

                return Ok(new
                {
                    ContentId = id,
                    Title = content.Title,
                    SuggestedCategory = category
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error categorizing content with ID {Id}", id);
                return StatusCode(500, "Error categorizing content");
            }
        }

        private string GetSentimentLabel(float score)
        {
            if (score < 0.3) return "Negative";
            if (score < 0.45) return "Somewhat Negative";
            if (score < 0.55) return "Neutral";
            if (score < 0.8) return "Somewhat Positive";
            return "Positive";
        }
    }
}