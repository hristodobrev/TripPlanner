using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TripPlanner.API.Tests.Utilities;
using TripPlanner.Application.DTOs.Request;
using TripPlanner.Application.DTOs.Response;

namespace TripPlanner.API.Tests;

public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_ShouldReturnToken()
    {
        var response = await _client.PostAsJsonAsync("/api/Auth/register", new RegisterRequest
        {
            Email = $"user-{Guid.NewGuid():N}@example.com",
            Password = "Password123!",
            FirstName = "Test",
            LastName = "User"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        auth!.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnToken()
    {
        var email = $"login-{Guid.NewGuid():N}@example.com";
        await _client.RegisterAndGetTokenAsync(email);

        var response = await _client.PostAsJsonAsync("/api/Auth/login", new LoginRequest
        {
            Email = email,
            Password = "Password123!"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        auth!.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ShouldReturnBadRequest()
    {
        var email = $"bad-login-{Guid.NewGuid():N}@example.com";
        await _client.RegisterAndGetTokenAsync(email);

        var response = await _client.PostAsJsonAsync("/api/Auth/login", new LoginRequest
        {
            Email = email,
            Password = "WrongPassword"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/Trips");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}