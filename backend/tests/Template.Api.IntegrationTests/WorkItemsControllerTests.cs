using System.Data.Common;
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Template.Application.DTOs;
using Template.Domain.Enums;
using Template.Infrastructure.Persistence;
using Xunit;

namespace Template.Api.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection?.Dispose();
    }
}

public class WorkItemsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public WorkItemsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthEndpoint_Returns200WithPassStatus()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("pass", content);
    }

    [Fact]
    public async Task CreateAndGetWorkItem_FlowSucceeds()
    {
        // Arrange
        var createDto = new CreateWorkItemDto("Test WorkItem", "Description for test", WorkItemPriority.High);

        // Act - Create
        var createResponse = await _client.PostAsJsonAsync("/api/v1/work-items", createDto);

        // Assert - Created
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<WorkItemDto>();
        Assert.NotNull(created);
        Assert.Equal(createDto.Title, created.Title);
        Assert.Equal(WorkItemStatus.Todo, created.Status);

        // Act - Get By ID
        var getResponse = await _client.GetAsync($"/api/v1/work-items/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        // Act - List
        var listResponse = await _client.GetAsync("/api/v1/work-items");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        var list = await listResponse.Content.ReadFromJsonAsync<List<WorkItemDto>>();
        Assert.NotNull(list);
        Assert.Contains(list, x => x.Id == created.Id);
    }
}
