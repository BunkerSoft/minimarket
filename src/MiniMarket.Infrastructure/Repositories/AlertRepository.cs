using Microsoft.EntityFrameworkCore;
using MiniMarket.Domain.Entities;
using MiniMarket.Domain.Enums;
using MiniMarket.Domain.Ports.Output;
using MiniMarket.Infrastructure.Data;

namespace MiniMarket.Infrastructure.Repositories;

public class AlertRepository : RepositoryBase<Alert, Guid>, IAlertRepository
{
    public AlertRepository(MiniMarketDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Alert>> GetActiveAsync()
    {
        return await DbSet
            .Where(a => a.Status == AlertStatus.Active || a.Status == AlertStatus.Acknowledged)
            .OrderByDescending(a => a.Severity)
            .ThenByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Alert>> GetByTypeAsync(AlertType type)
    {
        return await DbSet
            .Where(a => a.Type == type && (a.Status == AlertStatus.Active || a.Status == AlertStatus.Acknowledged))
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Alert>> GetBySeverityAsync(AlertSeverity severity)
    {
        return await DbSet
            .Where(a => a.Severity == severity && (a.Status == AlertStatus.Active || a.Status == AlertStatus.Acknowledged))
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<Alert?> GetByEntityAsync(string entityType, Guid entityId, AlertType type)
    {
        return await DbSet
            .FirstOrDefaultAsync(a =>
                a.EntityType == entityType &&
                a.EntityId == entityId &&
                a.Type == type &&
                (a.Status == AlertStatus.Active || a.Status == AlertStatus.Acknowledged));
    }

    public async Task<int> GetActiveCountAsync()
    {
        return await DbSet
            .CountAsync(a => a.Status == AlertStatus.Active || a.Status == AlertStatus.Acknowledged);
    }

    public async Task<int> GetActiveCountByTypeAsync(AlertType type)
    {
        return await DbSet
            .CountAsync(a => a.Type == type && (a.Status == AlertStatus.Active || a.Status == AlertStatus.Acknowledged));
    }
}
