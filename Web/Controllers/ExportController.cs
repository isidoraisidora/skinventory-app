using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExportController : ControllerBase
{
    private readonly IExcelExportService _exportService;

    public ExportController(IExcelExportService exportService)
    {
        _exportService = exportService;
    }

    [HttpGet("products")]
    public async Task<IActionResult> ExportProducts()
    {
        var bytes = await _exportService.ExportProductsAsync();
        return File(
            bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"products.xlsx");
    }
}