using Microsoft.AspNetCore.Mvc;
using Web.Mappers;
using Web.Request;
using Web.Response;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly ProductMapper _productMapper;

    public ProductController(ProductMapper productMapper)
    {
        _productMapper = productMapper;
    }

    [HttpGet]
    public async Task<List<ProductResponse>> GetAllAsync([FromQuery] string? name, [FromQuery] string? brand)
    {
        return await _productMapper.GetAllAsync(name, brand);
    }

    [HttpGet("paged")]
    public async Task<PaginatedResponse<ProductResponse>> GetAllPagedAsync([FromQuery] PaginatedRequest request)
    {
        return await _productMapper.GetAllPaginatedAsync(request);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
    {
        var result = await _productMapper.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> RegisterAsync([FromBody] ProductRequest request)
    {
        var result = await _productMapper.InsertAsync(request);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] ProductRequest request)
    {
        var result = await _productMapper.UpdateAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
    {
        var result = await _productMapper.DeleteAsync(id);
        return Ok(result);
    }
}