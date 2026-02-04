using Microsoft.EntityFrameworkCore;
using MiniMarket.Domain.Entities;
using MiniMarket.Domain.Ports.Output;
using MiniMarket.Infrastructure.Data;

namespace MiniMarket.Infrastructure.Repositories;

public class IdempotencyRepository : RepositoryBase<IdempotencyRecord, Guid>, IIdempotencyRepository
{
    public IdempotencyRepository(MiniMarketDbContext context) : base(context)
    {
    }

    public async Task<IdempotencyRecord?> GetByKeyAsync(Guid idempotencyKey, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(x => x.IdempotencyKey == idempotencyKey, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid idempotencyKey, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(x => x.IdempotencyKey == idempotencyKey, cancellationToken);
    }

    public async Task<IReadOnlyList<IdempotencyRecord>> GetExpiredAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await DbSet
            .Where(x => x.ExpiresAt < now)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteExpiredAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        await DbSet
            .Where(x => x.ExpiresAt < now)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
