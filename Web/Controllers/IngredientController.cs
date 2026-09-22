using Microsoft.AspNetCore.Mvc;
using Web.Mappers;
using Web.Request;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IngredientController : ControllerBase
{
    private readonly IngredientMapper _ingredientMapper;

    public IngredientController(IngredientMapper ingredientMapper)
    {
        _ingredientMapper = ingredientMapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await _ingredientMapper.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var result = await _ingredientMapper.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] IngredientRequest request)
    {
        var result = await _ingredientMapper.CreateAsync(request);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] IngredientRequest request)
    {
        var result = await _ingredientMapper.UpdateAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var result = await _ingredientMapper.DeleteAsync(id);
        return Ok(result);
    }
}