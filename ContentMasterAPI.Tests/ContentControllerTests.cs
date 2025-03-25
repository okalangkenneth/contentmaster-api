using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using ContentMasterAPI.Core.Models;

namespace ContentMasterAPI.Tests
{
    public class ContentControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ContentControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/api/content");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetById_WithValidId_ReturnsContent()
        {
            // Arrange
            var newContent = CreateTestContent();
            var createResponse = await _client.PostAsJsonAsync("/api/content", newContent);
            createResponse.EnsureSuccessStatusCode();
            var createdContent = await createResponse.Content.ReadFromJsonAsync<Content>();

            // Act
            var response = await _client.GetAsync($"/api/content/{createdContent.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadFromJsonAsync<Content>();
            Assert.Equal(createdContent.Id, content.Id);
            Assert.Equal(newContent.Title, content.Title);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync($"/api/content/{Guid.NewGuid()}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Create_WithValidContent_ReturnsCreatedContent()
        {
            // Arrange
            var newContent = CreateTestContent();

            // Act
            var response = await _client.PostAsJsonAsync("/api/content", newContent);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var content = await response.Content.ReadFromJsonAsync<Content>();
            Assert.NotEqual(Guid.Empty, content.Id);
            Assert.Equal(newContent.Title, content.Title);
        }

        [Fact]
        public async Task Update_WithValidContent_ReturnsNoContent()
        {
            // Arrange
            var newContent = CreateTestContent();
            var createResponse = await _client.PostAsJsonAsync("/api/content", newContent);
            createResponse.EnsureSuccessStatusCode();
            var createdContent = await createResponse.Content.ReadFromJsonAsync<Content>();

            // Update the content
            createdContent.Title = "Updated Title";

            // Act
            var response = await _client.PutAsJsonAsync($"/api/content/{createdContent.Id}", createdContent);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify the update
            var getResponse = await _client.GetAsync($"/api/content/{createdContent.Id}");
            getResponse.EnsureSuccessStatusCode();
            var updatedContent = await getResponse.Content.ReadFromJsonAsync<Content>();
            Assert.Equal("Updated Title", updatedContent.Title);
        }

        [Fact]
        public async Task Delete_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var newContent = CreateTestContent();
            var createResponse = await _client.PostAsJsonAsync("/api/content", newContent);
            createResponse.EnsureSuccessStatusCode();
            var createdContent = await createResponse.Content.ReadFromJsonAsync<Content>();

            // Act
            var response = await _client.DeleteAsync($"/api/content/{createdContent.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify the deletion
            var getResponse = await _client.GetAsync($"/api/content/{createdContent.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task Search_WithValidTerm_ReturnsMatchingContent()
        {
            // Arrange
            var newContent = CreateTestContent();
            newContent.Title = "Unique Search Term Test";
            var createResponse = await _client.PostAsJsonAsync("/api/content", newContent);
            createResponse.EnsureSuccessStatusCode();

            // Act
            var response = await _client.GetAsync("/api/content/search?searchTerm=Unique Search Term");

            // Assert
            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadFromJsonAsync<List<Content>>();
            Assert.Contains(contents, c => c.Title == "Unique Search Term Test");
        }

        private Content CreateTestContent()
        {
            return new Content
            {
                Title = "Test Content",
                Body = "This is a test content item for unit testing.",
                ContentType = "test",
                CreatedBy = "test_user",
                Status = "draft",
                Tags = new List<string> { "test", "unit-test" },
                Metadata = new Dictionary<string, string>
                {
                    { "test", "value" }
                }
            };
        }
    }
}
