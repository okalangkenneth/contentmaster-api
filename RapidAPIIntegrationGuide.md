# RapidAPI Integration Guide for ContentMaster API

This guide provides detailed instructions for integrating the ContentMaster API with RapidAPI Marketplace to enable commercial usage.

## Prerequisites

Before integrating with RapidAPI, ensure you have:

1. A RapidAPI account (sign up at [rapidapi.com](https://rapidapi.com))
2. The ContentMaster API running and accessible via a public URL
3. Admin access to configure your API settings

## Integration Steps

### 1. Create a RapidAPI Provider Account

1. Sign in to RapidAPI.com
2. Navigate to "My APIs" in the dashboard
3. Click "Add New API" to begin the process

### 2. Configure API Settings

#### General Information

- **API Name**: ContentMaster API
- **API Description**: A modern content management API with AI-driven capabilities, GraphQL support, and robust security
- **API Category**: Content Management, AI & Machine Learning
- **API Tags**: content-management, ai-analytics, graphql, sentiment-analysis

#### API Endpoints

Add the following endpoint groups:

1. **Content Management**
   - GET /api/content
   - POST /api/content
   - GET /api/content/{id}
   - PUT /api/content/{id}
   - DELETE /api/content/{id}

2. **AI Analytics**
   - GET /api/analytics/{id}/sentiment
   - GET /api/analytics/{id}/tags
   - GET /api/analytics/{id}/category
   - GET /api/analytics/{id}/summary

3. **GraphQL**
   - POST /api/graphql

4. **Marketplace Features**
   - GET /api/marketplace/info
   - GET /api/usage
   - GET /api/marketplace/analytics/usage

5. **Monetization**
   - GET /api/marketplace/monetization/plans
   - GET /api/marketplace/monetization/billing
   - GET /api/marketplace/payments/methods
   - GET /api/marketplace/subscriptions/current

### 3. Configure Authentication

1. In the "Security" tab, select "RapidAPI Auth" as the default authentication method
2. This will automatically set up the X-RapidAPI-Key and X-RapidAPI-Host headers

### 4. Configure Pricing Plans

Create the following pricing plans:

#### Basic (Free)
- **Price**: $0/month
- **Features**:
  - Basic content operations
  - Limited quota (100 requests per day)
  - No AI analytics features
- **Rate Limits**: 10 requests per minute
- **Daily Quota**: 100 requests

#### Pro ($25/month)
- **Price**: $25/month
- **Features**:
  - Full content management operations
  - 5,000 requests per month
  - Basic AI analytics (sentiment analysis only)
  - Standard rate limits
- **Rate Limits**: 30 requests per minute
- **Monthly Quota**: 5,000 requests
- **Overage**: $0.005 per additional request

#### Ultra ($75/month)
- **Price**: $75/month
- **Features**:
  - All content management operations
  - 20,000 requests per month
  - Full AI analytics capabilities
  - Higher rate limits
  - Priority support
- **Rate Limits**: 60 requests per minute
- **Monthly Quota**: 20,000 requests
- **Overage**: $0.003 per additional request

#### Mega ($150/month)
- **Price**: $150/month
- **Features**:
  - All features with no restrictions
  - 100,000 requests per month
  - Highest rate limits
  - Premium support
  - Custom analytics reports
- **Rate Limits**: 120 requests per minute
- **Monthly Quota**: 100,000 requests
- **Overage**: $0.001 per additional request

### 5. Configure API Gateway

1. In the "Gateway" tab, enter your API's base URL
2. Configure CORS settings to allow requests from RapidAPI domains
3. Set up health check endpoints to monitor API availability

### 6. Configure Documentation

1. In the "Docs" tab, upload your OpenAPI/Swagger definition
2. Add detailed descriptions for each endpoint
3. Include request/response examples
4. Add code samples for popular programming languages

### 7. Configure Billing

1. In the "Monetize" tab, connect your payment account
2. Configure payout methods
3. Set up tax information

### 8. Test Integration

Before publishing, test your API integration:

1. Use the RapidAPI testing console to verify each endpoint
2. Test authentication and authorization
3. Verify rate limiting and quota enforcement
4. Test subscription plan features and restrictions

### 9. Publish API

Once testing is complete:

1. Submit your API for review
2. Once approved, your API will be listed on the RapidAPI Marketplace
3. Monitor analytics and subscription data from your dashboard

## Implementation Notes

### Handling RapidAPI Headers

The ContentMaster API is configured to validate the following headers on all requests:

```
X-RapidAPI-Key: {api-key}
X-RapidAPI-Host: contentmaster.p.rapidapi.com
```

The RapidApiMiddleware in the API handles authentication and validation of these headers.

### Rate Limiting

Rate limiting is implemented based on the subscription tier:

- Basic: 10 requests per minute
- Pro: 30 requests per minute
- Ultra: 60 requests per minute
- Mega: 120 requests per minute

### Feature Access Control

Feature access is controlled based on the subscription tier:

- Basic tier can only access content endpoints (no analytics)
- Pro tier can access content endpoints and basic analytics (sentiment only)
- Ultra and Mega tiers have access to all endpoints

### Usage Tracking

The UsageTrackingService tracks API usage and enforces quota limits:

- Basic: 100 requests per day
- Pro: 5,000 requests per month
- Ultra: 20,000 requests per month
- Mega: 100,000 requests per month

## Troubleshooting

### Common Issues

1. **Authentication Errors**
   - Verify that X-RapidAPI-Key and X-RapidAPI-Host headers are correctly set
   - Check that the API key is valid and active

2. **Rate Limiting Issues**
   - Check the current rate limit for the subscription tier
   - Implement exponential backoff for retry logic

3. **Feature Access Denied**
   - Verify that the subscription tier has access to the requested feature
   - Check endpoint restrictions for the current tier

### Support Contacts

For integration support, contact:
- Email: support@contentmasterapi.com
- Documentation: https://contentmasterapi.com/docs
- RapidAPI Support: https://rapidapi.com/support

## Maintenance

### Updating API Endpoints

When adding or modifying API endpoints:

1. Update the API definition in RapidAPI dashboard
2. Update documentation and examples
3. Test new endpoints through the RapidAPI console

### Updating Pricing Plans

When modifying pricing plans:

1. Create new plans in the RapidAPI dashboard
2. Set up grandfathering for existing subscribers if needed
3. Update documentation to reflect new pricing

### Monitoring Performance

Regularly monitor:

1. API usage and performance metrics
2. Subscription growth and churn
3. Revenue and billing data
4. Error rates and availability
