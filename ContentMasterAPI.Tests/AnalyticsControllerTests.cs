using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using ContentMasterAPI.Core.Models;

namespace ContentMasterAPI.Tests
{
    public class AnalyticsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public AnalyticsControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task AnalyzeSentiment_WithValidId_ReturnsSentimentScore()
        {
            // Arrange - Create a content item to analyze
            var content = await CreateTestContent();

            // Act
            var response = await _client.GetAsync($"/api/analytics/{content.Id}/sentiment");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<dynamic>();
            Assert.NotNull(result);
            Assert.Equal(content.Id.ToString(), result.GetProperty("contentId").ToString());
            Assert.True(result.GetProperty("sentimentScore").GetDouble() >= 0);
            Assert.True(result.GetProperty("sentimentScore").GetDouble() <= 1);
            Assert.NotNull(result.GetProperty("sentimentLabel").GetString());
        }

        [Fact]
        public async Task GenerateTags_WithValidId_ReturnsSuggestedTags()
        {
            // Arrange - Create a content item to analyze
            var content = await CreateTestContent();

            // Act
            var response = await _client.GetAsync($"/api/analytics/{content.Id}/tags");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<dynamic>();
            Assert.NotNull(result);
            Assert.Equal(content.Id.ToString(), result.GetProperty("contentId").ToString());
            Assert.True(result.GetProperty("suggestedTags").GetArrayLength() > 0);
        }

        [Fact]
        public async Task GenerateSummary_WithValidId_ReturnsSummary()
        {
            // Arrange - Create a content item to analyze
            var content = await CreateTestContent();

            // Act
            var response = await _client.GetAsync($"/api/analytics/{content.Id}/summary");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<dynamic>();
            Assert.NotNull(result);
            Assert.Equal(content.Id.ToString(), result.GetProperty("contentId").ToString());
            Assert.NotNull(result.GetProperty("summary").GetString());
        }

        [Fact]
        public async Task CategorizeContent_WithValidId_ReturnsSuggestedCategory()
        {
            // Arrange - Create a content item to analyze
            var content = await CreateTestContent();

            // Act
            var response = await _client.GetAsync($"/api/analytics/{content.Id}/category");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<dynamic>();
            Assert.NotNull(result);
            Assert.Equal(content.Id.ToString(), result.GetProperty("contentId").ToString());
            Assert.NotNull(result.GetProperty("suggestedCategory").GetString());
        }

        private async Task<Content> CreateTestContent()
        {
            var newContent = new Content
            {
                Title = "AI Analysis Test Content",
                Body = "This is a test content item for AI analysis. It contains positive sentiment and should be categorized as a technical article about API development and testing.",
                ContentType = "article",
                CreatedBy = "test_user",
                Status = "draft",
                Tags = new List<string> { "test", "ai", "analysis" },
                Metadata = new Dictionary<string, string>
                {
                    { "test", "value" }
                }
            };

            var response = await _client.PostAsJsonAsync("/api/content", newContent);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Content>();
        }
    }
}
