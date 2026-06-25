using System;

namespace ContentMasterAPI.Core.Models
{
    public class Subscription
    {
        public string Id { get; set; }
        public string PlanId { get; set; }
        public string PlanName { get; set; }
        public string Status { get; set; }
        public DateTime CurrentPeriodStart { get; set; }
        public DateTime CurrentPeriodEnd { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool CancelAtPeriodEnd { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public string Interval { get; set; }
        public BillingDetails BillingDetails { get; set; }
        public PaymentMethodInfo PaymentMethod { get; set; }
    }
}
