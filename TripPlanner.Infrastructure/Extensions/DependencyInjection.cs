using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TripPlanner.Application.Interfaces;
using TripPlanner.Application.Services;
using TripPlanner.Infrastructure.Configurations;
using TripPlanner.Infrastructure.Persistence;
using TripPlanner.Infrastructure.Repositories;
using TripPlanner.Infrastructure.Services;

namespace TripPlanner.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

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
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPlaceAutoCompleteService, PlaceAutoCompleteService>();
            services.AddHttpClient<IPlaceAutoCompleteProvider, GooglePlaceAutoCompleteProvider>(client =>
            {
                client.BaseAddress = new Uri("https://places.googleapis.com/v1/places:autocomplete");
                var googlePlacesKey = configuration["GooglePlaces:ApiKey"];
                client.DefaultRequestHeaders.Add("X-Goog-Api-Key", googlePlacesKey);
                client.DefaultRequestHeaders.Add("X-Goog-FieldMask", "suggestions.placePrediction.placeId,suggestions.placePrediction.text.text,suggestions.placePrediction.structuredFormat.mainText.text");
            });

            return services;
        }
    }
}
