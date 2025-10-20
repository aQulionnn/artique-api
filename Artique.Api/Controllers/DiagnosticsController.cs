using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Artique.Api.Controllers;

[Route("api/diagnostics")]
[ApiController]
public class DiagnosticsController(HealthCheckService healthChecks, IWebHostEnvironment env) 
    : ControllerBase
{
    private readonly HealthCheckService _healthChecks = healthChecks;
    private readonly IWebHostEnvironment _env = env;

    [HttpGet]
    [Route("health")]
    public async Task<IActionResult> GetHealth()
    {
        var report = await _healthChecks.CheckHealthAsync();

        return Ok(new
        {
            Status = report.Status.ToString(),
            Results = report.Entries.ToDictionary(
                entry => entry.Key,
                entry => new
                {
                    status = entry.Value.Status.ToString(),
                    description = entry.Value.Description,
                    exception = entry.Value.Exception,
                    duration = entry.Value.Duration,
                    tags = entry.Value.Tags,
                    data = entry.Value.Data.Count > 0 ? entry.Value.Data : null
                })
        });  
    }
    
    [HttpGet]
    [Route("info")]
    public IActionResult GetInfo()
    {
        var url = Request.GetDisplayUrl();

        return Ok(new
        {
            Environment = new
            {
                Name = _env.EnvironmentName,
                Url = url
            },
            Application = new
            {
                Name = _env.ApplicationName,
                Version = "1.0.0"
            },
            System = new
            {
                Machine = Environment.MachineName,
                OS = Environment.OSVersion.ToString()
            },
            Runtime = new
            {
                Framework = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
            },
            Metadata = new
            {
                Timestamp = DateTime.UtcNow.ToString("O")
            }
        });
    }
}