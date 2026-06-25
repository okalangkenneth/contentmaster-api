using System.Collections.Generic;

namespace ContentMasterAPI.Core.Models
{
    public class UsageLimits
    {
        public string PlanId { get; set; }
        public string PlanName { get; set; }
        public Dictionary<string, UsageLimit> Limits { get; set; }
        public Dictionary<string, bool> FeatureAccess { get; set; }
    }
}
