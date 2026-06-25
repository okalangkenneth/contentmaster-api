namespace ContentMasterAPI.Core.Models
{
    public class PaymentMethodInfo
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Brand { get; set; }
        public string Last4 { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
    }
}
