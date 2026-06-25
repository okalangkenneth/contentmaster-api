using System;

namespace ContentMasterAPI.Core.Models
{
    public class UsageLimit
    {
        public string Name { get; set; }
        public int? DailyLimit { get; set; }
        public int? MonthlyLimit { get; set; }
        public int? PerMinuteLimit { get; set; }
        public int? CurrentDailyUsage { get; set; }
        public int? CurrentMonthlyUsage { get; set; }
        public int? CurrentPerMinuteUsage { get; set; }
        public DateTime ResetDate { get; set; }
    }
}
