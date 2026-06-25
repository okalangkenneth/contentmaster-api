using ContentMasterAPI.API.GraphQL.Queries;
using ContentMasterAPI.API.GraphQL.Types;
using ContentMasterAPI.API.Middleware;
using ContentMasterAPI.Core.Interfaces;
using ContentMasterAPI.Infrastructure.Services;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Path = System.IO.Path;

namespace ContentMasterAPI.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Configure Services FIRST
            ConfigureServices(builder.Services, builder.Configuration, builder.Environment);

            // 2. Build Application
            var app = builder.Build();

            // 3. Configure Middleware Pipeline
            ConfigureApp(app, app.Environment);

            // 4. Configure PORT and START APPLICATION
            var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
            app.Run($"http://*:{port}");
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            services.AddControllers();
            services.AddScoped<IContentAnalysisService, ContentAnalysisService>();

            // GraphQL Configuration
            services
                .AddGraphQLServer()
                .AddQueryType(d => d.Name("Query"))
                .AddTypeExtension<ContentQueries>()
                .AddTypeExtension<AnalyticsQueries>()
                .AddType<ContentType>();

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

            // Service Registrations
            services.AddSingleton<IContentRepository, InMemoryContentRepository>();
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
        }

        private static void ConfigureApp(WebApplication app, IWebHostEnvironment env)
        {
            // Exception Handling
            app.UseMiddleware<ErrorHandlingMiddleware>();

            // Always enable Swagger — this is a public API product
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ContentMaster API v1");
                c.RoutePrefix = string.Empty; // Serve Swagger at root /
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("RapidAPIPolicy");

            app.UseAuthorization();

            // RapidAPI key authentication applies to all endpoints
            app.UseRapidApiAuthentication();

            // Endpoint Configuration
            app.MapControllers();
            app.MapGraphQL();
        }
    }
}
