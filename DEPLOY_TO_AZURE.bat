@echo off
echo 🌐 ContentMasterAPI - AZURE DEPLOYMENT SCRIPT
echo =============================================
echo.

echo This script will deploy your ContentMasterAPI to Azure App Service
echo Prerequisites:
echo - Azure CLI installed and logged in (az login)
echo - Production setup completed
echo.

set /p CONTINUE="Ready to deploy to Azure? (y/n): "
if /i "%CONTINUE%" neq "y" goto :end

REM Get deployment parameters
set /p RESOURCE_GROUP="Enter Resource Group name (e.g., ContentMasterAPI-RG): "
set /p APP_NAME="Enter App Service name (e.g., contentmasterapi-prod): "
set /p LOCATION="Enter Azure region (e.g., East US): "
set /p SQL_SERVER="Enter SQL Server name (e.g., contentmaster-sql): "
set /p SQL_PASSWORD="Enter SQL admin password: "
set /p OPENAI_KEY="Enter your OpenAI API key: "

echo.
echo 🚀 Starting Azure deployment...
echo.

REM Create Resource Group
echo 📁 Creating resource group...
az group create --name %RESOURCE_GROUP% --location "%LOCATION%"

REM Create App Service Plan
echo 🏗️ Creating App Service Plan...
az appservice plan create --name %APP_NAME%-plan --resource-group %RESOURCE_GROUP% --sku B1 --is-linux

REM Create Web App
echo 🌐 Creating Web App...
az webapp create --resource-group %RESOURCE_GROUP% --plan %APP_NAME%-plan --name %APP_NAME% --runtime "DOTNETCORE|8.0"

REM Create SQL Server
echo 🗄️ Creating SQL Server...
az sql server create --name %SQL_SERVER% --resource-group %RESOURCE_GROUP% --location "%LOCATION%" --admin-user sqladmin --admin-password "%SQL_PASSWORD%"

REM Create SQL Database
echo 🗄️ Creating SQL Database...
az sql db create --resource-group %RESOURCE_GROUP% --server %SQL_SERVER% --name ContentMasterAPI --service-objective Basic

REM Configure firewall rule for Azure services
echo 🔥 Configuring firewall...
az sql server firewall-rule create --resource-group %RESOURCE_GROUP% --server %SQL_SERVER% --name AllowAzureServices --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0

REM Get SQL connection string
echo 📋 Getting SQL connection string...
for /f "delims=" %%i in ('az sql db show-connection-string --server %SQL_SERVER% --name ContentMasterAPI --client ado.net --auth-type SqlPassword') do set SQL_CONNECTION=%%i
set SQL_CONNECTION=%SQL_CONNECTION:<username>=sqladmin%
set SQL_CONNECTION=%SQL_CONNECTION:<password>=%SQL_PASSWORD%%

REM Configure App Settings
echo ⚙️ Configuring application settings...
az webapp config appsettings set --resource-group %RESOURCE_GROUP% --name %APP_NAME% --settings ^
    "ConnectionStrings__DefaultConnection=%SQL_CONNECTION%" ^
    "OpenAI__ApiKey=%OPENAI_KEY%" ^
    "Jwt__Key=ContentMasterAPI-Production-JWT-Key-Azure-12345678901234567890-SecureKey-2025" ^
    "ASPNETCORE_ENVIRONMENT=Production"

REM Build and publish
echo 🔨 Building and publishing application...
cd ContentMasterAPI.API
dotnet publish -c Release -o ./publish

REM Deploy to Azure
echo 🚀 Deploying to Azure...
az webapp deployment source config-zip --resource-group %RESOURCE_GROUP% --name %APP_NAME% --src ./publish.zip

echo.
echo 🎉 DEPLOYMENT COMPLETE!
echo ======================
echo.
echo 🌐 Your API is now live at: https://%APP_NAME%.azurewebsites.net
echo 📖 API Documentation: https://%APP_NAME%.azurewebsites.net/docs
echo 🔍 Health Check: https://%APP_NAME%.azurewebsites.net/health
echo 🎯 GraphQL Playground: https://%APP_NAME%.azurewebsites.net/graphql
echo.
echo 💰 Next: Set up RapidAPI marketplace listing!
echo.

:end
pause
