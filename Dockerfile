# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file and project files
COPY *.sln ./
COPY ContentMasterAPI.API/*.csproj ContentMasterAPI.API/
COPY ContentMasterAPI.Core/*.csproj ContentMasterAPI.Core/
COPY ContentMasterAPI.Infrastructure/*.csproj ContentMasterAPI.Infrastructure/

# Restore NuGet packages
RUN dotnet restore

# Copy all source files
COPY . .

# Publish as self-contained application for Linux
RUN dotnet publish "ContentMasterAPI.API/ContentMasterAPI.API.csproj" \
    -c Release \
    -o /app/publish \
    --self-contained true \
    --runtime linux-x64 \
    /p:PublishTrimmed=false \
    /p:PublishSingleFile=false

# Final stage - use a minimal base image
FROM mcr.microsoft.com/dotnet/runtime-deps:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080

# Expose the port
EXPOSE 8080

# Verify files exist
RUN ls -la

# Start the application
ENTRYPOINT ["./ContentMasterAPI.API"]