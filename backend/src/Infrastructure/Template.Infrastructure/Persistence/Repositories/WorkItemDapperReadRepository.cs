using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Template.Application.Contracts;
using Template.Application.DTOs;
using Template.Domain.Enums;

namespace Template.Infrastructure.Persistence.Repositories;

public class WorkItemDapperReadRepository : IWorkItemReadRepository
{
    private readonly ApplicationDbContext _context;

    public WorkItemDapperReadRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<WorkItemDto>> ListAsync(WorkItemStatus? status = null, CancellationToken cancellationToken = default)
    {
        var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        string sql = status.HasValue
            ? "SELECT Id, Title, Description, Status, Priority, CreatedAtUtc, UpdatedAtUtc FROM work_items WHERE Status = @Status ORDER BY CreatedAtUtc DESC"
            : "SELECT Id, Title, Description, Status, Priority, CreatedAtUtc, UpdatedAtUtc FROM work_items ORDER BY CreatedAtUtc DESC";

        var parameters = status.HasValue ? new { Status = status.Value.ToString() } : null;

        var rows = await connection.QueryAsync<WorkItemRecord>(
            new CommandDefinition(sql, parameters, cancellationToken: cancellationToken)
        );

        return rows.Select(MapToDto).ToList();
    }

    public async Task<WorkItemDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        const string sql = @"
            SELECT Id, Title, Description, Status, Priority, CreatedAtUtc, UpdatedAtUtc 
            FROM work_items 
            WHERE Id = @Id 
               OR Id = @IdString 
               OR Id = @IdStringLower 
               OR Id = @IdStringUpper 
            LIMIT 1";

        var row = await connection.QueryFirstOrDefaultAsync<WorkItemRecord>(
            new CommandDefinition(sql, new
            {
                Id = id,
                IdString = id.ToString(),
                IdStringLower = id.ToString().ToLowerInvariant(),
                IdStringUpper = id.ToString().ToUpperInvariant()
            }, cancellationToken: cancellationToken)
        );

        return row is null ? null : MapToDto(row);
    }

    private static WorkItemDto MapToDto(WorkItemRecord row)
    {
        var id = ParseGuid(row.Id);

        var status = Enum.TryParse<WorkItemStatus>(row.Status?.ToString(), ignoreCase: true, out var st)
            ? st
            : WorkItemStatus.Todo;

        var priority = Enum.TryParse<WorkItemPriority>(row.Priority?.ToString(), ignoreCase: true, out var pr)
            ? pr
            : WorkItemPriority.Medium;

        var createdAtUtc = ParseDateTimeOffset(row.CreatedAtUtc);
        var updatedAtUtc = row.UpdatedAtUtc is null ? (DateTimeOffset?)null : ParseDateTimeOffset(row.UpdatedAtUtc);

        return new WorkItemDto(
            id,
            row.Title ?? string.Empty,
            row.Description ?? string.Empty,
            status,
            priority,
            createdAtUtc,
            updatedAtUtc
        );
    }

    private static Guid ParseGuid(object? value)
    {
        if (value is null) return Guid.Empty;
        if (value is Guid g) return g;
        if (value is string s && Guid.TryParse(s, out var parsed)) return parsed;
        if (value is byte[] bytes && bytes.Length == 16) return new Guid(bytes);
        return Guid.Empty;
    }

    private static DateTimeOffset ParseDateTimeOffset(object? value)
    {
        if (value is null) return DateTimeOffset.UtcNow;
        if (value is DateTimeOffset dto) return dto;
        if (value is DateTime dt) return new DateTimeOffset(dt);
        if (value is string s && DateTimeOffset.TryParse(s, out var parsed)) return parsed;
        return DateTimeOffset.UtcNow;
    }

    private sealed class WorkItemRecord
    {
        public object? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public object? Status { get; set; }
        public object? Priority { get; set; }
        public object? CreatedAtUtc { get; set; }
        public object? UpdatedAtUtc { get; set; }
    }
}
