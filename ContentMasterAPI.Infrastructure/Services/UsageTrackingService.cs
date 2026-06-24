using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using ContentMasterAPI.Core.Interfaces;
using ContentMasterAPI.Core.Models;

namespace ContentMasterAPI.Infrastructure.Services
{
    /// <summary>
    /// Service for tracking API usage and enforcing rate limits based on subscription tiers
    /// </summary>
    public class UsageTrackingService : IUsageTrackingService
    {
        private readonly ILogger<UsageTrackingService> _logger;
        private readonly Dictionary<string, UserUsage> _usageData;
        
        // Rate limits for different subscription tiers (requests per day)
        private static readonly Dictionary<string, int> _dailyQuotas = new Dictionary<string, int>
        {
            { "Basic", 100 },
            { "Pro", 5000 / 30 }, // 5000 per month ≈ 167 per day
            { "Ultra", 20000 / 30 }, // 20000 per month ≈ 667 per day
            { "Mega", 100000 / 30 } // 100000 per month ≈ 3333 per day
        };

        public UsageTrackingService(ILogger<UsageTrackingService> logger)
        {
            _logger = logger;
            _usageData = new Dictionary<string, UserUsage>();
        }

        /// <summary>
        /// Tracks API usage for a specific API key and checks if the request is within quota limits
        /// </summary>
        /// <param name="apiKey">The RapidAPI key</param>
        /// <param name="tier">The subscription tier</param>
        /// <param name="endpoint">The API endpoint being accessed</param>
        /// <returns>True if the request is within quota limits, false otherwise</returns>
        public async Task<bool> TrackUsageAsync(string apiKey, string tier, string endpoint)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("Attempted to track usage with empty API key");
                return false;
            }

            // Get or create usage data for this API key
            if (!_usageData.TryGetValue(apiKey, out var usage))
            {
                usage = new UserUsage
                {
                    ApiKey = apiKey,
                    SubscriptionTier = tier,
                    LastResetDate = DateTime.UtcNow.Date,
                    DailyRequestCount = 0,
                    EndpointUsage = new Dictionary<string, int>()
                };
                _usageData[apiKey] = usage;
            }

            // Check if we need to reset the daily counter
            if (usage.LastResetDate.Date < DateTime.UtcNow.Date)
            {
                usage.LastResetDate = DateTime.UtcNow.Date;
                usage.DailyRequestCount = 0;
                usage.EndpointUsage.Clear();
            }

            // Increment usage counters
            usage.DailyRequestCount++;
            
            if (!usage.EndpointUsage.ContainsKey(endpoint))
            {
                usage.EndpointUsage[endpoint] = 0;
            }
            usage.EndpointUsage[endpoint]++;

            // Check if the user has exceeded their daily quota
            if (_dailyQuotas.TryGetValue(tier, out var dailyQuota) && usage.DailyRequestCount > dailyQuota)
            {
                _logger.LogWarning("API key {ApiKey} has exceeded daily quota for tier {Tier}", apiKey, tier);
                return false;
            }

            // In a production environment, you would persist this data to a database
            // For demo purposes, we're just keeping it in memory
            _logger.LogInformation("Tracked usage for API key {ApiKey}: {Count} requests today", apiKey, usage.DailyRequestCount);
            
            return true;
        }

        /// <summary>
        /// Gets the current usage statistics for a specific API key
        /// </summary>
        /// <param name="apiKey">The RapidAPI key</param>
        /// <returns>Usage statistics or null if the API key is not found</returns>
        public async Task<UsageStatistics> GetUsageStatisticsAsync(string apiKey)
        {
            if (!_usageData.TryGetValue(apiKey, out var usage))
            {
                return null;
            }

            int dailyQuota = _dailyQuotas.TryGetValue(usage.SubscriptionTier, out var quota) ? quota : 0;

            return new UsageStatistics
            {
                ApiKey = apiKey,
                SubscriptionTier = usage.SubscriptionTier,
                DailyRequestCount = usage.DailyRequestCount,
                DailyQuota = dailyQuota,
                RemainingRequests = Math.Max(0, dailyQuota - usage.DailyRequestCount),
                EndpointUsage = usage.EndpointUsage.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            };
        }
    }

    /// <summary>
    /// Internal class for tracking user usage
    /// </summary>
    internal class UserUsage
    {
        public string ApiKey { get; set; }
        public string SubscriptionTier { get; set; }
        public DateTime LastResetDate { get; set; }
        public int DailyRequestCount { get; set; }
        public Dictionary<string, int> EndpointUsage { get; set; }
    }

}
