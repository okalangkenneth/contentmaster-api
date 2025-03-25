using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ContentMasterAPI.Core.Models;
using ContentMasterAPI.Core.Interfaces;
using ContentMasterAPI.API.Models;

namespace ContentMasterAPI.API.Controllers
{
    [ApiController]
    [Route("api/marketplace/payments")]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(ILogger<PaymentController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets payment methods for the current API key
        /// </summary>
        /// <returns>Payment methods</returns>
        [HttpGet("methods")]
        [ProducesResponseType(typeof(List<PaymentMethod>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        public IActionResult GetPaymentMethods()
        {
            // Get the API key from the request headers
            if (!Request.Headers.TryGetValue("X-RapidAPI-Key", out var apiKeyValues))
            {
                return Unauthorized(new ErrorResponse
                {
                    StatusCode = 401,
                    Message = "Missing RapidAPI key header",
                    ErrorType = "AuthenticationError",
                    RequestId = HttpContext.TraceIdentifier,
                    Timestamp = DateTime.UtcNow
                });
            }

            var apiKey = apiKeyValues.ToString();
            
            // In a production environment, this would be populated with real data
            // For demo purposes, we're returning sample data
            var paymentMethods = new List<PaymentMethod>
            {
                new PaymentMethod
                {
                    Id = "pm_1",
                    Type = "credit_card",
                    Brand = "visa",
                    Last4 = "4242",
                    ExpiryMonth = 12,
                    ExpiryYear = 2026,
                    IsDefault = true
                },
                new PaymentMethod
                {
                    Id = "pm_2",
                    Type = "credit_card",
                    Brand = "mastercard",
                    Last4 = "5555",
                    ExpiryMonth = 10,
                    ExpiryYear = 2025,
                    IsDefault = false
                }
            };

            return Ok(paymentMethods);
        }

        /// <summary>
        /// Gets payment history for the current API key
        /// </summary>
        /// <returns>Payment history</returns>
        [HttpGet("history")]
        [ProducesResponseType(typeof(List<PaymentTransaction>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        public IActionResult GetPaymentHistory()
        {
            // Get the API key from the request headers
            if (!Request.Headers.TryGetValue("X-RapidAPI-Key", out var apiKeyValues))
            {
                return Unauthorized(new ErrorResponse
                {
                    StatusCode = 401,
                    Message = "Missing RapidAPI key header",
                    ErrorType = "AuthenticationError",
                    RequestId = HttpContext.TraceIdentifier,
                    Timestamp = DateTime.UtcNow
                });
            }

            var apiKey = apiKeyValues.ToString();
            
            // In a production environment, this would be populated with real data
            // For demo purposes, we're returning sample data
            var transactions = new List<PaymentTransaction>
            {
                new PaymentTransaction
                {
                    Id = "txn_1",
                    Date = DateTime.UtcNow.AddDays(-1),
                    Amount = 25.00m,
                    Currency = "USD",
                    Description = "Pro Plan Subscription - April 2025",
                    Status = "succeeded",
                    PaymentMethod = "visa ****4242"
                },
                new PaymentTransaction
                {
                    Id = "txn_2",
                    Date = DateTime.UtcNow.AddDays(-31),
                    Amount = 26.25m,
                    Currency = "USD",
                    Description = "Pro Plan Subscription - March 2025 (includes $1.25 overage)",
                    Status = "succeeded",
                    PaymentMethod = "visa ****4242"
                },
                new PaymentTransaction
                {
                    Id = "txn_3",
                    Date = DateTime.UtcNow.AddDays(-61),
                    Amount = 25.00m,
                    Currency = "USD",
                    Description = "Pro Plan Subscription - February 2025",
                    Status = "succeeded",
                    PaymentMethod = "visa ****4242"
                }
            };

            return Ok(transactions);
        }

        /// <summary>
        /// Gets invoices for the current API key
        /// </summary>
        /// <returns>Invoices</returns>
        [HttpGet("invoices")]
        [ProducesResponseType(typeof(List<Invoice>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        public IActionResult GetInvoices()
        {
            // Get the API key from the request headers
            if (!Request.Headers.TryGetValue("X-RapidAPI-Key", out var apiKeyValues))
            {
                return Unauthorized(new ErrorResponse
                {
                    StatusCode = 401,
                    Message = "Missing RapidAPI key header",
                    ErrorType = "AuthenticationError",
                    RequestId = HttpContext.TraceIdentifier,
                    Timestamp = DateTime.UtcNow
                });
            }

            var apiKey = apiKeyValues.ToString();
            
            // In a production environment, this would be populated with real data
            // For demo purposes, we're returning sample data
            var invoices = new List<Invoice>
            {
                new Invoice
                {
                    Id = "inv_1",
                    Number = "INV-2025-001",
                    Date = DateTime.UtcNow.AddDays(-1),
                    DueDate = DateTime.UtcNow.AddDays(-1),
                    Amount = 25.00m,
                    Currency = "USD",
                    Status = "paid",
                    Description = "Pro Plan Subscription - April 2025",
                    Items = new List<InvoiceItem>
                    {
                        new InvoiceItem
                        {
                            Description = "Pro Plan Monthly Subscription",
                            Quantity = 1,
                            UnitPrice = 25.00m,
                            Amount = 25.00m
                        }
                    },
                    DownloadUrl = "https://contentmaster.p.rapidapi.com/api/marketplace/payments/invoices/inv_1/pdf"
                },
                new Invoice
                {
                    Id = "inv_2",
                    Number = "INV-2025-002",
                    Date = DateTime.UtcNow.AddDays(-31),
                    DueDate = DateTime.UtcNow.AddDays(-31),
                    Amount = 26.25m,
                    Currency = "USD",
                    Status = "paid",
                    Description = "Pro Plan Subscription - March 2025",
                    Items = new List<InvoiceItem>
                    {
                        new InvoiceItem
                        {
                            Description = "Pro Plan Monthly Subscription",
                            Quantity = 1,
                            UnitPrice = 25.00m,
                            Amount = 25.00m
                        },
                        new InvoiceItem
                        {
                            Description = "Overage Charges (250 requests @ $0.005)",
                            Quantity = 250,
                            UnitPrice = 0.005m,
                            Amount = 1.25m
                        }
                    },
                    DownloadUrl = "https://contentmaster.p.rapidapi.com/api/marketplace/payments/invoices/inv_2/pdf"
                }
            };

            return Ok(invoices);
        }
    }

    /// <summary>
    /// Payment method information
    /// </summary>
    public class PaymentMethod
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Brand { get; set; }
        public string Last4 { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public bool IsDefault { get; set; }
    }

    /// <summary>
    /// Payment transaction information
    /// </summary>
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

    /// <summary>
    /// Invoice information
    /// </summary>
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

    /// <summary>
    /// Invoice item information
    /// </summary>
    public class InvoiceItem
    {
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
    }
}
