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
    [Route("api/marketplace/analytics")]
    public class MarketplaceAnalyticsController : ControllerBase
    {
        private readonly ILogger<MarketplaceAnalyticsController> _logger;

        public MarketplaceAnalyticsController(ILogger<MarketplaceAnalyticsController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets API usage analytics for marketplace consumers
        /// </summary>
        /// <returns>API usage analytics</returns>
        [HttpGet("usage")]
        [ProducesResponseType(typeof(ApiUsageAnalytics), 200)]
        public IActionResult GetUsageAnalytics()
        {
            // In a production environment, this would be populated with real data
            // For demo purposes, we're returning sample data
            var analytics = new ApiUsageAnalytics
            {
                Period = "Last 30 days",
                StartDate = DateTime.UtcNow.AddDays(-30),
                EndDate = DateTime.UtcNow,
                TotalRequests = 12450,
                UniqueUsers = 42,
                DailyUsage = GenerateSampleDailyUsage(30),
                EndpointUsage = new Dictionary<string, int>
                {
                    { "/api/content", 5230 },
                    { "/api/content/{id}", 3120 },
                    { "/api/analytics/{id}/sentiment", 1840 },
                    { "/api/analytics/{id}/tags", 1260 },
                    { "/api/analytics/{id}/category", 580 },
                    { "/api/analytics/{id}/summary", 420 }
                },
                SubscriptionDistribution = new Dictionary<string, int>
                {
                    { "Basic", 25 },
                    { "Pro", 12 },
                    { "Ultra", 4 },
                    { "Mega", 1 }
                }
            };

            return Ok(analytics);
        }

        /// <summary>
        /// Gets performance analytics for marketplace consumers
        /// </summary>
        /// <returns>API performance analytics</returns>
        [HttpGet("performance")]
        [ProducesResponseType(typeof(ApiPerformanceAnalytics), 200)]
        public IActionResult GetPerformanceAnalytics()
        {
            // In a production environment, this would be populated with real data
            // For demo purposes, we're returning sample data
            var analytics = new ApiPerformanceAnalytics
            {
                Period = "Last 30 days",
                StartDate = DateTime.UtcNow.AddDays(-30),
                EndDate = DateTime.UtcNow,
                AverageResponseTime = 125, // milliseconds
                P95ResponseTime = 250, // milliseconds
                P99ResponseTime = 450, // milliseconds
                ErrorRate = 0.5, // percentage
                Availability = 99.95, // percentage
                EndpointPerformance = new Dictionary<string, EndpointPerformance>
                {
                    { 
                        "/api/content", 
                        new EndpointPerformance 
                        { 
                            AverageResponseTime = 85,
                            P95ResponseTime = 180,
                            ErrorRate = 0.3,
                            RequestCount = 5230
                        } 
                    },
                    { 
                        "/api/content/{id}", 
                        new EndpointPerformance 
                        { 
                            AverageResponseTime = 65,
                            P95ResponseTime = 150,
                            ErrorRate = 0.2,
                            RequestCount = 3120
                        } 
                    },
                    { 
                        "/api/analytics/{id}/sentiment", 
                        new EndpointPerformance 
                        { 
                            AverageResponseTime = 220,
                            P95ResponseTime = 380,
                            ErrorRate = 0.8,
                            RequestCount = 1840
                        } 
                    },
                    { 
                        "/api/analytics/{id}/tags", 
                        new EndpointPerformance 
                        { 
                            AverageResponseTime = 250,
                            P95ResponseTime = 420,
                            ErrorRate = 1.2,
                            RequestCount = 1260
                        } 
                    }
                }
            };

            return Ok(analytics);
        }

        /// <summary>
        /// Gets revenue analytics for marketplace providers
        /// </summary>
        /// <returns>API revenue analytics</returns>
        [HttpGet("revenue")]
        [ProducesResponseType(typeof(ApiRevenueAnalytics), 200)]
        public IActionResult GetRevenueAnalytics()
        {
            // In a production environment, this would be populated with real data
            // For demo purposes, we're returning sample data
            var analytics = new ApiRevenueAnalytics
            {
                Period = "Last 30 days",
                StartDate = DateTime.UtcNow.AddDays(-30),
                EndDate = DateTime.UtcNow,
                TotalRevenue = 1245.50m,
                MonthlyRecurringRevenue = 1125.00m,
                OverageRevenue = 120.50m,
                RevenueByTier = new Dictionary<string, decimal>
                {
                    { "Basic", 0.00m },
                    { "Pro", 300.00m },
                    { "Ultra", 300.00m },
                    { "Mega", 525.00m }
                },
                MonthlyRevenue = GenerateSampleMonthlyRevenue(6),
                ProjectedAnnualRevenue = 14946.00m,
                CustomerLifetimeValue = 1245.50m
            };

            return Ok(analytics);
        }

        private List<DailyUsage> GenerateSampleDailyUsage(int days)
        {
            var result = new List<DailyUsage>();
            var random = new Random();
            var baseRequests = 350;

            for (int i = 0; i < days; i++)
            {
                var date = DateTime.UtcNow.AddDays(-days + i + 1).Date;
                var isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
                var multiplier = isWeekend ? 0.7 : 1.0;
                var requests = (int)(baseRequests * multiplier * (0.8 + random.NextDouble() * 0.4));

                result.Add(new DailyUsage
                {
                    Date = date,
                    Requests = requests,
                    UniqueUsers = random.Next(30, 45)
                });
            }

            return result;
        }

        private List<MonthlyRevenue> GenerateSampleMonthlyRevenue(int months)
        {
            var result = new List<MonthlyRevenue>();
            var baseRevenue = 1000.00m;
            var growthRate = 0.05m; // 5% monthly growth

            for (int i = 0; i < months; i++)
            {
                var date = DateTime.UtcNow.AddMonths(-months + i + 1).Date;
                var revenue = baseRevenue * (1 + growthRate * i);

                result.Add(new MonthlyRevenue
                {
                    Month = date.ToString("MMMM yyyy"),
                    Revenue = Math.Round(revenue, 2),
                    SubscriberCount = 30 + i * 2
                });
            }

            return result;
        }
    }

    /// <summary>
    /// API usage analytics
    /// </summary>
    public class ApiUsageAnalytics
    {
        public string Period { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalRequests { get; set; }
        public int UniqueUsers { get; set; }
        public List<DailyUsage> DailyUsage { get; set; }
        public Dictionary<string, int> EndpointUsage { get; set; }
        public Dictionary<string, int> SubscriptionDistribution { get; set; }
    }

    /// <summary>
    /// Daily usage data
    /// </summary>
    public class DailyUsage
    {
        public DateTime Date { get; set; }
        public int Requests { get; set; }
        public int UniqueUsers { get; set; }
    }

    /// <summary>
    /// API performance analytics
    /// </summary>
    public class ApiPerformanceAnalytics
    {
        public string Period { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double AverageResponseTime { get; set; }
        public double P95ResponseTime { get; set; }
        public double P99ResponseTime { get; set; }
        public double ErrorRate { get; set; }
        public double Availability { get; set; }
        public Dictionary<string, EndpointPerformance> EndpointPerformance { get; set; }
    }

    /// <summary>
    /// Endpoint performance data
    /// </summary>
    public class EndpointPerformance
    {
        public double AverageResponseTime { get; set; }
        public double P95ResponseTime { get; set; }
        public double ErrorRate { get; set; }
        public int RequestCount { get; set; }
    }

    /// <summary>
    /// API revenue analytics
    /// </summary>
    public class ApiRevenueAnalytics
    {
        public string Period { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRecurringRevenue { get; set; }
        public decimal OverageRevenue { get; set; }
        public Dictionary<string, decimal> RevenueByTier { get; set; }
        public List<MonthlyRevenue> MonthlyRevenue { get; set; }
        public decimal ProjectedAnnualRevenue { get; set; }
        public decimal CustomerLifetimeValue { get; set; }
    }

    /// <summary>
    /// Monthly revenue data
    /// </summary>
    public class MonthlyRevenue
    {
        public string Month { get; set; }
        public decimal Revenue { get; set; }
        public int SubscriberCount { get; set; }
    }
}
