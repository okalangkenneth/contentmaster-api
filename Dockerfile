# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file and project files
COPY *.sln ./
COPY ContentMasterAPI.API/*.csproj ContentMasterAPI.API/
COPY ContentMasterAPI.Core/*.csproj ContentMasterAPI.Core/
COPY ContentMasterAPI.Infrastructure/*.csproj ContentMasterAPI.Infrastructure/
COPY ContentMasterAPI.Examples/*.csproj ContentMasterAPI.Examples/
COPY ContentMasterAPI.Tests/*.csproj ContentMasterAPI.Tests/

# Restore NuGet packages
RUN dotnet restore

# Copy all source files
COPY . .

# Build and publish the application
RUN dotnet publish "ContentMasterAPI.API/ContentMasterAPI.API.csproj" -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV PORT=8080

# Expose the port
EXPOSE 8080

# Verify files exist (useful for debugging)
RUN ls -la

# Start the application
ENTRYPOINT ["dotnet", "ContentMasterAPI.API.dll"]]