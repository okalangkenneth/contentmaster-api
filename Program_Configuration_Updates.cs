// Program.cs Configuration Updates for Production
// Replace the existing ConfigureServices method with this updated version

private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddControllers();

    // === DATABASE CONFIGURATION ===
    // Replace InMemoryContentRepository with Entity Framework
    services.AddDbContext<ContentMasterDbContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

    // === AI SERVICE CONFIGURATION ===
    // Replace ContentAnalysisService with OpenAI-powered version
    services.AddScoped<IContentAnalysisService, OpenAiContentAnalysisService>();

    // === REPOSITORY CONFIGURATION ===
    // Use Entity Framework repository instead of in-memory
    services.AddScoped<IContentRepository, EfContentRepository>();

    // === EXISTING CONFIGURATIONS (keep these) ===
    
    // GraphQL Configuration
    services
        .AddGraphQLServer()
        .AddQueryType(d => d.Name("Query"))
        .AddTypeExtension<ContentQueries>()
        .AddTypeExtension<AnalyticsQueries>()
        .AddType<ContentType>();

    // JWT Authentication
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? "ContentMasterAPISecretKey1234567890!"))
            };
        });

    // Swagger Configuration
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "ContentMaster API",
            Version = "v1",
            Description = "A modern content management API with AI-driven capabilities",
            Contact = new OpenApiContact
            {
                Name = "ContentMaster API Team",
                Email = "support@contentmasterapi.com",
                Url = new Uri("https://contentmasterapi.com")
            }
        });

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }

        // JWT Security Definition
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new List<string>()
            }
        });

        // RapidAPI Security Definition
        c.AddSecurityDefinition("RapidAPI", new OpenApiSecurityScheme
        {
            Description = "RapidAPI Key Authentication",
            Name = "X-RapidAPI-Key",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "apiKey"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "RapidAPI"
                    }
                },
                new List<string>()
            }
        });
    });

    // Usage Tracking Service
    services.AddSingleton<IUsageTrackingService, UsageTrackingService>();

    // CORS Configuration
    services.AddCors(options =>
    {
        options.AddPolicy("RapidAPIPolicy", builder =>
        {
            builder.WithOrigins("https://*.rapidapi.com")
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
    });

    // === PRODUCTION LOGGING (Optional) ===
    services.AddLogging(builder =>
    {
        builder.AddConsole();
        builder.AddFile("logs/contentmaster-{Date}.txt"); // Requires Serilog.Extensions.Logging.File
    });
}

// Add this method to automatically run database migrations on startup
private static void EnsureDatabaseCreated(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ContentMasterDbContext>();
    
    // This will create the database if it doesn't exist and run pending migrations
    context.Database.EnsureCreated();
    
    // For production, use migrations instead:
    // context.Database.Migrate();
}

// Update your Main method to include database initialization:
public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    // 1. Configure Services
    ConfigureServices(builder.Services, builder.Configuration);

    // 2. Build Application
    var app = builder.Build();

    // 3. Ensure Database is Created (add this line)
    EnsureDatabaseCreated(app.Services);

    // 4. Configure Middleware Pipeline
    ConfigureApp(app, app.Environment);

    // 5. Start Application
    var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
    app.Run($"http://*:{port}");
}

// Don't forget to add these using statements at the top of Program.cs:
/*
using ContentMasterAPI.Infrastructure.Data;
using ContentMasterAPI.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
*/
