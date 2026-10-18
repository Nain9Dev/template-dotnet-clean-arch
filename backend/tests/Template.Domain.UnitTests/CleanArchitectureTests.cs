using NetArchTest.Rules;
using Xunit;

namespace Template.Domain.UnitTests;

public sealed class CleanArchitectureTests
{
    private static readonly System.Reflection.Assembly DomainAssembly = typeof(Template.Domain.Entities.WorkItem).Assembly;
    private static readonly System.Reflection.Assembly ApplicationAssembly = typeof(Template.Application.Contracts.IWorkItemReadRepository).Assembly;
    private static readonly System.Reflection.Assembly InfrastructureAssembly = typeof(Template.Infrastructure.DependencyInjection).Assembly;
    private static readonly System.Reflection.Assembly ApiAssembly = typeof(Template.Api.Controllers.WorkItemsController).Assembly;

    [Fact]
    public void Domain_Should_Not_Have_Dependency_On_Other_Projects()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Template.Application",
                "Template.Infrastructure",
                "Template.Api")
            .GetResult();

        Assert.True(result.IsSuccessful, $"Domain layer has illegal dependencies on outer layers: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Application_Should_Not_Have_Dependency_On_Infrastructure_Or_Api()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Template.Infrastructure",
                "Template.Api")
            .GetResult();

        Assert.True(result.IsSuccessful, $"Application layer has illegal dependencies: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Infrastructure_Should_Not_Have_Dependency_On_Api()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOn("Template.Api")
            .GetResult();

        Assert.True(result.IsSuccessful, $"Infrastructure layer has illegal dependencies on Api: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }
}
