using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task AddAsync(User user, CancellationToken cancellationToken);
        Task<IEnumerable<User>> SearchAsync(string keyword, CancellationToken cancellationToken);
    }
}
