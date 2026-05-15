using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TripPlanner.Application.Interfaces;
using TripPlanner.Application.Interfaces.Background;
using TripPlanner.Application.Services;
using TripPlanner.Infrastructure.Background;
using TripPlanner.Infrastructure.Configurations;
using TripPlanner.Infrastructure.Persistence;
using TripPlanner.Infrastructure.Repositories;
using TripPlanner.Infrastructure.Services;
using TripPlanner.Infrastructure.Services.Google;
using TripPlanner.Infrastructure.Services.LocalLlm;
using TripPlanner.Infrastructure.Services.Unsplash;

namespace TripPlanner.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")),
                optionsLifetime: ServiceLifetime.Singleton);

            services.AddDbContextFactory<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>()!;

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.MapInboundClaims = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings.Key)),

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddSingleton(jwtSettings);
            services.AddTransient<IJwtTokenService, JwtTokenService>();

            services.AddScoped<IPasswordHasher, PasswordHasher>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IPlaceSearchService, PlaceSearchService>();

            services.AddScoped<ITripPlaceService, TripPlaceService>();
            services.AddScoped<IPlaceProviderRequestLogRepository, PlaceProviderRequestLogRepository>();

            // TODO: Mocking up the Google Places API during development, will replace with real API calls later
            services.AddHttpClient<TestPlaceProvider>(client =>
            {
                client.BaseAddress = new Uri("https://places.googleapis.com/");
                var googlePlacesKey = configuration["GooglePlaces:ApiKey"];
                client.DefaultRequestHeaders.Add("X-Goog-Api-Key", googlePlacesKey);
            });
            services.AddScoped<IPlaceProvider>(sp =>
            {
                var googleProvider = sp.GetRequiredService<TestPlaceProvider>();
                var logger = sp.GetRequiredService<IPlaceProviderRequestLogRepository>();
                var currentUserService = sp.GetRequiredService<ICurrentUserService>();

                return new LoggingGooglePlaceProviderDecorator(googleProvider, logger, currentUserService);
            });
            services.AddScoped<ITripPlaceRepository, TripPlaceRepository>();

            services.AddScoped<IPlaceRepository, PlaceRepository>();

            services.AddHostedService<PlaceDetailsBackgroundService>();
            services.AddScoped<IPlaceDetailsGenerationProcessor, PlaceDetailsGenerationProcessor>();

            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddHttpClient<IDescriptionProvider, LocalLlmDescriptionProvider>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5001");
            });
            services.AddHttpClient<IPlaceRecommendationsProvider, LocalLlmPlaceRecommendationsProvider>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5001");
            });
            services.AddHttpClient<IImageProvider, UnsplashImageProvider>(client =>
            {
                client.BaseAddress = new Uri("https://api.unsplash.com");
                var key = configuration["UNSPLASH_ACCESS_KEY"];
                client.DefaultRequestHeaders.Add("Authorization", "Client-ID " + key);
            });

            services.AddScoped<ITripService, TripService>();
            services.AddScoped<ITripRepository, TripRepository>();

            services.AddScoped<ITripShareService, TripShareService>();
            services.AddScoped<ITripShareRepository, TripShareRepository>();

            return services;
        }
    }
}
