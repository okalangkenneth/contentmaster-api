using System;

namespace ContentMasterAPI.Core.Models
{
    public class SubscriptionEvent
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string PlanId { get; set; }
        public string PlanName { get; set; }
        public decimal? Amount { get; set; }
        public string Currency { get; set; }
    }
}
