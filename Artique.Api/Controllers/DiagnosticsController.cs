using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Artique.Api.Controllers;

[Route("api/diagnostics")]
[ApiController]
public class DiagnosticsController(IWebHostEnvironment env) 
    : ControllerBase
{
    private readonly IWebHostEnvironment _env = env;
    
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