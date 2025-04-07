# Build stage - use a specific SDK version to match your global.json
FROM mcr.microsoft.com/dotnet/sdk:8.0.100 AS build
WORKDIR /src

# Copy solution and project files first
COPY *.sln ./
COPY ContentMasterAPI.API/*.csproj ContentMasterAPI.API/
COPY ContentMasterAPI.Core/*.csproj ContentMasterAPI.Core/
COPY ContentMasterAPI.Infrastructure/*.csproj ContentMasterAPI.Infrastructure/

# Handle other projects if they exist
COPY ContentMasterAPI.Examples/*.csproj ContentMasterAPI.Examples/ 2>/dev/null || true
COPY ContentMasterAPI.Tests/*.csproj ContentMasterAPI.Tests/ 2>/dev/null || true

# Restore packages
RUN dotnet restore

# Copy everything else
COPY . .

# Publish directly (skipping build step)
RUN dotnet publish "ContentMasterAPI.API/ContentMasterAPI.API.csproj" -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published files
COPY --from=build /app/publish .

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080

# Expose the port
EXPOSE 8080

# Debug - list files to make sure the DLL exists
RUN ls -la

# Start the application
ENTRYPOINT ["dotnet", "ContentMasterAPI.API.dll"]