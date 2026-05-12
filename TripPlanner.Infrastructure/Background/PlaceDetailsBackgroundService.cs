using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TripPlanner.Application.Interfaces;
using TripPlanner.Application.Interfaces.Background;

namespace TripPlanner.Infrastructure.Background
{
    public class PlaceDetailsBackgroundService : BackgroundService
    {
        private readonly IBackgroundTaskQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;

        public PlaceDetailsBackgroundService(
            IBackgroundTaskQueue queue,
            IServiceScopeFactory scopeFactory)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var placeDetailsId = await _queue.DequeueAsync(stoppingToken);

                using var scope = _scopeFactory.CreateScope();

                var processor = scope.ServiceProvider
                    .GetRequiredService<IPlaceDetailsGenerationProcessor>();

                await processor.ProcessAsync(placeDetailsId, stoppingToken);
            }
        }
    }
}
