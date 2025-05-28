@echo off
echo 🚀 ContentMasterAPI - IMMEDIATE PRODUCTION SETUP
echo ================================================
echo.

REM Step 1: Navigate to the main project directory
echo 📁 Step 1: Navigating to main project...
cd /d "C:\Users\Ken\Documents\ContentMasterAPI"

REM Step 2: Copy production-ready Program.cs
echo 📋 Step 2: Installing production Program.cs...
copy "Program_Production_Ready.cs" "ContentMasterAPI.API\Program.cs" /Y
echo ✅ Production Program.cs installed

REM Step 3: Copy production-ready appsettings.json
echo 📋 Step 3: Installing production appsettings.json...
copy "appsettings_ready_to_use.json" "ContentMasterAPI.API\appsettings.json" /Y
echo ✅ Production appsettings.json installed

REM Step 4: Navigate to API project and add required packages
echo 📦 Step 4: Adding production packages...
cd ContentMasterAPI.API

REM Add Entity Framework packages
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0

REM Add OpenAI package for real AI integration
dotnet add package OpenAI --version 1.11.0

REM Add health checks
dotnet add package Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore --version 8.0.0

REM Add response compression
dotnet add package Microsoft.AspNetCore.ResponseCompression --version 8.0.0

echo ✅ All packages added successfully!

REM Step 5: Create initial migration
echo 🗄️ Step 5: Creating database migration...
dotnet ef migrations add InitialCreate --context ContentMasterDbContext
echo ✅ Database migration created

REM Step 6: Build the project
echo 🔨 Step 6: Building production project...
dotnet build --configuration Release
echo ✅ Project built successfully

echo.
echo 🎉 PRODUCTION SETUP COMPLETE!
echo ================================
echo.
echo ⚠️  IMPORTANT NEXT STEPS:
echo 1. Add your OpenAI API key to appsettings.json
echo 2. Test locally: dotnet run
echo 3. Deploy to Azure/cloud platform
echo 4. Set up RapidAPI marketplace listing
echo.
echo 💰 Your API is now ready to earn money!
echo.
echo 📖 Next: Follow the RapidAPI Integration Guide
echo.

pause
