using System.Collections.Generic;

namespace ContentMasterAPI.Core.Models
{
    /// <summary>
    /// Usage statistics for a specific RapidAPI subscriber key.
    /// </summary>
    public class UsageStatistics
    {
        public string ApiKey { get; set; }
        public string SubscriptionTier { get; set; }
        public int DailyRequestCount { get; set; }
        public int DailyQuota { get; set; }
        public int RemainingRequests { get; set; }
        public Dictionary<string, int> EndpointUsage { get; set; }
    }
}
