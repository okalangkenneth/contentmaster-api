using System;

namespace ContentMasterAPI.Core.Models
{
    public class SentimentResult
    {
        public Guid ContentId { get; set; }
        public string Title { get; set; }
        public double SentimentScore { get; set; }
        public string SentimentLabel { get; set; }
    }
}
