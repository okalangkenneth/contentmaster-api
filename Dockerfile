FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj files and restore dependencies
COPY *.sln .
COPY ContentMasterAPI.API/*.csproj ./ContentMasterAPI.API/
COPY ContentMasterAPI.Core/*.csproj ./ContentMasterAPI.Core/
COPY ContentMasterAPI.Infrastructure/*.csproj ./ContentMasterAPI.Infrastructure/
RUN dotnet restore

# Copy the project files and build
COPY . .
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Make port 8080 available
EXPOSE 8080

# Use the PORT environment variable
ENV PORT=8080

# Start the application
ENTRYPOINT ["dotnet", "ContentMasterAPI.API.dll"]