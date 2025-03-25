# ContentMaster API

A modern content management API built with C# and .NET 8, featuring AI-driven capabilities, GraphQL support, and robust security.

## Key Features

- **Content Management**: Full CRUD operations for content items
- **AI-Driven Capabilities**: Sentiment analysis, auto-tagging, content categorization, and summarization
- **GraphQL Support**: Flexible querying capabilities
- **Security**: JWT authentication and authorization
- **Error Handling**: Consistent error responses across the API
- **Swagger Documentation**: Interactive API documentation

## Architecture

ContentMaster API follows clean architecture principles with three main projects:

- **ContentMasterAPI.API**: The presentation layer containing controllers and middleware
- **ContentMasterAPI.Core**: The domain layer containing models and interfaces
- **ContentMasterAPI.Infrastructure**: The data access layer containing implementations

## Getting Started

### Prerequisites

- .NET 8 SDK
- Visual Studio 2022 or Visual Studio Code

### Installation

1. Clone the repository
2. Navigate to the project directory
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

## Documentation

For detailed documentation, see [Documentation.md](Documentation.md).

## Examples

Example code demonstrating how to use the API is available in the ContentMasterAPI.Examples project.

## License

This project is licensed under the MIT License - see the LICENSE file for details.
