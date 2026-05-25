using FluentAssertions;
using Moq;
using TripPlanner.Application.DTOs.Request;
using TripPlanner.Application.Exceptions;
using TripPlanner.Application.Interfaces.Repositories;
using TripPlanner.Application.Interfaces.Utilities;
using TripPlanner.Application.Services;
using TripPlanner.Domain.Entities;
using TripPlanner.Domain.Enums;

namespace TripPlanner.Application.Tests.Services;

public class TripShareServiceTests
{
    private readonly Mock<ITripShareRepository> _tripShareRepository = new();
    private readonly Mock<ITripRepository> _tripRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    public TripShareServiceTests()
    {
        _unitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
    }

    private TripShareService CreateService()
    {
        return new TripShareService(
            _tripShareRepository.Object,
            _tripRepository.Object,
            _unitOfWork.Object);
    }

    [Fact]
    public async Task AddAsync_WhenUserIsOwner_ShouldAddTripShareAndSaveChanges()
    {
        var ownerId = Guid.NewGuid();
        var sharedUserId = Guid.NewGuid();
        var tripId = Guid.NewGuid();

        var trip = CreateTrip(tripId, ownerId);

        var request = new AddTripShareRequest
        {
            UserId = sharedUserId,
            Permission = TripPermission.Edit
        };

        TripShare? addedShare = null;

        _tripRepository
            .Setup(x => x.GetByIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trip);

        _tripShareRepository
            .Setup(x => x.AddAsync(It.IsAny<TripShare>(), It.IsAny<CancellationToken>()))
            .Callback<TripShare, CancellationToken>((share, _) => addedShare = share)
            .Returns(Task.CompletedTask);

        var service = CreateService();

        await service.AddAsync(tripId, request, ownerId, CancellationToken.None);

        addedShare.Should().NotBeNull();
        addedShare!.TripId.Should().Be(tripId);
        addedShare.UserId.Should().Be(sharedUserId);
        addedShare.Permission.Should().Be(TripPermission.Edit);

        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_WhenTripDoesNotExist_ShouldThrowNotFoundException()
    {
        var tripId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var request = new AddTripShareRequest
        {
            UserId = Guid.NewGuid(),
            Permission = TripPermission.ReadOnly
        };

        _tripRepository
            .Setup(x => x.GetByIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Trip?)null);

        var service = CreateService();

        var act = async () => await service.AddAsync(tripId, request, userId, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task AddAsync_WhenUserIsNotOwner_ShouldThrowForbiddenException()
    {
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var tripId = Guid.NewGuid();

        var trip = CreateTrip(tripId, ownerId);

        var request = new AddTripShareRequest
        {
            UserId = Guid.NewGuid(),
            Permission = TripPermission.ReadOnly
        };

        _tripRepository
            .Setup(x => x.GetByIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trip);

        var service = CreateService();

        var act = async () => await service.AddAsync(tripId, request, otherUserId, CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();

        _tripShareRepository.Verify(x => x.AddAsync(It.IsAny<TripShare>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetByTripIdAsync_WhenUserIsOwner_ShouldReturnTripShares()
    {
        var ownerId = Guid.NewGuid();
        var sharedUserId = Guid.NewGuid();
        var tripId = Guid.NewGuid();

        var trip = CreateTrip(tripId, ownerId);

        var shares = new List<TripShare>
        {
            new()
            {
                Id = Guid.NewGuid(),
                TripId = tripId,
                UserId = sharedUserId,
                Permission = TripPermission.ReadOnly,
                User = new User
                {
                    Id = sharedUserId,
                    FirstName = "Ivan",
                    LastName = "Ivanov",
                    Email = "ivan@example.com",
                    Trips = new List<Trip>()
                }
            }
        };

        _tripRepository
            .Setup(x => x.GetByIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trip);

        _tripShareRepository
            .Setup(x => x.GetByTripIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shares);

        var service = CreateService();

        var result = (await service.GetByTripIdAsync(tripId, ownerId, CancellationToken.None)).ToList();

        result.Should().HaveCount(1);
        result[0].UserId.Should().Be(sharedUserId);
        result[0].UserFullName.Should().Be("Ivan Ivanov");
        result[0].Permission.Should().Be(TripPermission.ReadOnly);
    }

    [Fact]
    public async Task RemoveAsync_WhenUserIsOwner_ShouldDeleteTripShareAndSaveChanges()
    {
        var ownerId = Guid.NewGuid();
        var tripId = Guid.NewGuid();
        var tripShareId = Guid.NewGuid();

        var trip = CreateTrip(tripId, ownerId);

        _tripRepository
            .Setup(x => x.GetByIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trip);

        _tripShareRepository
            .Setup(x => x.DeleteAsync(tripShareId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = CreateService();

        await service.RemoveAsync(tripId, tripShareId, ownerId, CancellationToken.None);

        _tripShareRepository.Verify(x => x.DeleteAsync(tripShareId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserIsOwner_ShouldUpdatePermissionAndSaveChanges()
    {
        var ownerId = Guid.NewGuid();
        var tripId = Guid.NewGuid();
        var tripShareId = Guid.NewGuid();

        var trip = CreateTrip(tripId, ownerId);

        var tripShare = new TripShare
        {
            Id = tripShareId,
            TripId = tripId,
            UserId = Guid.NewGuid(),
            Permission = TripPermission.ReadOnly
        };

        var request = new UpdateTripShareRequest
        {
            Permission = TripPermission.Edit
        };

        _tripRepository
            .Setup(x => x.GetByIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trip);

        _tripShareRepository
            .Setup(x => x.GetByIdAsync(tripShareId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tripShare);

        var service = CreateService();

        await service.UpdateAsync(tripId, tripShareId, request, ownerId, CancellationToken.None);

        tripShare.Permission.Should().Be(TripPermission.Edit);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static Trip CreateTrip(Guid tripId, Guid userId)
    {
        return new Trip
        {
            Id = tripId,
            UserId = userId,
            Name = "Test trip",
            Description = "Test description",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(3),
            DestinationPlace = new Place
            {
                Id = Guid.NewGuid(),
                ExternalId = "place-1",
                Name = "Paris",
                Country = "France"
            },
            TripShares = new List<TripShare>(),
            Places = new List<TripPlace>()
        };
    }
}