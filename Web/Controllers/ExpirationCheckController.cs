using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpirationCheckController : ControllerBase
{
    private readonly IExpirationCheckService _expirationCheckService;

    public ExpirationCheckController(IExpirationCheckService expirationCheckService)
    {
        _expirationCheckService = expirationCheckService;
    }

    [HttpPost("run")]
    public async Task<IActionResult> RunAsync()
    {
        await _expirationCheckService.RunAsync();
        return Ok("Expiration check completed.");
    }
}