using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContentMasterAPI.Examples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("ContentMaster API Example Client");
            Console.WriteLine("================================");
            
            // Initialize the API client
            var apiClient = new ContentMasterApiClient("https://localhost:7001/");
            
            // Login to the API
            bool isAuthenticated = await apiClient.LoginAsync("admin", "admin123");
            if (!isAuthenticated)
            {
                Console.WriteLine("Authentication failed. Exiting...");
                return;
            }
            
            // Create a new content item
            var newContent = new Content
            {
                Title = "Getting Started with ContentMaster API",
                Body = "This is a comprehensive guide to help you get started with the ContentMaster API. " +
                       "It provides examples of how to use the various features and endpoints.",
                ContentType = "article",
                CreatedBy = "example_user",
                Status = "draft",
                Tags = new List<string> { "api", "getting-started", "tutorial" },
                Metadata = new Dictionary<string, string>
                {
                    { "readTime", "5 minutes" },
                    { "category", "Documentation" }
                }
            };
            
            var createdContent = await apiClient.CreateContentAsync(newContent);
            if (createdContent == null)
            {
                Console.WriteLine("Failed to create content. Exiting...");
                return;
            }
            
            // Get all content items
            var allContent = await apiClient.GetAllContentAsync();
            Console.WriteLine("\nAll Content Items:");
            foreach (var content in allContent)
            {
                Console.WriteLine($"- {content.Title} ({content.ContentType})");
            }
            
            // Analyze sentiment of the created content
            var sentimentResult = await apiClient.AnalyzeSentimentAsync(createdContent.Id);
            if (sentimentResult != null)
            {
                Console.WriteLine($"\nSentiment Analysis for '{createdContent.Title}':");
                Console.WriteLine($"Score: {sentimentResult.SentimentScore}");
                Console.WriteLine($"Label: {sentimentResult.SentimentLabel}");
            }
            
            // Execute a GraphQL query
            Console.WriteLine("\nExecuting GraphQL Query...");
            var graphQLQuery = @"
            {
              contents {
                id
                title
                contentType
                tags
              }
            }";
            
            var graphQLResult = await apiClient.ExecuteGraphQLQueryAsync<dynamic>(graphQLQuery);
            Console.WriteLine("GraphQL Query Results:");
            Console.WriteLine(JsonSerializer.Serialize(graphQLResult, new JsonSerializerOptions { WriteIndented = true }));
            
            Console.WriteLine("\nExample completed successfully!");
        }
    }
}
