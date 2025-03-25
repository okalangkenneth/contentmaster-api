using System;

namespace ContentMasterAPI.API.Models
{
    /// <summary>
    /// Error response model
    /// </summary>
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string ErrorType { get; set; }
        public string RequestId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
