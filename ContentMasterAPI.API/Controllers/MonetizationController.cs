using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ContentMasterAPI.Core.Models;
using ContentMasterAPI.Core.Interfaces;

namespace ContentMasterAPI.API.Controllers
{
    [ApiController]
    [Route("api/marketplace/monetization")]
    public class MonetizationController : ControllerBase
    {
        private readonly ILogger<MonetizationController> _logger;

        public MonetizationController(ILogger<MonetizationController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets subscription plans for the API
        /// </summary>
        /// <returns>Available subscription plans</returns>
        [HttpGet("plans")]
        [ProducesResponseType(typeof(List<SubscriptionPlan>), 200)]
        public IActionResult GetSubscriptionPlans()
        {
            var plans = new List<SubscriptionPlan>
            {
                new SubscriptionPlan
                {
                    Id = "basic",
                    Name = "Basic",
                    Description = "Free tier with limited access",
                    Price = 0,
                    Currency = "USD",
                    Interval = "month",
                    Features = new List<string>
                    {
                        "Basic content operations",
                        "Limited quota (100 requests per day)",
                        "No AI analytics features"
                    },
                    Limits = new Dictionary<string, object>
                    {
                        { "dailyRequests", 100 },
                        { "monthlyRequests", 3000 },
                        { "rateLimit", 10 } // requests per minute
                    },
                    IsPopular = false
                },
                new SubscriptionPlan
                {
                    Id = "pro",
                    Name = "Pro",
                    Description = "Professional tier for individual developers",
                    Price = 25,
                    Currency = "USD",
                    Interval = "month",
                    Features = new List<string>
                    {
                        "Full content management operations",
                        "5,000 requests per month",
                        "Basic AI analytics (sentiment analysis only)",
                        "Standard rate limits",
                        "Overage: $0.005 per additional request"
                    },
                    Limits = new Dictionary<string, object>
                    {
                        { "dailyRequests", 167 },
                        { "monthlyRequests", 5000 },
                        { "rateLimit", 30 } // requests per minute
                    },
                    IsPopular = true
                },
                new SubscriptionPlan
                {
                    Id = "ultra",
                    Name = "Ultra",
                    Description = "Ultra tier for small businesses",
                    Price = 75,
                    Currency = "USD",
                    Interval = "month",
                    Features = new List<string>
                    {
                        "All content management operations",
                        "20,000 requests per month",
                        "Full AI analytics capabilities",
                        "Higher rate limits",
                        "Priority support",
                        "Overage: $0.003 per additional request"
                    },
                    Limits = new Dictionary<string, object>
                    {
                        { "dailyRequests", 667 },
                        { "monthlyRequests", 20000 },
                        { "rateLimit", 60 } // requests per minute
                    },
                    IsPopular = false
                },
                new SubscriptionPlan
                {
                    Id = "mega",
                    Name = "Mega",
                    Description = "Enterprise tier for large organizations",
                    Price = 150,
                    Currency = "USD",
                    Interval = "month",
                    Features = new List<string>
                    {
                        "All features with no restrictions",
                        "100,000 requests per month",
                        "Highest rate limits",
                        "Premium support",
                        "Custom analytics reports",
                        "Overage: $0.001 per additional request"
                    },
                    Limits = new Dictionary<string, object>
                    {
                        { "dailyRequests", 3333 },
                        { "monthlyRequests", 100000 },
                        { "rateLimit", 120 } // requests per minute
                    },
                    IsPopular = false
                }
            };

            return Ok(plans);
        }

        /// <summary>
        /// Gets billing information for the current API key
        /// </summary>
        /// <returns>Billing information</returns>
        [HttpGet("billing")]
        [ProducesResponseType(typeof(BillingInfo), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        public IActionResult GetBillingInfo()
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
            
            // In a production environment, this would be populated with real data
            // For demo purposes, we're returning sample data
            var billingInfo = new BillingInfo
            {
                SubscriptionPlan = "Pro",
                Status = "active",
                CurrentPeriodStart = DateTime.UtcNow.AddDays(-15),
                CurrentPeriodEnd = DateTime.UtcNow.AddDays(15),
                MonthlyQuota = 5000,
                UsedQuota = 2345,
                RemainingQuota = 2655,
                OverageUsage = 0,
                OverageRate = 0.005m,
                CurrentCharges = 25.00m,
                OverageCharges = 0.00m,
                TotalCharges = 25.00m,
                BillingCycle = "monthly",
                NextBillingDate = DateTime.UtcNow.AddDays(15),
                PaymentMethod = "credit_card",
                PaymentMethodLast4 = "1234"
            };

            return Ok(billingInfo);
        }

        /// <summary>
        /// Gets usage reports for the current API key
        /// </summary>
        /// <returns>Usage reports</returns>
        [HttpGet("usage-reports")]
        [ProducesResponseType(typeof(List<UsageReport>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        public IActionResult GetUsageReports()
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
            
            // In a production environment, this would be populated with real data
            // For demo purposes, we're returning sample data
            var reports = new List<UsageReport>
            {
                new UsageReport
                {
                    Period = "March 2025",
                    StartDate = new DateTime(2025, 3, 1),
                    EndDate = new DateTime(2025, 3, 31),
                    SubscriptionPlan = "Pro",
                    TotalRequests = 4850,
                    QuotaLimit = 5000,
                    OverageRequests = 0,
                    OverageCharges = 0.00m,
                    BaseCharges = 25.00m,
                    TotalCharges = 25.00m,
                    EndpointUsage = new Dictionary<string, int>
                    {
                        { "/api/content", 2230 },
                        { "/api/content/{id}", 1620 },
                        { "/api/analytics/{id}/sentiment", 1000 }
                    }
                },
                new UsageReport
                {
                    Period = "February 2025",
                    StartDate = new DateTime(2025, 2, 1),
                    EndDate = new DateTime(2025, 2, 28),
                    SubscriptionPlan = "Pro",
                    TotalRequests = 5250,
                    QuotaLimit = 5000,
                    OverageRequests = 250,
                    OverageCharges = 1.25m,
                    BaseCharges = 25.00m,
                    TotalCharges = 26.25m,
                    EndpointUsage = new Dictionary<string, int>
                    {
                        { "/api/content", 2430 },
                        { "/api/content/{id}", 1820 },
                        { "/api/analytics/{id}/sentiment", 1000 }
                    }
                },
                new UsageReport
                {
                    Period = "January 2025",
                    StartDate = new DateTime(2025, 1, 1),
                    EndDate = new DateTime(2025, 1, 31),
                    SubscriptionPlan = "Pro",
                    TotalRequests = 4750,
                    QuotaLimit = 5000,
                    OverageRequests = 0,
                    OverageCharges = 0.00m,
                    BaseCharges = 25.00m,
                    TotalCharges = 25.00m,
                    EndpointUsage = new Dictionary<string, int>
                    {
                        { "/api/content", 2130 },
                        { "/api/content/{id}", 1620 },
                        { "/api/analytics/{id}/sentiment", 1000 }
                    }
                }
            };

            return Ok(reports);
        }
    }

    /// <summary>
    /// Subscription plan information
    /// </summary>
    public class SubscriptionPlan
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public string Interval { get; set; }
        public List<string> Features { get; set; }
        public Dictionary<string, object> Limits { get; set; }
        public bool IsPopular { get; set; }
    }

    /// <summary>
    /// Billing information
    /// </summary>
    public class BillingInfo
    {
        public string SubscriptionPlan { get; set; }
        public string Status { get; set; }
        public DateTime CurrentPeriodStart { get; set; }
        public DateTime CurrentPeriodEnd { get; set; }
        public int MonthlyQuota { get; set; }
        public int UsedQuota { get; set; }
        public int RemainingQuota { get; set; }
        public int OverageUsage { get; set; }
        public decimal OverageRate { get; set; }
        public decimal CurrentCharges { get; set; }
        public decimal OverageCharges { get; set; }
        public decimal TotalCharges { get; set; }
        public string BillingCycle { get; set; }
        public DateTime NextBillingDate { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentMethodLast4 { get; set; }
    }

    /// <summary>
    /// Usage report
    /// </summary>
    public class UsageReport
    {
        public string Period { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string SubscriptionPlan { get; set; }
        public int TotalRequests { get; set; }
        public int QuotaLimit { get; set; }
        public int OverageRequests { get; set; }
        public decimal OverageCharges { get; set; }
        public decimal BaseCharges { get; set; }
        public decimal TotalCharges { get; set; }
        public Dictionary<string, int> EndpointUsage { get; set; }
    }

    /// <summary>
    /// Error response model
    /// </summary>
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string ErrorType { get; set; }
        public string RequestId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
