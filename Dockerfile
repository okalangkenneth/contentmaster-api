# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file first to properly restore projects
COPY *.sln ./
COPY ContentMasterAPI.API/*.csproj ./ContentMasterAPI.API/
COPY ContentMasterAPI.Core/*.csproj ./ContentMasterAPI.Core/
COPY ContentMasterAPI.Infrastructure/*.csproj ./ContentMasterAPI.Infrastructure/
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o /app/publish ./ContentMasterAPI.API/ContentMasterAPI.API.csproj

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV PORT=8080

# Expose the port
EXPOSE 8080

# Verify files exist
RUN ls -la

# Set the entry point
ENTRYPOINT ["dotnet", "ContentMasterAPI.API.dll"]