using Template.Domain.Enums;

namespace Template.Application.DTOs;

public record WorkItemDto(
    Guid Id,
    string Title,
    string Description,
    WorkItemStatus Status,
    WorkItemPriority Priority,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc
);
