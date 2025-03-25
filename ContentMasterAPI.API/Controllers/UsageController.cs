using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ContentMasterAPI.Infrastructure.Services;
using System.Collections.Generic;
using ContentMasterAPI.API.Models;

namespace ContentMasterAPI.API.Controllers
{
    [ApiController]
    [Route("api/usage")]
    public class UsageController : ControllerBase
    {
        private readonly ILogger<UsageController> _logger;
        private readonly IUsageTrackingService _usageTrackingService;

        public UsageController(
            ILogger<UsageController> logger,
            IUsageTrackingService usageTrackingService)
        {
            _logger = logger;
            _usageTrackingService = usageTrackingService;
        }

        /// <summary>
        /// Gets usage statistics for the current API key
        /// </summary>
        /// <returns>Usage statistics for the current API key</returns>
        [HttpGet]
        [ProducesResponseType(typeof(UsageStatistics), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        public async Task<IActionResult> GetUsageStatistics()
        {
            // Get the API key from the request headers
            if (!Request.Headers.TryGetValue("X-RapidAPI-Key", out var apiKeyValues))
            {
                return Unauthorized(new ErrorResponse
                {
                    StatusCode = 401,
                    Message = "Missing RapidAPI key header",
                    ErrorType = "AuthenticationError",
                    RequestId = HttpContext.TraceIdentifier,
                    Timestamp = DateTime.UtcNow
                });
            }

            var apiKey = apiKeyValues.ToString();
            var statistics = await _usageTrackingService.GetUsageStatisticsAsync(apiKey);

            if (statistics == null)
            {
                return NotFound(new ErrorResponse
                {
                    StatusCode = 404,
                    Message = "No usage statistics found for this API key",
                    ErrorType = "ResourceNotFoundError",
                    RequestId = HttpContext.TraceIdentifier,
                    Timestamp = DateTime.UtcNow
                });
            }

            return Ok(statistics);
        }

        /// <summary>
        /// Gets information about the available subscription tiers
        /// </summary>
        /// <returns>Information about the available subscription tiers</returns>
        [HttpGet("tiers")]
        [ProducesResponseType(typeof(List<SubscriptionTierInfo>), 200)]
        public IActionResult GetSubscriptionTiers()
        {
            var tiers = new List<SubscriptionTierInfo>
            {
                new SubscriptionTierInfo
                {
                    Name = "Basic",
                    Price = 0,
                    DailyQuota = 100,
                    MonthlyQuota = 3000,
                    Features = new List<string>
                    {
                        "Basic content operations",
                        "Limited quota (100 requests per day)",
                        "No AI analytics features"
                    }
                },
                new SubscriptionTierInfo
                {
                    Name = "Pro",
                    Price = 25,
                    DailyQuota = 167,
                    MonthlyQuota = 5000,
                    Features = new List<string>
                    {
                        "Full content management operations",
                        "5,000 requests per month",
                        "Basic AI analytics (sentiment analysis only)",
                        "Standard rate limits",
                        "Overage: $0.005 per additional request"
                    }
                },
                new SubscriptionTierInfo
                {
                    Name = "Ultra",
                    Price = 75,
                    DailyQuota = 667,
                    MonthlyQuota = 20000,
                    Features = new List<string>
                    {
                        "All content management operations",
                        "20,000 requests per month",
                        "Full AI analytics capabilities",
                        "Higher rate limits",
                        "Priority support",
                        "Overage: $0.003 per additional request"
                    }
                },
                new SubscriptionTierInfo
                {
                    Name = "Mega",
                    Price = 150,
                    DailyQuota = 3333,
                    MonthlyQuota = 100000,
                    Features = new List<string>
                    {
                        "All features with no restrictions",
                        "100,000 requests per month",
                        "Highest rate limits",
                        "Premium support",
                        "Custom analytics reports",
                        "Overage: $0.001 per additional request"
                    }
                }
            };

            return Ok(tiers);
        }
    }

    /// <summary>
    /// Information about a subscription tier
    /// </summary>
    public class SubscriptionTierInfo
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int DailyQuota { get; set; }
        public int MonthlyQuota { get; set; }
        public List<string> Features { get; set; }
    }
}
