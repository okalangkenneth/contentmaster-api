# ContentMaster API - Commercial Documentation

## Overview

ContentMaster API is a modern content management API with AI-driven capabilities, designed for commercial use through RapidAPI Marketplace. This API provides comprehensive content management features, AI-powered analytics, and GraphQL support, all accessible through a tiered subscription model.

## Marketplace Integration

ContentMaster API is fully integrated with RapidAPI Marketplace, allowing for easy discovery, subscription management, and monetization. The API uses standard RapidAPI authentication mechanisms and supports various pricing tiers to accommodate different user needs.

### Authentication

All requests to ContentMaster API must include the following headers:

```
X-RapidAPI-Key: your-rapidapi-key
X-RapidAPI-Host: contentmaster.p.rapidapi.com
```

These headers are automatically included when accessing the API through RapidAPI Marketplace.

## Subscription Plans

ContentMaster API offers the following subscription plans:

### Basic (Free)
- Basic content operations
- Limited quota (100 requests per day)
- No AI analytics features

### Pro ($25/month)
- Full content management operations
- 5,000 requests per month
- Basic AI analytics (sentiment analysis only)
- Standard rate limits
- Overage: $0.005 per additional request

### Ultra ($75/month)
- All content management operations
- 20,000 requests per month
- Full AI analytics capabilities
- Higher rate limits
- Priority support
- Overage: $0.003 per additional request

### Mega ($150/month)
- All features with no restrictions
- 100,000 requests per month
- Highest rate limits
- Premium support
- Custom analytics reports
- Overage: $0.001 per additional request

## API Endpoints

### Content Management

#### GET /api/content
Get all content items with pagination support.

**Parameters:**
- `page` (optional): Page number (default: 1)
- `pageSize` (optional): Number of items per page (default: 10)

**Response:**
```json
[
  {
    "id": "guid",
    "title": "Sample Content",
    "body": "This is sample content",
    "contentType": "article",
    "createdAt": "2025-03-20T12:00:00Z",
    "updatedAt": "2025-03-20T12:00:00Z",
    "createdBy": "user123",
    "status": "published",
    "tags": ["sample", "api"],
    "metadata": {
      "readTime": "2 minutes"
    },
    "version": 1
  }
]
```

#### POST /api/content
Create a new content item.

**Request Body:**
```json
{
  "title": "New Content",
  "body": "This is the content body",
  "contentType": "article",
  "tags": ["sample", "api"]
}
```

**Response:**
```json
{
  "id": "guid",
  "title": "New Content",
  "body": "This is the content body",
  "contentType": "article",
  "createdAt": "2025-03-20T12:00:00Z",
  "updatedAt": "2025-03-20T12:00:00Z",
  "createdBy": "user123",
  "status": "draft",
  "tags": ["sample", "api"],
  "metadata": {},
  "version": 1
}
```

#### GET /api/content/{id}
Get a specific content item by ID.

**Parameters:**
- `id`: Content ID

**Response:**
```json
{
  "id": "guid",
  "title": "Sample Content",
  "body": "This is sample content",
  "contentType": "article",
  "createdAt": "2025-03-20T12:00:00Z",
  "updatedAt": "2025-03-20T12:00:00Z",
  "createdBy": "user123",
  "status": "published",
  "tags": ["sample", "api"],
  "metadata": {
    "readTime": "2 minutes"
  },
  "version": 1
}
```

### AI Analytics

#### GET /api/analytics/{id}/sentiment
Analyze sentiment of a content item.

**Parameters:**
- `id`: Content ID

**Response:**
```json
{
  "contentId": "guid",
  "title": "Sample Content",
  "sentimentScore": 0.75,
  "sentimentLabel": "positive"
}
```

#### GET /api/analytics/{id}/tags
Generate tags for a content item (Ultra and Mega plans only).

**Parameters:**
- `id`: Content ID

**Response:**
```json
{
  "contentId": "guid",
  "title": "Sample Content",
  "tags": ["technology", "ai", "api", "development"]
}
```

#### GET /api/analytics/{id}/category
Categorize a content item (Ultra and Mega plans only).

**Parameters:**
- `id`: Content ID

**Response:**
```json
{
  "contentId": "guid",
  "title": "Sample Content",
  "category": "Technology",
  "confidence": 0.85
}
```

#### GET /api/analytics/{id}/summary
Generate a summary of a content item (Ultra and Mega plans only).

**Parameters:**
- `id`: Content ID

**Response:**
```json
{
  "contentId": "guid",
  "title": "Sample Content",
  "summary": "This is a summarized version of the content.",
  "originalLength": 1250,
  "summaryLength": 150
}
```

### GraphQL Support

#### POST /api/graphql
Execute GraphQL queries.

**Request Body:**
```json
{
  "query": "{ content(id: \"guid\") { id title body tags } }"
}
```

**Response:**
```json
{
  "data": {
    "content": {
      "id": "guid",
      "title": "Sample Content",
      "body": "This is sample content",
      "tags": ["sample", "api"]
    }
  }
}
```

### Marketplace Features

#### GET /api/marketplace/info
Get information about the API for marketplace listings.

**Response:**
```json
{
  "name": "ContentMaster API",
  "version": "1.0.0",
  "description": "A modern content management API with AI-driven capabilities",
  "provider": "ContentMaster",
  "logoUrl": "https://contentmasterapi.com/logo.png",
  "documentationUrl": "https://contentmasterapi.com/docs",
  "categories": ["Content Management", "AI", "Machine Learning", "Analytics"],
  "tags": ["content", "ai", "analytics", "graphql", "sentiment-analysis", "tagging"],
  "features": [
    {
      "name": "Content Management",
      "description": "Full CRUD operations for content items with versioning and metadata"
    },
    {
      "name": "AI-Driven Analytics",
      "description": "Sentiment analysis, auto-tagging, content categorization, and summarization"
    },
    {
      "name": "GraphQL Support",
      "description": "Flexible querying capabilities for clients to request exactly what they need"
    },
    {
      "name": "Security",
      "description": "JWT authentication and authorization with role-based access control"
    }
  ],
  "pricingTiers": [
    {
      "name": "Basic",
      "price": 0,
      "interval": "month",
      "currency": "USD",
      "description": "Free tier with limited access",
      "features": [
        "Basic content operations",
        "Limited quota (100 requests per day)",
        "No AI analytics features"
      ],
      "quota": 100,
      "quotaInterval": "day"
    },
    {
      "name": "Pro",
      "price": 25,
      "interval": "month",
      "currency": "USD",
      "description": "Professional tier for individual developers",
      "features": [
        "Full content management operations",
        "5,000 requests per month",
        "Basic AI analytics (sentiment analysis only)",
        "Standard rate limits"
      ],
      "quota": 5000,
      "quotaInterval": "month",
      "overagePrice": 0.005,
      "overageInterval": "request"
    }
  ]
}
```

#### GET /api/usage
Get usage statistics for the current API key.

**Response:**
```json
{
  "apiKey": "your-rapidapi-key",
  "subscriptionTier": "Pro",
  "dailyRequestCount": 45,
  "dailyQuota": 167,
  "remainingRequests": 122,
  "endpointUsage": {
    "/api/content": 25,
    "/api/content/{id}": 15,
    "/api/analytics/{id}/sentiment": 5
  }
}
```

#### GET /api/marketplace/analytics/usage
Get API usage analytics (for API providers).

**Response:**
```json
{
  "period": "Last 30 days",
  "startDate": "2025-02-20T00:00:00Z",
  "endDate": "2025-03-20T00:00:00Z",
  "totalRequests": 12450,
  "uniqueUsers": 42,
  "dailyUsage": [
    {
      "date": "2025-03-20T00:00:00Z",
      "requests": 450,
      "uniqueUsers": 35
    }
  ],
  "endpointUsage": {
    "/api/content": 5230,
    "/api/content/{id}": 3120,
    "/api/analytics/{id}/sentiment": 1840
  },
  "subscriptionDistribution": {
    "Basic": 25,
    "Pro": 12,
    "Ultra": 4,
    "Mega": 1
  }
}
```

### Monetization Endpoints

#### GET /api/marketplace/monetization/plans
Get subscription plans for the API.

**Response:**
```json
[
  {
    "id": "basic",
    "name": "Basic",
    "description": "Free tier with limited access",
    "price": 0,
    "currency": "USD",
    "interval": "month",
    "features": [
      "Basic content operations",
      "Limited quota (100 requests per day)",
      "No AI analytics features"
    ],
    "limits": {
      "dailyRequests": 100,
      "monthlyRequests": 3000,
      "rateLimit": 10
    },
    "isPopular": false
  },
  {
    "id": "pro",
    "name": "Pro",
    "description": "Professional tier for individual developers",
    "price": 25,
    "currency": "USD",
    "interval": "month",
    "features": [
      "Full content management operations",
      "5,000 requests per month",
      "Basic AI analytics (sentiment analysis only)",
      "Standard rate limits",
      "Overage: $0.005 per additional request"
    ],
    "limits": {
      "dailyRequests": 167,
      "monthlyRequests": 5000,
      "rateLimit": 30
    },
    "isPopular": true
  }
]
```

#### GET /api/marketplace/monetization/billing
Get billing information for the current API key.

**Response:**
```json
{
  "subscriptionPlan": "Pro",
  "status": "active",
  "currentPeriodStart": "2025-03-05T00:00:00Z",
  "currentPeriodEnd": "2025-04-05T00:00:00Z",
  "monthlyQuota": 5000,
  "usedQuota": 2345,
  "remainingQuota": 2655,
  "overageUsage": 0,
  "overageRate": 0.005,
  "currentCharges": 25.00,
  "overageCharges": 0.00,
  "totalCharges": 25.00,
  "billingCycle": "monthly",
  "nextBillingDate": "2025-04-05T00:00:00Z",
  "paymentMethod": "credit_card",
  "paymentMethodLast4": "1234"
}
```

#### GET /api/marketplace/payments/methods
Get payment methods for the current API key.

**Response:**
```json
[
  {
    "id": "pm_1",
    "type": "credit_card",
    "brand": "visa",
    "last4": "4242",
    "expiryMonth": 12,
    "expiryYear": 2026,
    "isDefault": true
  },
  {
    "id": "pm_2",
    "type": "credit_card",
    "brand": "mastercard",
    "last4": "5555",
    "expiryMonth": 10,
    "expiryYear": 2025,
    "isDefault": false
  }
]
```

#### GET /api/marketplace/subscriptions/current
Get subscription information for the current API key.

**Response:**
```json
{
  "id": "sub_1",
  "planId": "pro",
  "planName": "Pro",
  "status": "active",
  "currentPeriodStart": "2025-03-05T00:00:00Z",
  "currentPeriodEnd": "2025-04-05T00:00:00Z",
  "createdAt": "2025-02-05T00:00:00Z",
  "cancelAtPeriodEnd": false,
  "price": 25.00,
  "currency": "USD",
  "interval": "month",
  "billingDetails": {
    "name": "John Doe",
    "email": "john.doe@example.com",
    "company": "Example Corp",
    "address": {
      "line1": "123 Main St",
      "line2": "Suite 100",
      "city": "San Francisco",
      "state": "CA",
      "postalCode": "94105",
      "country": "US"
    },
    "vatId": "US123456789"
  },
  "paymentMethod": {
    "id": "pm_1",
    "type": "credit_card",
    "brand": "visa",
    "last4": "4242",
    "expiryMonth": 12,
    "expiryYear": 2026
  }
}
```

## Code Examples

### C#

```csharp
var client = new RestClient("https://contentmaster.p.rapidapi.com/api/content");
var request = new RestRequest(Method.GET);
request.AddHeader("X-RapidAPI-Key", "your-rapidapi-key");
request.AddHeader("X-RapidAPI-Host", "contentmaster.p.rapidapi.com");
IRestResponse response = client.Execute(request);
Console.WriteLine(response.Content);
```

### JavaScript

```javascript
const options = {
  method: 'GET',
  headers: {
    'X-RapidAPI-Key': 'your-rapidapi-key',
    'X-RapidAPI-Host': 'contentmaster.p.rapidapi.com'
  }
};

fetch('https://contentmaster.p.rapidapi.com/api/content', options)
  .then(response => response.json())
  .then(response => console.log(response))
  .catch(err => console.error(err));
```

### Python

```python
import requests

url = "https://contentmaster.p.rapidapi.com/api/content"

headers = {
    "X-RapidAPI-Key": "your-rapidapi-key",
    "X-RapidAPI-Host": "contentmaster.p.rapidapi.com"
}

response = requests.request("GET", url, headers=headers)

print(response.text)
```

## Rate Limits

Rate limits vary by subscription tier:

- Basic: 10 requests per minute
- Pro: 30 requests per minute
- Ultra: 60 requests per minute
- Mega: 120 requests per minute

## Support

For support inquiries, please contact support@contentmasterapi.com or visit our documentation at https://contentmasterapi.com/docs.

## Terms of Service

By using ContentMaster API, you agree to our Terms of Service and Privacy Policy, which can be found at https://contentmasterapi.com/terms and https://contentmasterapi.com/privacy.
