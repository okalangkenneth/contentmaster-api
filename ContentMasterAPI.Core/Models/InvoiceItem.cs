namespace ContentMasterAPI.Core.Models
{
    public class InvoiceItem
    {
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
    }
}
