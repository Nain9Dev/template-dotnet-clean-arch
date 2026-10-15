using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Application.Contracts;
using Template.Application.Services;
using Template.Domain.Repositories;
using Template.Infrastructure.Persistence;
using Template.Infrastructure.Persistence.Repositories;

namespace Template.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Data Source=template_app.db";

        var provider = configuration["DatabaseProvider"]?.ToLowerInvariant();

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (provider == "postgres" || connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase))
            {
                options.UseNpgsql(connectionString);
            }
            else
            {
                options.UseSqlite(connectionString);
            }
        });

        services.AddScoped<IWorkItemRepository, WorkItemEfRepository>();
        services.AddScoped<IWorkItemReadRepository, WorkItemDapperReadRepository>();
        services.AddScoped<IWorkItemService, WorkItemService>();

        return services;
    }
}
