using Microsoft.EntityFrameworkCore;
using Template.Domain.Entities;
using Template.Domain.Repositories;

namespace Template.Infrastructure.Persistence.Repositories;

public class WorkItemEfRepository : IWorkItemRepository
{
    private readonly ApplicationDbContext _context;

    public WorkItemEfRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WorkItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.WorkItems
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task AddAsync(WorkItem item, CancellationToken cancellationToken = default)
    {
        await _context.WorkItems.AddAsync(item, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(WorkItem item, CancellationToken cancellationToken = default)
    {
        _context.WorkItems.Update(item);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(WorkItem item, CancellationToken cancellationToken = default)
    {
        _context.WorkItems.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
