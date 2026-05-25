using FluentAssertions;
using Moq;
using TripPlanner.Application.DTOs.Request;
using TripPlanner.Application.DTOs.Response;
using TripPlanner.Application.Exceptions;
using TripPlanner.Application.Interfaces.Repositories;
using TripPlanner.Application.Interfaces.Services;
using TripPlanner.Application.Interfaces.Utilities;
using TripPlanner.Application.Models;
using TripPlanner.Application.Services;
using TripPlanner.Domain.Entities;
using TripPlanner.Domain.Enums;

namespace TripPlanner.Application.Tests.Services;

public class TripServiceTests
{
    private readonly Mock<ITripRepository> _tripRepository = new();
    private readonly Mock<ITripShareRepository> _tripShareRepository = new();
    private readonly Mock<IPlaceSearchService> _placeSearchService = new();
    private readonly Mock<IPlaceRepository> _placeRepository = new();
    private readonly Mock<IBackgroundTaskQueue> _backgroundTaskQueue = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    public TripServiceTests()
    {
        _unitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _backgroundTaskQueue
            .Setup(x => x.QueueAsync(It.IsAny<PlaceDetailsGenerationJob>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);
    }

    private TripService CreateService()
    {
        return new TripService(
            _tripRepository.Object,
            _tripShareRepository.Object,
            _placeSearchService.Object,
            _placeRepository.Object,
            _backgroundTaskQueue.Object,
            _unitOfWork.Object);
    }

    [Fact]
    public async Task AddAsync_WhenDestinationPlaceExists_ShouldCreateTripOnly()
    {
        var userId = Guid.NewGuid();
        var existingPlace = new Place
        {
            Id = Guid.NewGuid(),
            ExternalId = "place-1",
            Name = "Paris",
            Country = "France"
        };

        var request = new TripRequest
        {
            Name = "Paris trip",
            Description = "Test trip",
            PlaceId = existingPlace.ExternalId,
            DestinationName = "Paris",
            DestinationCountry = "France",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(3)
        };

        Trip? addedTrip = null;

        _placeRepository
            .Setup(x => x.GetByExternalIdAsync(request.PlaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPlace);

        _tripRepository
            .Setup(x => x.AddAsync(It.IsAny<Trip>(), It.IsAny<CancellationToken>()))
            .Callback<Trip, CancellationToken>((trip, _) => addedTrip = trip)
            .Returns(Task.CompletedTask);

        var service = CreateService();

        var result = await service.AddAsync(request, userId, CancellationToken.None);

        result.Should().NotBeEmpty();
        addedTrip.Should().NotBeNull();
        addedTrip!.Name.Should().Be(request.Name);
        addedTrip.UserId.Should().Be(userId);
        addedTrip.DestinationPlaceId.Should().Be(existingPlace.Id);

        _placeRepository.Verify(x => x.AddAsync(It.IsAny<Place>(), It.IsAny<CancellationToken>()), Times.Never);
        _backgroundTaskQueue.Verify(x => x.QueueAsync(It.IsAny<PlaceDetailsGenerationJob>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_WhenDestinationPlaceDoesNotExist_ShouldCreatePlaceQueueJobAndCreateTrip()
    {
        var userId = Guid.NewGuid();

        var request = new TripRequest
        {
            Name = "Rome trip",
            Description = "Test trip",
            PlaceId = "rome-place",
            DestinationName = "Rome",
            DestinationCountry = "Italy",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(4)
        };

        Place? addedPlace = null;
        Trip? addedTrip = null;
        PlaceDetailsGenerationJob? queuedJob = null;

        _placeRepository
            .Setup(x => x.GetByExternalIdAsync(request.PlaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Place?)null);

        _placeRepository
            .Setup(x => x.AddAsync(It.IsAny<Place>(), It.IsAny<CancellationToken>()))
            .Callback<Place, CancellationToken>((place, _) => addedPlace = place)
            .Returns(Task.CompletedTask);

        _tripRepository
            .Setup(x => x.AddAsync(It.IsAny<Trip>(), It.IsAny<CancellationToken>()))
            .Callback<Trip, CancellationToken>((trip, _) => addedTrip = trip)
            .Returns(Task.CompletedTask);

        _backgroundTaskQueue
            .Setup(x => x.QueueAsync(It.IsAny<PlaceDetailsGenerationJob>(), It.IsAny<CancellationToken>()))
            .Callback<PlaceDetailsGenerationJob, CancellationToken>((job, _) => queuedJob = job)
            .Returns(ValueTask.CompletedTask);

        var service = CreateService();

        await service.AddAsync(request, userId, CancellationToken.None);

        addedPlace.Should().NotBeNull();
        addedPlace!.ExternalId.Should().Be(request.PlaceId);
        addedPlace.Name.Should().Be(request.DestinationName);
        addedPlace.Country.Should().Be(request.DestinationCountry);

        queuedJob.Should().NotBeNull();
        queuedJob!.PlaceName.Should().Be(request.DestinationName);
        queuedJob.PlaceLocation.Should().Be(request.DestinationCountry);

        addedTrip.Should().NotBeNull();
        addedTrip!.UserId.Should().Be(userId);
        addedTrip.DestinationPlaceId.Should().Be(addedPlace.Id);

        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task RemoveAsync_WhenTripDoesNotExist_ShouldThrowNotFoundException()
    {
        var tripId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _tripRepository
            .Setup(x => x.GetByIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Trip?)null);

        var service = CreateService();

        var act = async () => await service.RemoveAsync(tripId, userId, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task RemoveAsync_WhenUserIsNotOwner_ShouldThrowForbiddenException()
    {
        var tripId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        var trip = new Trip
        {
            Id = tripId,
            UserId = ownerId,
            Name = "Test trip",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(3)
        };

        _tripRepository
            .Setup(x => x.GetByIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trip);

        var service = CreateService();

        var act = async () => await service.RemoveAsync(tripId, otherUserId, CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task RemoveAsync_WhenUserIsOwner_ShouldRemoveTripAndSaveChanges()
    {
        var tripId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var trip = new Trip
        {
            Id = tripId,
            UserId = userId,
            Name = "Test trip",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(3)
        };

        _tripRepository
            .Setup(x => x.GetByIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trip);

        var service = CreateService();

        await service.RemoveAsync(tripId, userId, CancellationToken.None);

        _tripRepository.Verify(x => x.Remove(trip), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdForUserAsync_WhenUserIsOwner_ShouldReturnTrip()
    {
        var userId = Guid.NewGuid();
        var tripId = Guid.NewGuid();

        var trip = CreateTrip(tripId, userId);

        _tripRepository
            .Setup(x => x.GetByIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trip);

        _placeSearchService
            .Setup(x => x.GetByExternalIdAsync(trip.DestinationPlace.ExternalId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreatePlaceResponse(trip.DestinationPlace.ExternalId));

        _placeSearchService
            .Setup(x => x.GetPlacesForTripWithDetailsAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PlacesResponse>());

        var service = CreateService();

        var result = await service.GetByIdForUserAsync(tripId, userId, CancellationToken.None);

        result.Id.Should().Be(tripId);
        result.Name.Should().Be(trip.Name);
        result.Shared.Should().BeFalse();
    }

    [Fact]
    public async Task GetByIdForUserAsync_WhenUserIsShared_ShouldReturnTripAsShared()
    {
        var ownerId = Guid.NewGuid();
        var sharedUserId = Guid.NewGuid();
        var tripId = Guid.NewGuid();

        var trip = CreateTrip(tripId, ownerId);
        trip.TripShares = new List<TripShare>
        {
            new()
            {
                TripId = tripId,
                UserId = sharedUserId,
                Permission = TripPermission.ReadOnly
            }
        };

        _tripRepository
            .Setup(x => x.GetByIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trip);

        _placeSearchService
            .Setup(x => x.GetByExternalIdAsync(trip.DestinationPlace.ExternalId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreatePlaceResponse(trip.DestinationPlace.ExternalId));

        _placeSearchService
            .Setup(x => x.GetPlacesForTripWithDetailsAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PlacesResponse>());

        var service = CreateService();

        var result = await service.GetByIdForUserAsync(tripId, sharedUserId, CancellationToken.None);

        result.Id.Should().Be(tripId);
        result.Shared.Should().BeTrue();
        result.SharedPermission.Should().Be(TripPermission.ReadOnly);
    }

    [Fact]
    public async Task GetByIdForUserAsync_WhenUserHasNoAccess_ShouldThrowForbiddenException()
    {
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var tripId = Guid.NewGuid();

        var trip = CreateTrip(tripId, ownerId);

        _tripRepository
            .Setup(x => x.GetByIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trip);

        var service = CreateService();

        var act = async () => await service.GetByIdForUserAsync(tripId, otherUserId, CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    private static Trip CreateTrip(Guid tripId, Guid userId)
    {
        var destinationPlace = new Place
        {
            Id = Guid.NewGuid(),
            ExternalId = "destination-place",
            Name = "Paris",
            Country = "France"
        };

        return new Trip
        {
            Id = tripId,
            UserId = userId,
            Name = "Paris trip",
            Description = "Test trip",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(3),
            DestinationPlaceId = destinationPlace.Id,
            DestinationPlace = destinationPlace,
            TripShares = new List<TripShare>(),
            Places = new List<TripPlace>()
        };
    }

    private static GetPlaceResponse CreatePlaceResponse(string externalId)
    {
        return new GetPlaceResponse
        {
            ExternalId = externalId,
            Name = "Paris",
            FormattedAddress = "Paris, France",
            Country = "France",
            Locality = "Paris",
            Latitude = 48.8566m,
            Longitude = 2.3522m,
            Rating = 4.8,
            UserRatingCount = 1000
        };
    }
}