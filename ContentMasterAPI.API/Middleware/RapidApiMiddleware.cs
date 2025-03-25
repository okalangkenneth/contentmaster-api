using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace ContentMasterAPI.API.Middleware
{
    /// <summary>
    /// Middleware for handling RapidAPI authentication and request validation
    /// </summary>
    public class RapidApiMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RapidApiMiddleware> _logger;
        private readonly IConfiguration _configuration;
        private readonly Dictionary<string, SubscriptionTier> _apiKeys;

        // Constants for header names
        private const string RapidApiKeyHeader = "X-RapidAPI-Key";
        private const string RapidApiHostHeader = "X-RapidAPI-Host";
        
        public RapidApiMiddleware(
            RequestDelegate next,
            ILogger<RapidApiMiddleware> logger,
            IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _configuration = configuration;
            
            // In a production environment, these would be loaded from a database or configuration
            // For demo purposes, we're hardcoding some sample API keys with their subscription tiers
            _apiKeys = new Dictionary<string, SubscriptionTier>
            {
                { "demo-basic-api-key", SubscriptionTier.Basic },
                { "demo-pro-api-key", SubscriptionTier.Pro },
                { "demo-ultra-api-key", SubscriptionTier.Ultra },
                { "demo-mega-api-key", SubscriptionTier.Mega }
            };
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip RapidAPI validation for Swagger and development environments if needed
            if (context.Request.Path.StartsWithSegments("/swagger") || 
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                await _next(context);
                return;
            }

            // Check for RapidAPI headers
            if (!context.Request.Headers.TryGetValue(RapidApiKeyHeader, out var apiKeyValues))
            {
                _logger.LogWarning("Request missing RapidAPI key header");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new 
                {
                    statusCode = StatusCodes.Status401Unauthorized,
                    message = "Missing RapidAPI key header",
                    errorType = "AuthenticationError",
                    requestId = context.TraceIdentifier,
                    timestamp = DateTime.UtcNow
                });
                return;
            }

            if (!context.Request.Headers.TryGetValue(RapidApiHostHeader, out var hostValues))
            {
                _logger.LogWarning("Request missing RapidAPI host header");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new 
                {
                    statusCode = StatusCodes.Status401Unauthorized,
                    message = "Missing RapidAPI host header",
                    errorType = "AuthenticationError",
                    requestId = context.TraceIdentifier,
                    timestamp = DateTime.UtcNow
                });
                return;
            }

            var apiKey = apiKeyValues.FirstOrDefault();
            var host = hostValues.FirstOrDefault();

            // Validate API key
            if (string.IsNullOrEmpty(apiKey) || !_apiKeys.TryGetValue(apiKey, out var tier))
            {
                _logger.LogWarning("Invalid RapidAPI key: {ApiKey}", apiKey);
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new 
                {
                    statusCode = StatusCodes.Status401Unauthorized,
                    message = "Invalid RapidAPI key",
                    errorType = "AuthenticationError",
                    requestId = context.TraceIdentifier,
                    timestamp = DateTime.UtcNow
                });
                return;
            }

            // Validate host (in production, you would check against your configured host)
            if (string.IsNullOrEmpty(host) || !host.Contains("rapidapi.com"))
            {
                _logger.LogWarning("Invalid RapidAPI host: {Host}", host);
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new 
                {
                    statusCode = StatusCodes.Status401Unauthorized,
                    message = "Invalid RapidAPI host",
                    errorType = "AuthenticationError",
                    requestId = context.TraceIdentifier,
                    timestamp = DateTime.UtcNow
                });
                return;
            }

            // Add subscription tier to the HttpContext items for use in controllers
            context.Items["SubscriptionTier"] = tier;
            
            // Check if the endpoint is restricted based on subscription tier
            if (!IsEndpointAllowedForTier(context.Request.Path, tier))
            {
                _logger.LogWarning("Access to endpoint {Endpoint} denied for tier {Tier}", 
                    context.Request.Path, tier);
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new 
                {
                    statusCode = StatusCodes.Status403Forbidden,
                    message = "Your subscription tier does not have access to this endpoint",
                    errorType = "SubscriptionLimitError",
                    requestId = context.TraceIdentifier,
                    timestamp = DateTime.UtcNow
                });
                return;
            }

            // Continue with the request pipeline
            await _next(context);
        }

        private bool IsEndpointAllowedForTier(PathString path, SubscriptionTier tier)
        {
            // Basic tier can only access content endpoints (no analytics)
            if (tier == SubscriptionTier.Basic && path.StartsWithSegments("/api/analytics"))
            {
                return false;
            }

            // Pro tier can access content endpoints and basic analytics (sentiment only)
            if (tier == SubscriptionTier.Pro && 
                path.StartsWithSegments("/api/analytics") && 
                !path.Value.Contains("/sentiment"))
            {
                return false;
            }

            // Ultra and Mega tiers have access to all endpoints
            return true;
        }
    }

    /// <summary>
    /// Subscription tiers for the ContentMaster API
    /// </summary>
    public enum SubscriptionTier
    {
        Basic,  // Free tier
        Pro,    // $25/month
        Ultra,  // $75/month
        Mega    // $150/month
    }

    /// <summary>
    /// Extension methods for the RapidAPI middleware
    /// </summary>
    public static class RapidApiMiddlewareExtensions
    {
        public static IApplicationBuilder UseRapidApiAuthentication(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RapidApiMiddleware>();
        }
    }
}
