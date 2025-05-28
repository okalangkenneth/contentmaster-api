using ContentMasterAPI.API.GraphQL.Queries;
using ContentMasterAPI.API.GraphQL.Types;
using ContentMasterAPI.API.Middleware;
using ContentMasterAPI.Core.Interfaces;
using ContentMasterAPI.Infrastructure.Services;
using ContentMasterAPI.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text;
using Path = System.IO.Path; 

namespace ContentMasterAPI.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Configure Services FIRST
            ConfigureServices(builder.Services, builder.Configuration);

            // 2. Build Application
            var app = builder.Build();

            // 3. Initialize Database
            InitializeDatabase(app);

            // 4. Configure Middleware Pipeline
            ConfigureApp(app, app.Environment);

            // 5. Configure PORT and START APPLICATION
            var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
            app.Run($"http://*:{port}");
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();

            // 🔥 PRODUCTION DATABASE - Entity Framework
            services.AddDbContext<ContentMasterDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // 🔥 PRODUCTION AI SERVICE - OpenAI Integration
            services.AddScoped<IContentAnalysisService, OpenAiContentAnalysisService>();

            // 🔥 PRODUCTION REPOSITORY - Entity Framework
            services.AddScoped<IContentRepository, EfContentRepository>();

            // Usage Tracking Service
            services.AddSingleton<IUsageTrackingService, UsageTrackingService>();

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
                    Version = "1.0.0",
                    Description = "A modern content management API with AI-driven capabilities, GraphQL support, and robust security",
                    Contact = new OpenApiContact
                    {
                        Name = "ContentMaster API Team",
                        Email = "support@contentmasterapi.com",
                        Url = new Uri("https://contentmasterapi.com")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Commercial License",
                        Url = new Uri("https://contentmasterapi.com/license")
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
                    Description = "RapidAPI Key Authentication. Required for all API calls through RapidAPI marketplace.",
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

            // CORS Configuration for RapidAPI
            services.AddCors(options =>
            {
                options.AddPolicy("RapidAPIPolicy", builder =>
                {
                    builder
                        .WithOrigins(
                            "https://rapidapi.com",
                            "https://*.rapidapi.com",
                            "https://contentmaster.p.rapidapi.com"
                        )
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });

                options.AddPolicy("DevelopmentPolicy", builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            // Health Checks
            services.AddHealthChecks()
                .AddDbContext<ContentMasterDbContext>();

            // Response Compression
            services.AddResponseCompression();
        }

        private static void InitializeDatabase(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ContentMasterDbContext>();
            
            try
            {
                // Create database if it doesn't exist
                context.Database.EnsureCreated();
                
                // Apply any pending migrations
                context.Database.Migrate();
                
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Database initialized successfully");
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while initializing the database");
                throw;
            }
        }

        private static void ConfigureApp(WebApplication app, IWebHostEnvironment env)
        {
            // Exception Handling
            app.UseMiddleware<ErrorHandlingMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ContentMaster API v1.0.0");
                    c.RoutePrefix = string.Empty; // Serve Swagger UI at root
                    c.DocumentTitle = "ContentMaster API Documentation";
                });
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ContentMaster API v1.0.0");
                    c.RoutePrefix = "docs"; // Serve Swagger UI at /docs in production
                });
            }

            // Response Compression
            app.UseResponseCompression();

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRouting();

            // CORS - Use development policy in dev, RapidAPI policy in production
            if (env.IsDevelopment())
            {
                app.UseCors("DevelopmentPolicy");
            }
            else
            {
                app.UseCors("RapidAPIPolicy");
            }

            // Authentication & Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Conditional RapidAPI Middleware (skip for auth endpoints and health checks)
            app.UseWhen(context => 
                !context.Request.Path.StartsWithSegments("/api/auth") &&
                !context.Request.Path.StartsWithSegments("/health") &&
                !context.Request.Path.StartsWithSegments("/swagger") &&
                !context.Request.Path.StartsWithSegments("/docs"),
                appBuilder => {
                    appBuilder.UseRapidApiAuthentication();
                });

            // Endpoint Configuration
            app.MapControllers();
            app.MapGraphQL();
            app.MapHealthChecks("/health");

            // Root endpoint
            app.MapGet("/", () => new
            {
                name = "ContentMaster API",
                version = "1.0.0",
                description = "A modern content management API with AI-driven capabilities",
                documentation = "/docs",
                health = "/health",
                graphql = "/graphql"
            });
        }
    }
}
