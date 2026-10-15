using Microsoft.AspNetCore.Mvc;

namespace Template.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;

    public HealthController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Check()
    {
        return Ok(new
        {
            status = "pass",
            service = "template-dotnet-clean-arch",
            framework = ".NET 10",
            environment = _environment.EnvironmentName,
            timestampUtc = DateTimeOffset.UtcNow
        });
    }
}
