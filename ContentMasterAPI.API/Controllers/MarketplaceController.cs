using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ContentMasterAPI.Core.Models;
using ContentMasterAPI.Core.Interfaces;
using ContentMasterAPI.Core.Interfaces;

namespace ContentMasterAPI.API.Controllers
{
    [ApiController]
    [Route("api/marketplace")]
    public class MarketplaceController : ControllerBase
    {
        private readonly ILogger<MarketplaceController> _logger;
        private readonly IUsageTrackingService _usageTrackingService;

        public MarketplaceController(
            ILogger<MarketplaceController> logger,
            IUsageTrackingService usageTrackingService)
        {
            _logger = logger;
            _usageTrackingService = usageTrackingService;
        }

        /// <summary>
        /// Gets information about the API for marketplace listings
        /// </summary>
        /// <returns>API information for marketplace listings</returns>
        [HttpGet("info")]
        [ProducesResponseType(typeof(ApiMarketplaceInfo), 200)]
        public IActionResult GetApiInfo()
        {
            var info = new ApiMarketplaceInfo
            {
                Name = "ContentMaster API",
                Version = "1.0.0",
                Description = "A modern content management API with AI-driven capabilities, GraphQL support, and robust security",
                Provider = "ContentMaster",
                LogoUrl = "https://contentmasterapi.com/logo.png",
                DocumentationUrl = "https://contentmasterapi.com/docs",
                Categories = new List<string> { "Content Management", "AI", "Machine Learning", "Analytics" },
                Tags = new List<string> { "content", "ai", "analytics", "graphql", "sentiment-analysis", "tagging" },
                Features = new List<ApiFeature>
                {
                    new ApiFeature
                    {
                        Name = "Content Management",
                        Description = "Full CRUD operations for content items with versioning and metadata",
                        Endpoints = new List<string> { "/api/content", "/api/content/{id}" }
                    },
                    new ApiFeature
                    {
                        Name = "AI-Driven Analytics",
                        Description = "Sentiment analysis, auto-tagging, content categorization, and summarization",
                        Endpoints = new List<string> { "/api/analytics/{id}/sentiment", "/api/analytics/{id}/tags", "/api/analytics/{id}/category", "/api/analytics/{id}/summary" }
                    },
                    new ApiFeature
                    {
                        Name = "GraphQL Support",
                        Description = "Flexible querying capabilities for clients to request exactly what they need",
                        Endpoints = new List<string> { "/api/graphql" }
                    },
                    new ApiFeature
                    {
                        Name = "Security",
                        Description = "JWT authentication and authorization with role-based access control",
                        Endpoints = new List<string> { "/api/auth/login", "/api/auth/validate" }
                    }
                },
                PricingTiers = new List<PricingTier>
                {
                    new PricingTier
                    {
                        Name = "Basic",
                        Price = 0,
                        Interval = "month",
                        Currency = "USD",
                        Description = "Free tier with limited access",
                        Features = new List<string>
                        {
                            "Basic content operations",
                            "Limited quota (100 requests per day)",
                            "No AI analytics features"
                        },
                        Quota = 100,
                        QuotaInterval = "day"
                    },
                    new PricingTier
                    {
                        Name = "Pro",
                        Price = 25,
                        Interval = "month",
                        Currency = "USD",
                        Description = "Professional tier for individual developers",
                        Features = new List<string>
                        {
                            "Full content management operations",
                            "5,000 requests per month",
                            "Basic AI analytics (sentiment analysis only)",
                            "Standard rate limits"
                        },
                        Quota = 5000,
                        QuotaInterval = "month",
                        OveragePrice = 0.005m,
                        OverageInterval = "request"
                    },
                    new PricingTier
                    {
                        Name = "Ultra",
                        Price = 75,
                        Interval = "month",
                        Currency = "USD",
                        Description = "Ultra tier for small businesses",
                        Features = new List<string>
                        {
                            "All content management operations",
                            "20,000 requests per month",
                            "Full AI analytics capabilities",
                            "Higher rate limits",
                            "Priority support"
                        },
                        Quota = 20000,
                        QuotaInterval = "month",
                        OveragePrice = 0.003m,
                        OverageInterval = "request"
                    },
                    new PricingTier
                    {
                        Name = "Mega",
                        Price = 150,
                        Interval = "month",
                        Currency = "USD",
                        Description = "Enterprise tier for large organizations",
                        Features = new List<string>
                        {
                            "All features with no restrictions",
                            "100,000 requests per month",
                            "Highest rate limits",
                            "Premium support",
                            "Custom analytics reports"
                        },
                        Quota = 100000,
                        QuotaInterval = "month",
                        OveragePrice = 0.001m,
                        OverageInterval = "request"
                    }
                }
            };

            return Ok(info);
        }

        /// <summary>
        /// Gets health status of the API for marketplace monitoring
        /// </summary>
        /// <returns>Health status information</returns>
        [HttpGet("health")]
        [ProducesResponseType(typeof(ApiHealthStatus), 200)]
        public IActionResult GetHealthStatus()
        {
            var status = new ApiHealthStatus
            {
                Status = "healthy",
                Version = "1.0.0",
                Timestamp = DateTime.UtcNow,
                Uptime = TimeSpan.FromSeconds(Environment.TickCount / 1000).ToString(),
                Services = new Dictionary<string, string>
                {
                    { "ContentRepository", "operational" },
                    { "AnalyticsService", "operational" },
                    { "AuthenticationService", "operational" }
                }
            };

            return Ok(status);
        }
    }

    /// <summary>
    /// API information for marketplace listings
    /// </summary>
    public class ApiMarketplaceInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string Provider { get; set; }
        public string LogoUrl { get; set; }
        public string DocumentationUrl { get; set; }
        public List<string> Categories { get; set; }
        public List<string> Tags { get; set; }
        public List<ApiFeature> Features { get; set; }
        public List<PricingTier> PricingTiers { get; set; }
    }

    /// <summary>
    /// API feature information
    /// </summary>
    public class ApiFeature
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Endpoints { get; set; }
    }

    /// <summary>
    /// Pricing tier information
    /// </summary>
    public class PricingTier
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Interval { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public List<string> Features { get; set; }
        public int Quota { get; set; }
        public string QuotaInterval { get; set; }
        public decimal? OveragePrice { get; set; }
        public string OverageInterval { get; set; }
    }

    /// <summary>
    /// API health status information
    /// </summary>
    public class ApiHealthStatus
    {
        public string Status { get; set; }
        public string Version { get; set; }
        public DateTime Timestamp { get; set; }
        public string Uptime { get; set; }
        public Dictionary<string, string> Services { get; set; }
    }
}
