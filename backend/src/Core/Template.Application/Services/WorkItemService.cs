using Template.Application.Contracts;
using Template.Application.DTOs;
using Template.Domain.Entities;
using Template.Domain.Enums;
using Template.Domain.Exceptions;
using Template.Domain.Repositories;

namespace Template.Application.Services;

public class WorkItemService : IWorkItemService
{
    private readonly IWorkItemRepository _writeRepository;
    private readonly IWorkItemReadRepository _readRepository;

    public WorkItemService(
        IWorkItemRepository writeRepository,
        IWorkItemReadRepository readRepository)
    {
        _writeRepository = writeRepository;
        _readRepository = readRepository;
    }

    public async Task<IReadOnlyList<WorkItemDto>> ListAsync(WorkItemStatus? status = null, CancellationToken cancellationToken = default)
    {
        return await _readRepository.ListAsync(status, cancellationToken);
    }

    public async Task<WorkItemDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _readRepository.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            throw new EntityNotFoundException(nameof(WorkItem), id);
        }

        return item;
    }

    public async Task<WorkItemDto> CreateAsync(CreateWorkItemDto dto, CancellationToken cancellationToken = default)
    {
        var item = new WorkItem(dto.Title, dto.Description, dto.Priority);
        await _writeRepository.AddAsync(item, cancellationToken);

        return MapToDto(item);
    }

    public async Task<WorkItemDto> UpdateAsync(Guid id, UpdateWorkItemDto dto, CancellationToken cancellationToken = default)
    {
        var item = await _writeRepository.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            throw new EntityNotFoundException(nameof(WorkItem), id);
        }

        item.UpdateDetails(dto.Title, dto.Description, dto.Priority);
        await _writeRepository.UpdateAsync(item, cancellationToken);

        return MapToDto(item);
    }

    public async Task<WorkItemDto> UpdateStatusAsync(Guid id, UpdateWorkItemStatusDto dto, CancellationToken cancellationToken = default)
    {
        var item = await _writeRepository.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            throw new EntityNotFoundException(nameof(WorkItem), id);
        }

        item.UpdateStatus(dto.Status);
        await _writeRepository.UpdateAsync(item, cancellationToken);

        return MapToDto(item);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _writeRepository.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            throw new EntityNotFoundException(nameof(WorkItem), id);
        }

        await _writeRepository.DeleteAsync(item, cancellationToken);
    }

    private static WorkItemDto MapToDto(WorkItem item)
    {
        return new WorkItemDto(
            item.Id,
            item.Title,
            item.Description,
            item.Status,
            item.Priority,
            item.CreatedAtUtc,
            item.UpdatedAtUtc
        );
    }
}
