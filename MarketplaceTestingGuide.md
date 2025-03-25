# ContentMaster API - Marketplace Testing Guide

This guide provides instructions for testing the ContentMaster API's marketplace integration and commercial features before deployment to RapidAPI.

## Prerequisites

Before testing, ensure you have:

1. The ContentMaster API running locally
2. Test API keys for different subscription tiers
3. Postman or another API testing tool

## Test Cases

### 1. Authentication Testing

#### 1.1 Valid Authentication

**Test:** Send a request with valid RapidAPI headers
```
X-RapidAPI-Key: demo-pro-api-key
X-RapidAPI-Host: contentmaster.p.rapidapi.com
```

**Expected Result:** Request succeeds with 200 OK response

#### 1.2 Missing API Key

**Test:** Send a request without X-RapidAPI-Key header

**Expected Result:** Request fails with 401 Unauthorized response

#### 1.3 Missing Host Header

**Test:** Send a request without X-RapidAPI-Host header

**Expected Result:** Request fails with 401 Unauthorized response

#### 1.4 Invalid API Key

**Test:** Send a request with invalid X-RapidAPI-Key header

**Expected Result:** Request fails with 401 Unauthorized response

### 2. Subscription Tier Testing

#### 2.1 Basic Tier Access

**Test:** Use a Basic tier API key to access content endpoints
```
X-RapidAPI-Key: demo-basic-api-key
```

**Expected Result:** Content endpoints accessible, analytics endpoints restricted

#### 2.2 Pro Tier Access

**Test:** Use a Pro tier API key to access sentiment analysis
```
X-RapidAPI-Key: demo-pro-api-key
```

**Expected Result:** Content and sentiment analysis endpoints accessible, other analytics endpoints restricted

#### 2.3 Ultra Tier Access

**Test:** Use an Ultra tier API key to access all analytics endpoints
```
X-RapidAPI-Key: demo-ultra-api-key
```

**Expected Result:** All endpoints accessible

### 3. Rate Limiting Testing

#### 3.1 Basic Tier Rate Limits

**Test:** Send 15 requests in one minute using a Basic tier API key

**Expected Result:** First 10 requests succeed, remaining requests fail with 429 Too Many Requests

#### 3.2 Pro Tier Rate Limits

**Test:** Send 35 requests in one minute using a Pro tier API key

**Expected Result:** First 30 requests succeed, remaining requests fail with 429 Too Many Requests

### 4. Usage Quota Testing

#### 4.1 Basic Tier Daily Quota

**Test:** Send 110 requests in one day using a Basic tier API key

**Expected Result:** First 100 requests succeed, remaining requests fail with quota exceeded error

#### 4.2 Usage Tracking

**Test:** Send multiple requests and check usage statistics
```
GET /api/usage
```

**Expected Result:** Usage statistics show correct request count and remaining quota

### 5. Marketplace Features Testing

#### 5.1 API Information

**Test:** Get API marketplace information
```
GET /api/marketplace/info
```

**Expected Result:** Complete API information returned with pricing tiers and features

#### 5.2 Subscription Plans

**Test:** Get subscription plans
```
GET /api/marketplace/monetization/plans
```

**Expected Result:** All subscription plans returned with correct pricing and features

#### 5.3 Billing Information

**Test:** Get billing information for current API key
```
GET /api/marketplace/monetization/billing
```

**Expected Result:** Billing information returned with correct subscription details

#### 5.4 Payment Methods

**Test:** Get payment methods for current API key
```
GET /api/marketplace/payments/methods
```

**Expected Result:** Payment methods returned with masked card details

### 6. Analytics Testing

#### 6.1 Usage Analytics

**Test:** Get usage analytics
```
GET /api/marketplace/analytics/usage
```

**Expected Result:** Usage analytics returned with request counts and user statistics

#### 6.2 Performance Analytics

**Test:** Get performance analytics
```
GET /api/marketplace/analytics/performance
```

**Expected Result:** Performance analytics returned with response times and error rates

#### 6.3 Revenue Analytics

**Test:** Get revenue analytics
```
GET /api/marketplace/analytics/revenue
```

**Expected Result:** Revenue analytics returned with subscription and overage revenue

### 7. Developer Portal Testing

#### 7.1 API Documentation

**Test:** Get API documentation
```
GET /api/marketplace/developer/documentation
```

**Expected Result:** Complete API documentation returned with endpoints and examples

#### 7.2 Code Samples

**Test:** Get code samples
```
GET /api/marketplace/developer/code-samples
```

**Expected Result:** Code samples returned for multiple programming languages

## Test Automation

For automated testing, use the following script structure:

```csharp
// Authentication tests
[Test]
public async Task ValidAuthentication_ShouldSucceed()
{
    var client = new HttpClient();
    client.DefaultRequestHeaders.Add("X-RapidAPI-Key", "demo-pro-api-key");
    client.DefaultRequestHeaders.Add("X-RapidAPI-Host", "contentmaster.p.rapidapi.com");
    
    var response = await client.GetAsync("https://localhost:7001/api/content");
    
    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
}

[Test]
public async Task MissingApiKey_ShouldFail()
{
    var client = new HttpClient();
    client.DefaultRequestHeaders.Add("X-RapidAPI-Host", "contentmaster.p.rapidapi.com");
    
    var response = await client.GetAsync("https://localhost:7001/api/content");
    
    Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
}

// Subscription tier tests
[Test]
public async Task BasicTier_ShouldNotAccessAnalytics()
{
    var client = new HttpClient();
    client.DefaultRequestHeaders.Add("X-RapidAPI-Key", "demo-basic-api-key");
    client.DefaultRequestHeaders.Add("X-RapidAPI-Host", "contentmaster.p.rapidapi.com");
    
    var response = await client.GetAsync("https://localhost:7001/api/analytics/123/sentiment");
    
    Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
}
```

## Test Results Reporting

Document test results in the following format:

| Test Case | Status | Notes |
|-----------|--------|-------|
| 1.1 Valid Authentication | ✅ Pass | |
| 1.2 Missing API Key | ✅ Pass | |
| 1.3 Missing Host Header | ✅ Pass | |
| 1.4 Invalid API Key | ✅ Pass | |
| 2.1 Basic Tier Access | ✅ Pass | |
| 2.2 Pro Tier Access | ✅ Pass | |
| 2.3 Ultra Tier Access | ✅ Pass | |
| 3.1 Basic Tier Rate Limits | ✅ Pass | |
| 3.2 Pro Tier Rate Limits | ✅ Pass | |
| 4.1 Basic Tier Daily Quota | ✅ Pass | |
| 4.2 Usage Tracking | ✅ Pass | |
| 5.1 API Information | ✅ Pass | |
| 5.2 Subscription Plans | ✅ Pass | |
| 5.3 Billing Information | ✅ Pass | |
| 5.4 Payment Methods | ✅ Pass | |
| 6.1 Usage Analytics | ✅ Pass | |
| 6.2 Performance Analytics | ✅ Pass | |
| 6.3 Revenue Analytics | ✅ Pass | |
| 7.1 API Documentation | ✅ Pass | |
| 7.2 Code Samples | ✅ Pass | |

## Troubleshooting Common Issues

### Authentication Issues
- Verify header names and values are correct
- Check that API keys are properly configured in the system
- Ensure the RapidApiMiddleware is properly registered in the pipeline

### Rate Limiting Issues
- Check rate limit configuration in UsageTrackingService
- Verify that rate limits are properly enforced by middleware
- Test with different API keys to ensure tier-specific limits are applied

### Feature Access Issues
- Verify subscription tier configuration
- Check endpoint restrictions in RapidApiMiddleware
- Ensure feature flags are properly set for each tier

## Next Steps After Testing

1. Fix any issues identified during testing
2. Update documentation if necessary
3. Prepare for deployment to RapidAPI Marketplace
4. Set up monitoring for production environment
