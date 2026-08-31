using Microsoft.AspNetCore.Mvc;
using Web.Mappers;
using Web.Request;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly CategoryMapper _categoryMapper;

    public CategoryController(CategoryMapper categoryMapper)
    {
        _categoryMapper = categoryMapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await _categoryMapper.GetAllAsync();
        return Ok(result);
    }
    

    [HttpGet("product/{productId}")]
    public async Task<IActionResult> GetForProductAsync(Guid productId)
    {
        var result = await _categoryMapper.GetForProductAsync(productId);
        return Ok(result);
    }
    
}