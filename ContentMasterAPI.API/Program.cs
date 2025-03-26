using ContentMasterAPI.API.GraphQL.Queries;
using ContentMasterAPI.API.GraphQL.Types;
using ContentMasterAPI.API.Middleware;
using ContentMasterAPI.Core.Interfaces;
using ContentMasterAPI.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using HotChocolate.AspNetCore;




namespace ContentMasterAPI.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add this code to use the PORT environment variable
            var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
            builder.WebHost.UseUrls($"http://0.0.0.0:{port}");


            // Add services to the container
            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline
            ConfigureApp(app, app.Environment);

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Add controllers
            services.AddControllers();

            // Add this line with your other service registrations:
            services.AddScoped<IContentAnalysisService, ContentAnalysisService>();

            

            // Add HotChocolate GraphQL services
            services
                .AddGraphQLServer()
                .AddQueryType(d => d.Name("Query"))
                .AddTypeExtension<ContentQueries>()
                .AddTypeExtension<AnalyticsQueries>()
                .AddType<ContentType>();




            // Configure JWT Authentication
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

            // Add Swagger
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

                // Add JWT Authentication to Swagger
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

                // Add RapidAPI key authentication to Swagger
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

            // Register services
            services.AddSingleton<IContentRepository, InMemoryContentRepository>();
            services.AddSingleton<IUsageTrackingService, UsageTrackingService>();

            // Add CORS for RapidAPI
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
            app.UseStaticFiles();



            // Configure the HTTP request pipeline
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            
            // Enable Swagger
            app.UseSwagger(c =>
            {
                app.UseSwagger(c =>
                {
                    c.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0; // This forces Swagger to use the 2.0 spec

                });                                                                    
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ContentMaster API v1");
            });

            // Use HTTPS redirection
            app.UseHttpsRedirection();

            // Use CORS
            app.UseCors("RapidAPIPolicy");

            // Use authentication before RapidAPI middleware
            app.UseAuthentication();

            //// Use RapidAPI authentication middleware
            //app.UseRapidApiAuthentication();

            // To this:
            app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api/auth"),
                appBuilder => {
                    appBuilder.UseRapidApiAuthentication();
                });

            // Use global error handling middleware
            app.UseMiddleware<ErrorHandlingMiddleware>();

            // Use routing and authorization
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            // Map GraphQL endpoint
            app.MapGraphQL();

            // Map controllers
            app.MapControllers();
        }
    }
}

