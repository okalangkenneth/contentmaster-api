# ContentMaster API Documentation

## Overview

ContentMaster API is a modern content management API built with C# and .NET 8. It provides a comprehensive set of features for managing content, including AI-driven capabilities, GraphQL support, and robust security.

## Key Features

- **Content Management**: Full CRUD operations for content items
- **AI-Driven Capabilities**: Sentiment analysis, auto-tagging, content categorization, and summarization
- **GraphQL Support**: Flexible querying capabilities
- **Security**: JWT authentication and authorization
- **Error Handling**: Consistent error responses across the API

## Getting Started

### Prerequisites

- .NET 8 SDK
- Visual Studio 2022 or Visual Studio Code

### Installation

1. Clone the repository:
```bash
git clone https://github.com/yourusername/ContentMasterAPI.git
```

2. Navigate to the project directory:
```bash
cd ContentMasterAPI
```

3. Build the solution:
```bash
dotnet build
```

4. Run the API:
```bash
cd ContentMasterAPI.API
dotnet run
```

5. Access the Swagger UI at:
```
https://localhost:7001/
```

## API Endpoints

### Content Management

#### Get All Content
```
GET /api/content
```
Returns a list of all content items.

#### Get Content by ID
```
GET /api/content/{id}
```
Returns a specific content item by its ID.

#### Create Content
```
POST /api/content
```
Creates a new content item.

Example request body:
```json
{
  "title": "Sample Content",
  "body": "This is the body of the sample content.",
  "contentType": "article",
  "createdBy": "user123",
  "status": "draft",
  "tags": ["sample", "api", "documentation"],
  "metadata": {
    "readTime": "3 minutes",
    "category": "API"
  }
}
```

#### Update Content
```
PUT /api/content/{id}
```
Updates an existing content item.

#### Delete Content
```
DELETE /api/content/{id}
```
Deletes a content item.

#### Search Content
```
GET /api/content/search?searchTerm={term}&contentType={type}&tags={tags}
```
Searches for content items based on search criteria.

### AI Analytics

#### Analyze Sentiment
```
GET /api/analytics/{id}/sentiment
```
Analyzes the sentiment of a content item.

#### Generate Tags
```
GET /api/analytics/{id}/tags
```
Generates tags for a content item.

#### Generate Summary
```
GET /api/analytics/{id}/summary?maxLength={length}
```
Generates a summary of a content item.

#### Categorize Content
```
GET /api/analytics/{id}/category
```
Categorizes a content item.

### Authentication

#### Login
```
POST /api/auth/login
```
Authenticates a user and returns a JWT token.

Example request body:
```json
{
  "username": "admin",
  "password": "admin123"
}
```

Example response:
```json
{
  "username": "admin",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 3600
}
```

#### Validate Token
```
GET /api/auth/validate
```
Validates the current token.

### GraphQL

```
POST /api/graphql
```
Executes GraphQL queries against the API.

Example request body:
```json
{
  "query": "{ contents { id title body contentType } }"
}
```

## Authentication

The API uses JWT (JSON Web Tokens) for authentication. To access protected endpoints, include the JWT token in the Authorization header:

```
Authorization: Bearer {your_token}
```

## Error Handling

The API returns consistent error responses with the following structure:

```json
{
  "statusCode": 400,
  "message": "Error message",
  "errorType": "ArgumentException",
  "requestId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "timestamp": "2025-03-20T16:45:30Z"
}
```

## Rate Limiting

The API implements rate limiting to prevent abuse. The default limits are:

- 100 requests per minute for authenticated users
- 20 requests per minute for unauthenticated users

## Best Practices

1. **Use GraphQL for complex queries**: When you need to fetch multiple related resources, use GraphQL to minimize network requests.
2. **Leverage AI capabilities**: Use the analytics endpoints to extract insights from your content.
3. **Implement proper error handling**: Always check for error responses and handle them appropriately.
4. **Use pagination**: When fetching large collections, use pagination parameters to improve performance.
5. **Cache responses**: Implement client-side caching for frequently accessed resources.

## Examples

### Creating and Analyzing Content

```csharp
// Create a new content item
var content = new Content
{
    Title = "Understanding GraphQL",
    Body = "GraphQL is a query language for APIs and a runtime for fulfilling those queries with your existing data.",
    ContentType = "article",
    CreatedBy = "user123",
    Status = "draft",
    Tags = new List<string> { "graphql", "api", "tutorial" },
    Metadata = new Dictionary<string, string>
    {
        { "readTime", "5 minutes" },
        { "category", "Technical" }
    }
};

// POST to /api/content
var createResponse = await httpClient.PostAsJsonAsync("api/content", content);
var createdContent = await createResponse.Content.ReadFromJsonAsync<Content>();

// Analyze sentiment
var sentimentResponse = await httpClient.GetAsync($"api/analytics/{createdContent.Id}/sentiment");
var sentiment = await sentimentResponse.Content.ReadFromJsonAsync<SentimentResult>();

Console.WriteLine($"Content sentiment: {sentiment.SentimentLabel} ({sentiment.SentimentScore})");

// Generate tags
var tagsResponse = await httpClient.GetAsync($"api/analytics/{createdContent.Id}/tags");
var tags = await tagsResponse.Content.ReadFromJsonAsync<TagsResult>();

Console.WriteLine("Suggested tags:");
foreach (var tag in tags.SuggestedTags)
{
    Console.WriteLine($"- {tag}");
}
```

### Using GraphQL

```csharp
// Define a GraphQL query
var query = new
{
    query = @"
    {
      contents {
        id
        title
        contentType
        createdAt
        tags
      }
    }"
};

// POST to /api/graphql
var graphQLResponse = await httpClient.PostAsJsonAsync("api/graphql", query);
var result = await graphQLResponse.Content.ReadFromJsonAsync<GraphQLResponse>();

// Process the results
foreach (var content in result.Data.Contents)
{
    Console.WriteLine($"Title: {content.Title}");
    Console.WriteLine($"Type: {content.ContentType}");
    Console.WriteLine($"Created: {content.CreatedAt}");
    Console.WriteLine($"Tags: {string.Join(", ", content.Tags)}");
    Console.WriteLine();
}
```

### Authentication

```csharp
// Login to get a token
var loginRequest = new
{
    Username = "admin",
    Password = "admin123"
};

var loginResponse = await httpClient.PostAsJsonAsync("api/auth/login", loginRequest);
var authResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

// Use the token for subsequent requests
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult.Token);

// Now you can access protected endpoints
var protectedResponse = await httpClient.GetAsync("api/content");
```

## Conclusion

The ContentMaster API provides a powerful and flexible platform for content management with modern features like AI-driven analytics and GraphQL support. By following the documentation and examples, you can quickly integrate the API into your applications and leverage its capabilities.

For additional support or to report issues, please contact our support team or open an issue on GitHub.
