using Template.Domain.Enums;
using Template.Domain.Exceptions;

namespace Template.Domain.Entities;

public class WorkItem
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public WorkItemStatus Status { get; private set; }
    public WorkItemPriority Priority { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? UpdatedAtUtc { get; private set; }

    // Required by EF Core
    private WorkItem() { }

    public WorkItem(string title, string? description, WorkItemPriority priority = WorkItemPriority.Medium)
    {
        ValidateTitle(title);

        Id = Guid.NewGuid();
        Title = title.Trim();
        Description = description?.Trim() ?? string.Empty;
        Priority = priority;
        Status = WorkItemStatus.Todo;
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void UpdateDetails(string title, string? description, WorkItemPriority priority)
    {
        ValidateTitle(title);

        Title = title.Trim();
        Description = description?.Trim() ?? string.Empty;
        Priority = priority;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void UpdateStatus(WorkItemStatus newStatus)
    {
        if (Status == WorkItemStatus.Archived && newStatus != WorkItemStatus.Archived)
        {
            throw new DomainException("Cannot transition out of Archived status directly.");
        }

        Status = newStatus;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("WorkItem title cannot be null, empty or whitespace.");
        }

        if (title.Trim().Length > 120)
        {
            throw new DomainException("WorkItem title cannot exceed 120 characters.");
        }
    }
}
