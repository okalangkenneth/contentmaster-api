using ContentMasterAPI.API.GraphQL.Queries;
using ContentMasterAPI.API.GraphQL.Types;
using ContentMasterAPI.API.Middleware;
using ContentMasterAPI.Core.Interfaces;
using ContentMasterAPI.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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

            // JWT Authentication
            var jwtKey = configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                if (environment.IsDevelopment())
                    throw new InvalidOperationException(
                        "Jwt:Key is not configured. Add it to appsettings.Development.json or user secrets.");
                else
                    throw new InvalidOperationException(
                        "Jwt:Key is not configured. Set the JWT__Key environment variable in production.");
            }

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
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
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

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ContentMaster API v1");
                });
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

            // Authentication & Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Conditional RapidAPI Middleware
            app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api/auth"),
                appBuilder => {
                    appBuilder.UseRapidApiAuthentication();
                });

            // Endpoint Configuration
            app.MapControllers();
            app.MapGraphQL();
        }
    }
}
