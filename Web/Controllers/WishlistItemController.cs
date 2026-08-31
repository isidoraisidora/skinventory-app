using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Mappers;
using Web.Request;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WishlistItemController : ControllerBase
{
    private readonly WishlistItemMapper _wishlistItemMapper;

    public WishlistItemController(WishlistItemMapper wishlistItemMapper)
    {
        _wishlistItemMapper = wishlistItemMapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await _wishlistItemMapper.GetAllAsync();
        return Ok(result);
    }

    [HttpPost("{productId}")]
    public async Task<IActionResult> AddAsync(Guid productId)
    {
        var result = await _wishlistItemMapper.AddAsync(productId);
        return Ok(result);
    }

    [HttpPatch("{productId}/discard")]
    public async Task<IActionResult> DiscardAsync(Guid productId)
    {
        var result = await _wishlistItemMapper.DiscardAsync(productId);
        return Ok(result);
    }

    [HttpPost("{productId}/move-to-owned")]
    public async Task<IActionResult> MoveToOwnedAsync(Guid productId, [FromBody] MoveToOwnedRequest request)
    {
        var result = await _wishlistItemMapper.MoveToOwnedAsync(productId, request);
        return Ok(result);
    }
}