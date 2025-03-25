using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

namespace ContentMasterAPI.Examples
{
    /// <summary>
    /// Example client for the ContentMaster API
    /// </summary>
    public class ContentMasterApiClient
    {
        private readonly HttpClient _httpClient;
        private string _token;

        public ContentMasterApiClient(string baseUrl)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Authenticates with the API and stores the token for subsequent requests
        /// </summary>
        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                var loginRequest = new
                {
                    Username = username,
                    Password = password
                };

                var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginRequest);
                response.EnsureSuccessStatusCode();

                var authResult = await response.Content.ReadFromJsonAsync<AuthResponse>();
                _token = authResult.Token;

                // Set the authorization header for subsequent requests
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                
                Console.WriteLine($"Successfully logged in as {username}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets all content items
        /// </summary>
        public async Task<List<Content>> GetAllContentAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/content");
                response.EnsureSuccessStatusCode();

                var contents = await response.Content.ReadFromJsonAsync<List<Content>>();
                Console.WriteLine($"Retrieved {contents.Count} content items");
                return contents;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving content: {ex.Message}");
                return new List<Content>();
            }
        }

        /// <summary>
        /// Creates a new content item
        /// </summary>
        public async Task<Content> CreateContentAsync(Content content)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/content", content);
                response.EnsureSuccessStatusCode();

                var createdContent = await response.Content.ReadFromJsonAsync<Content>();
                Console.WriteLine($"Created content with ID: {createdContent.Id}");
                return createdContent;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating content: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Analyzes the sentiment of a content item
        /// </summary>
        public async Task<SentimentResult> AnalyzeSentimentAsync(Guid contentId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/analytics/{contentId}/sentiment");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<SentimentResult>();
                Console.WriteLine($"Sentiment analysis: {result.SentimentLabel} ({result.SentimentScore})");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error analyzing sentiment: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Executes a GraphQL query
        /// </summary>
        public async Task<T> ExecuteGraphQLQueryAsync<T>(string query)
        {
            try
            {
                var request = new
                {
                    query = query
                };

                var response = await _httpClient.PostAsJsonAsync("api/graphql", request);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<GraphQLResponse<T>>();
                Console.WriteLine("GraphQL query executed successfully");
                return result.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing GraphQL query: {ex.Message}");
                return default;
            }
        }
    }

    #region Models

    public class Content
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string ContentType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string Status { get; set; }
        public List<string> Tags { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public int Version { get; set; }
    }

    public class AuthResponse
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public int ExpiresIn { get; set; }
    }

    public class SentimentResult
    {
        public Guid ContentId { get; set; }
        public string Title { get; set; }
        public float SentimentScore { get; set; }
        public string SentimentLabel { get; set; }
    }

    public class GraphQLResponse<T>
    {
        public T Data { get; set; }
        public List<GraphQLError> Errors { get; set; }
    }

    public class GraphQLError
    {
        public string Message { get; set; }
        public List<GraphQLLocation> Locations { get; set; }
        public List<string> Path { get; set; }
    }

    public class GraphQLLocation
    {
        public int Line { get; set; }
        public int Column { get; set; }
    }

    #endregion
}
