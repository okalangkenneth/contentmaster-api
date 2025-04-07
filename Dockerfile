# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["ContentMasterAPI.API/ContentMasterAPI.API.csproj", "ContentMasterAPI.API/"]
COPY ["ContentMasterAPI.Core/ContentMasterAPI.Core.csproj", "ContentMasterAPI.Core/"]
COPY ["ContentMasterAPI.Infrastructure/ContentMasterAPI.Infrastructure.csproj", "ContentMasterAPI.Infrastructure/"]
RUN dotnet restore "ContentMasterAPI.API/ContentMasterAPI.API.csproj"

# Copy all source code and build the application
COPY . .
WORKDIR "/src/ContentMasterAPI.API"
RUN dotnet build "ContentMasterAPI.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ContentMasterAPI.API.csproj" -c Release -o /app/publish

# Final stage - use the ASP.NET runtime image which is smaller
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080

# Expose the port
EXPOSE 8080

# Start the application
ENTRYPOINT ["dotnet", "ContentMasterAPI.API.dll"]