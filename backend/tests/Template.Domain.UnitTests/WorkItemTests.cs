using Template.Domain.Entities;
using Template.Domain.Enums;
using Template.Domain.Exceptions;
using Xunit;

namespace Template.Domain.UnitTests;

public class WorkItemTests
{
    [Fact]
    public void Constructor_WithValidParameters_InitializesCorrectly()
    {
        // Act
        var item = new WorkItem("Design Architecture", "Setup Clean Architecture layers", WorkItemPriority.High);

        // Assert
        Assert.NotEqual(Guid.Empty, item.Id);
        Assert.Equal("Design Architecture", item.Title);
        Assert.Equal("Setup Clean Architecture layers", item.Description);
        Assert.Equal(WorkItemPriority.High, item.Priority);
        Assert.Equal(WorkItemStatus.Todo, item.Status);
        Assert.True(item.CreatedAtUtc <= DateTimeOffset.UtcNow);
        Assert.Null(item.UpdatedAtUtc);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithEmptyOrNullTitle_ThrowsDomainException(string? invalidTitle)
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new WorkItem(invalidTitle!, "Test"));
        Assert.Contains("title cannot be null, empty or whitespace", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UpdateStatus_ToValidStatus_UpdatesStatusAndTimestamp()
    {
        // Arrange
        var item = new WorkItem("Task 1", null);

        // Act
        item.UpdateStatus(WorkItemStatus.InProgress);

        // Assert
        Assert.Equal(WorkItemStatus.InProgress, item.Status);
        Assert.NotNull(item.UpdatedAtUtc);
    }

    [Fact]
    public void UpdateStatus_FromArchivedToTodo_ThrowsDomainException()
    {
        // Arrange
        var item = new WorkItem("Task 1", null);
        item.UpdateStatus(WorkItemStatus.Archived);

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => item.UpdateStatus(WorkItemStatus.Todo));
        Assert.Contains("Cannot transition out of Archived status", ex.Message);
    }
}
