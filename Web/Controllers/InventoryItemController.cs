using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Mappers;
using Web.Request;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryItemController : ControllerBase
{
    private readonly InventoryItemMapper _inventoryItemMapper;

    public InventoryItemController(InventoryItemMapper inventoryItemMapper)
    {
        _inventoryItemMapper = inventoryItemMapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await _inventoryItemMapper.GetAllAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] CreateInventoryItemRequest request)
    {
        var result = await _inventoryItemMapper.AddAsync(request);
        return Ok(result);
    }

    [HttpPatch("{productId}/open")]
    public async Task<IActionResult> OpenAsync(Guid productId)
    {
        var result = await _inventoryItemMapper.OpenAsync(productId);
        return Ok(result);
    }

    [HttpPatch("{productId}/finish")]
    public async Task<IActionResult> FinishAsync(Guid productId)
    {
        var result = await _inventoryItemMapper.FinishAsync(productId);
        return Ok(result);
    }

    [HttpPatch("{productId}/discard")]
    public async Task<IActionResult> DiscardAsync(Guid productId)
    {
        var result = await _inventoryItemMapper.DiscardAsync(productId);
        return Ok(result);
    }

    [HttpPut("{productId}")]
    public async Task<IActionResult> UpdateAsync(Guid productId, [FromBody] UpdateInventoryItemRequest request)
    {
        var result = await _inventoryItemMapper.UpdateAsync(productId, request);
        return Ok(result);
    }
}