namespace ContentMasterAPI.Core.Models
{
    public class BillingDetails
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public Address Address { get; set; }
        public string VatId { get; set; }
    }
}
