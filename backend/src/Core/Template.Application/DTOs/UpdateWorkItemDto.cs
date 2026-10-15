using System.ComponentModel.DataAnnotations;
using Template.Domain.Enums;

namespace Template.Application.DTOs;

public record UpdateWorkItemDto(
    [Required]
    [StringLength(120, MinimumLength = 1)]
    string Title,

    [StringLength(500)]
    string? Description,

    WorkItemPriority Priority
);

public record UpdateWorkItemStatusDto(
    WorkItemStatus Status
);
