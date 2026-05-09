using TripPlanner.Application.Interfaces;
using TripPlanner.Application.Models;
using TripPlanner.Domain.Entities;
using TripPlanner.Domain.Enums;

namespace TripPlanner.Infrastructure.Services.Google
{
    public class LoggingGooglePlaceProviderDecorator : IPlaceProvider
    {
        private readonly IPlaceProvider _placeProvider;
        private readonly IPlaceProviderRequestLogRepository _logRepository;
        private readonly ICurrentUserService _currentUserService;

        public LoggingGooglePlaceProviderDecorator(IPlaceProvider placeProvider, IPlaceProviderRequestLogRepository logRepository, ICurrentUserService currentUserService)
        {
            _placeProvider = placeProvider;
            _logRepository = logRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<PlaceAutoCompleteResult>> AutoCompleteAsync(string query, CancellationToken cancellationToken)
        {
            return await ExecuteMethod(() => _placeProvider.AutoCompleteAsync(query, cancellationToken), PlaceProviderEndpointType.AutoCompleteRequests, cancellationToken);
        }

        public async Task<PlaceResult> GetPlaceAsync(string externalPlaceId, CancellationToken cancellationToken)
        {
            return await ExecuteMethod(() => _placeProvider.GetPlaceAsync(externalPlaceId, cancellationToken), PlaceProviderEndpointType.PlaceDetailsEnterprise, cancellationToken);
        }

        public async Task<string> GetPlacePhotoAsync(string photoName, CancellationToken cancellationToken)
        {
            return await ExecuteMethod(() => _placeProvider.GetPlacePhotoAsync(photoName, cancellationToken), PlaceProviderEndpointType.PlaceDetailsPhotos, cancellationToken);
        }

        public async Task<List<PlaceResult>> NearbySearchPlacesAsync(decimal latitude, decimal longitude, CancellationToken cancellationToken)
        {
            return await ExecuteMethod(() => _placeProvider.NearbySearchPlacesAsync(latitude, longitude, cancellationToken), PlaceProviderEndpointType.NearbySearchEnterprise, cancellationToken);
        }

        public async Task<List<PlaceResult>> TextSearchPlacesAsync(decimal latitude, decimal longitude, string query, CancellationToken cancellationToken)
        {
            return await ExecuteMethod(() => _placeProvider.TextSearchPlacesAsync(latitude, longitude, query, cancellationToken), PlaceProviderEndpointType.TextSearchEnterprise, cancellationToken);
        }

        private async Task<T> ExecuteMethod<T>(Func<Task<T>> method, PlaceProviderEndpointType endpointType, CancellationToken cancellationToken)
        {
            PlaceProviderRequestLog log = new PlaceProviderRequestLog
            {
                Provider = PlaceProvider.GooglePlacesAPI,
                EndpointType = endpointType,
                UserId = _currentUserService.UserId,
            };

            try
            {
                var result = await method();

                log.Succeeded = true;
                log.DurationMs = (int)(DateTime.UtcNow - log.RequestedAt).TotalMilliseconds;

                return result;
            }
            catch
            {
                log.Succeeded = false;
                log.DurationMs = (int)(DateTime.UtcNow - log.RequestedAt).TotalMilliseconds;

                throw;
            }
            finally
            {
                await _logRepository.AddAsync(log, cancellationToken);
            }
        }
    }
}
