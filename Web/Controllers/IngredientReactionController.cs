using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Mappers;
using Web.Request;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IngredientReactionController : ControllerBase
{
    private readonly IngredientReactionMapper _ingredientReactionMapper;

    public IngredientReactionController(IngredientReactionMapper ingredientReactionMapper)
    {
        _ingredientReactionMapper = ingredientReactionMapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await _ingredientReactionMapper.GetAllAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> LogAsync([FromBody] LogReactionRequest request)
    {
        var result = await _ingredientReactionMapper.LogAsync(request);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateReactionRequest request)
    {
        var result = await _ingredientReactionMapper.UpdateAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        await _ingredientReactionMapper.DeleteAsync(id);
        return Ok();
    }

    [HttpGet("conflicts/{productId}")]
    public async Task<IActionResult> GetConflictsAsync(Guid productId)
    {
        var result = await _ingredientReactionMapper.GetConflictingIngredientsAsync(productId);
        return Ok(result);
    }
}