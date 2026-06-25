using System;
using System.Collections.Generic;

namespace ContentMasterAPI.Core.Models
{
    public class Invoice
    {
        public string Id { get; set; }
        public string Number { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public List<InvoiceItem> Items { get; set; }
        public string DownloadUrl { get; set; }
    }
}
