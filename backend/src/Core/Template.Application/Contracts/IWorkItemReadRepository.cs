using Template.Application.DTOs;
using Template.Domain.Enums;

namespace Template.Application.Contracts;

public interface IWorkItemReadRepository
{
    Task<IReadOnlyList<WorkItemDto>> ListAsync(WorkItemStatus? status = null, CancellationToken cancellationToken = default);
    Task<WorkItemDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
