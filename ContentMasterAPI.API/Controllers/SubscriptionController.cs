using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ContentMasterAPI.Core.Models;
using ContentMasterAPI.Core.Interfaces;
using ContentMasterAPI.API.Models;

namespace ContentMasterAPI.API.Controllers
{
    [ApiController]
    [Route("api/marketplace/subscriptions")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ILogger<SubscriptionController> _logger;

        public SubscriptionController(ILogger<SubscriptionController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets subscription information for the current API key
        /// </summary>
        /// <returns>Subscription information</returns>
        [HttpGet("current")]
        [ProducesResponseType(typeof(Subscription), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        public IActionResult GetCurrentSubscription()
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
            var subscription = new Subscription
            {
                Id = "sub_1",
                PlanId = "pro",
                PlanName = "Pro",
                Status = "active",
                CurrentPeriodStart = DateTime.UtcNow.AddDays(-15),
                CurrentPeriodEnd = DateTime.UtcNow.AddDays(15),
                CreatedAt = DateTime.UtcNow.AddDays(-45),
                CancelAtPeriodEnd = false,
                Price = 25.00m,
                Currency = "USD",
                Interval = "month",
                BillingDetails = new BillingDetails
                {
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    Company = "Example Corp",
                    Address = new Address
                    {
                        Line1 = "123 Main St",
                        Line2 = "Suite 100",
                        City = "San Francisco",
                        State = "CA",
                        PostalCode = "94105",
                        Country = "US"
                    },
                    VatId = "US123456789"
                },
                PaymentMethod = new PaymentMethodInfo
                {
                    Id = "pm_1",
                    Type = "credit_card",
                    Brand = "visa",
                    Last4 = "4242",
                    ExpiryMonth = 12,
                    ExpiryYear = 2026
                }
            };

            return Ok(subscription);
        }

        /// <summary>
        /// Gets subscription history for the current API key
        /// </summary>
        /// <returns>Subscription history</returns>
        [HttpGet("history")]
        [ProducesResponseType(typeof(List<SubscriptionEvent>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        public IActionResult GetSubscriptionHistory()
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
            var events = new List<SubscriptionEvent>
            {
                new SubscriptionEvent
                {
                    Id = "evt_1",
                    Type = "subscription_created",
                    Date = DateTime.UtcNow.AddDays(-45),
                    Description = "Subscription created on Pro plan",
                    PlanId = "pro",
                    PlanName = "Pro"
                },
                new SubscriptionEvent
                {
                    Id = "evt_2",
                    Type = "subscription_renewed",
                    Date = DateTime.UtcNow.AddDays(-15),
                    Description = "Subscription renewed on Pro plan",
                    PlanId = "pro",
                    PlanName = "Pro"
                },
                new SubscriptionEvent
                {
                    Id = "evt_3",
                    Type = "payment_succeeded",
                    Date = DateTime.UtcNow.AddDays(-15),
                    Description = "Payment succeeded for Pro plan",
                    PlanId = "pro",
                    PlanName = "Pro",
                    Amount = 25.00m,
                    Currency = "USD"
                }
            };

            return Ok(events);
        }

        /// <summary>
        /// Gets usage limits for the current subscription
        /// </summary>
        /// <returns>Usage limits</returns>
        [HttpGet("limits")]
        [ProducesResponseType(typeof(UsageLimits), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        public IActionResult GetUsageLimits()
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
            var limits = new UsageLimits
            {
                PlanId = "pro",
                PlanName = "Pro",
                Limits = new Dictionary<string, UsageLimit>
                {
                    {
                        "requests", new UsageLimit
                        {
                            Name = "API Requests",
                            DailyLimit = 167,
                            MonthlyLimit = 5000,
                            CurrentDailyUsage = 45,
                            CurrentMonthlyUsage = 2345,
                            ResetDate = DateTime.UtcNow.AddDays(15)
                        }
                    },
                    {
                        "rate", new UsageLimit
                        {
                            Name = "Rate Limit",
                            PerMinuteLimit = 30,
                            CurrentPerMinuteUsage = 0,
                            ResetDate = DateTime.UtcNow.AddMinutes(1)
                        }
                    }
                },
                FeatureAccess = new Dictionary<string, bool>
                {
                    { "content_management", true },
                    { "sentiment_analysis", true },
                    { "auto_tagging", false },
                    { "content_categorization", false },
                    { "summarization", false },
                    { "priority_support", false }
                }
            };

            return Ok(limits);
        }
    }
}
