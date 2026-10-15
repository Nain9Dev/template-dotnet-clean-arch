using System.ComponentModel.DataAnnotations;
using Template.Domain.Enums;

namespace Template.Application.DTOs;

public record CreateWorkItemDto(
    [Required]
    [StringLength(120, MinimumLength = 1)]
    string Title,

    [StringLength(500)]
    string? Description,

    WorkItemPriority Priority = WorkItemPriority.Medium
);
