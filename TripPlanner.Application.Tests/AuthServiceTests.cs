using FluentAssertions;
using Moq;
using TripPlanner.Application.DTOs.Request;
using TripPlanner.Application.Exceptions;
using TripPlanner.Application.Interfaces.Repositories;
using TripPlanner.Application.Interfaces.Services;
using TripPlanner.Application.Interfaces.Utilities;
using TripPlanner.Application.Services;
using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<IJwtTokenService> _jwtTokenService = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private AuthService CreateService() =>
        new(_userRepository.Object, _passwordHasher.Object, _jwtTokenService.Object, _unitOfWork.Object);

    [Fact]
    public async Task RegisterAsync_WhenEmailIsFree_ShouldCreateUserAndReturnToken()
    {
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "Test",
            LastName = "User"
        };

        User? addedUser = null;

        _userRepository.Setup(x => x.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        _passwordHasher.Setup(x => x.Hash(request.Password)).Returns("hashed-password");
        _jwtTokenService.Setup(x => x.GenerateToken(It.IsAny<User>())).Returns("jwt-token");

        _userRepository.Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((user, _) => addedUser = user)
            .Returns(Task.CompletedTask);

        _unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await CreateService().RegisterAsync(request, CancellationToken.None);

        result.AccessToken.Should().Be("jwt-token");
        result.Email.Should().Be(request.Email);
        addedUser.Should().NotBeNull();
        addedUser!.PasswordHash.Should().Be("hashed-password");

        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailExists_ShouldThrowBadRequestException()
    {
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "Test",
            LastName = "User"
        };

        _userRepository.Setup(x => x.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Email = request.Email, FirstName = "Existing", LastName = "User", Trips = new List<Trip>() });

        var act = async () => await CreateService().RegisterAsync(request, CancellationToken.None);

        await act.Should().ThrowAsync<BadRequestException>();
        _userRepository.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WhenCredentialsAreValid_ShouldReturnToken()
    {
        var request = new LoginRequest { Email = "test@example.com", Password = "Password123!" };
        var user = new User
        {
            Email = request.Email,
            PasswordHash = "hashed-password",
            FirstName = "Test",
            LastName = "User",
            Trips = new List<Trip>()
        };

        _userRepository.Setup(x => x.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasher.Setup(x => x.Verify(request.Password, user.PasswordHash)).Returns(true);
        _jwtTokenService.Setup(x => x.GenerateToken(user)).Returns("jwt-token");

        var result = await CreateService().LoginAsync(request, CancellationToken.None);

        result.AccessToken.Should().Be("jwt-token");
        result.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsInvalid_ShouldThrowBadRequestException()
    {
        var request = new LoginRequest { Email = "test@example.com", Password = "wrong" };
        var user = new User
        {
            Email = request.Email,
            PasswordHash = "hashed-password",
            FirstName = "Test",
            LastName = "User",
            Trips = new List<Trip>()
        };

        _userRepository.Setup(x => x.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasher.Setup(x => x.Verify(request.Password, user.PasswordHash)).Returns(false);

        var act = async () => await CreateService().LoginAsync(request, CancellationToken.None);

        await act.Should().ThrowAsync<BadRequestException>();
    }
}