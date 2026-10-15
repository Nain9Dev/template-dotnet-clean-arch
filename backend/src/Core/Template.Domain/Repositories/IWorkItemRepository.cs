using Template.Domain.Entities;

namespace Template.Domain.Repositories;

public interface IWorkItemRepository
{
    Task<WorkItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(WorkItem item, CancellationToken cancellationToken = default);
    Task UpdateAsync(WorkItem item, CancellationToken cancellationToken = default);
    Task DeleteAsync(WorkItem item, CancellationToken cancellationToken = default);
}
