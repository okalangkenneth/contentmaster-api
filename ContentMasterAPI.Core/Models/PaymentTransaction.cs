using System;

namespace ContentMasterAPI.Core.Models
{
    public class PaymentTransaction
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
    }
}
