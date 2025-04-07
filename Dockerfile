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

# Publish as self-contained application
RUN dotnet publish "ContentMasterAPI.API/ContentMasterAPI.API.csproj" -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080

# Expose the port
EXPOSE 8080

# Create a better startup script
RUN echo '#!/bin/bash\nls -la\nif [ -f "ContentMasterAPI.API.dll" ]; then\n  dotnet ContentMasterAPI.API.dll\nelif [ -f "ContentMasterAPI.API" ]; then\n  ./ContentMasterAPI.API\nelse\n  echo "Neither DLL nor executable found!"\n  ls -la\nfi' > start.sh && \
    chmod +x start.sh

# Start using the script
ENTRYPOINT ["./start.sh"]