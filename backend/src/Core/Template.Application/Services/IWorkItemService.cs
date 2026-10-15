using Template.Application.DTOs;
using Template.Domain.Enums;

namespace Template.Application.Services;

public interface IWorkItemService
{
    Task<IReadOnlyList<WorkItemDto>> ListAsync(WorkItemStatus? status = null, CancellationToken cancellationToken = default);
    Task<WorkItemDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WorkItemDto> CreateAsync(CreateWorkItemDto dto, CancellationToken cancellationToken = default);
    Task<WorkItemDto> UpdateAsync(Guid id, UpdateWorkItemDto dto, CancellationToken cancellationToken = default);
    Task<WorkItemDto> UpdateStatusAsync(Guid id, UpdateWorkItemStatusDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
