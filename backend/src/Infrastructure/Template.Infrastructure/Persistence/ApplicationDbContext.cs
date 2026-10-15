using Microsoft.EntityFrameworkCore;
using Template.Domain.Entities;
using Template.Domain.Enums;

namespace Template.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<WorkItem> WorkItems => Set<WorkItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WorkItem>(builder =>
        {
            builder.ToTable("work_items");

            builder.HasKey(w => w.Id);

            builder.Property(w => w.Title)
                .IsRequired()
                .HasMaxLength(120);

            builder.Property(w => w.Description)
                .HasMaxLength(500);

            builder.Property(w => w.Status)
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(w => w.Priority)
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(w => w.CreatedAtUtc)
                .IsRequired();

            builder.Property(w => w.UpdatedAtUtc);

            builder.HasIndex(w => w.Status);
            builder.HasIndex(w => w.CreatedAtUtc);
        });
    }
}
