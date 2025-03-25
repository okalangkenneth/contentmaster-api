using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ContentMasterAPI.Tests
{
    public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public AuthControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            // Arrange
            var loginRequest = new
            {
                Username = "admin",
                Password = "admin123"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<dynamic>();
            Assert.NotNull(result);
            Assert.Equal("admin", result.GetProperty("username").GetString());
            Assert.NotNull(result.GetProperty("token").GetString());
            Assert.True(result.GetProperty("expiresIn").GetInt32() > 0);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new
            {
                Username = "invalid",
                Password = "invalid"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ValidateToken_WithValidToken_ReturnsUserInfo()
        {
            // Arrange - Login to get a token
            var loginRequest = new
            {
                Username = "admin",
                Password = "admin123"
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
            loginResponse.EnsureSuccessStatusCode();
            var authResult = await loginResponse.Content.ReadFromJsonAsync<dynamic>();
            var token = authResult.GetProperty("token").GetString();

            // Set the authorization header
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/auth/validate");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<dynamic>();
            Assert.NotNull(result);
            Assert.Equal("admin", result.GetProperty("username").GetString());
            Assert.True(result.GetProperty("isAuthenticated").GetBoolean());
        }

        [Fact]
        public async Task ValidateToken_WithoutToken_ReturnsUnauthorized()
        {
            // Arrange - Clear any authorization headers
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync("/api/auth/validate");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
