using Microsoft.AspNetCore.Mvc;
using Template.Application.DTOs;
using Template.Application.Services;
using Template.Domain.Enums;

namespace Template.Api.Controllers;

[ApiController]
[Route("api/v1/work-items")]
[Produces("application/json")]
public class WorkItemsController : ControllerBase
{
    private readonly IWorkItemService _service;

    public WorkItemsController(IWorkItemService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<WorkItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<WorkItemDto>>> List(
        [FromQuery] WorkItemStatus? status,
        CancellationToken cancellationToken)
    {
        var items = await _service.ListAsync(status, cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(WorkItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkItemDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var item = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(item);
    }

    [HttpPost]
    [ProducesResponseType(typeof(WorkItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WorkItemDto>> Create(
        [FromBody] CreateWorkItemDto dto,
        CancellationToken cancellationToken)
    {
        var created = await _service.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(WorkItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkItemDto>> Update(
        Guid id,
        [FromBody] UpdateWorkItemDto dto,
        CancellationToken cancellationToken)
    {
        var updated = await _service.UpdateAsync(id, dto, cancellationToken);
        return Ok(updated);
    }

    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(WorkItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkItemDto>> UpdateStatus(
        Guid id,
        [FromBody] UpdateWorkItemStatusDto dto,
        CancellationToken cancellationToken)
    {
        var updated = await _service.UpdateStatusAsync(id, dto, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
