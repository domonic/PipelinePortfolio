using InventoryApi.Models;
using InventoryApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly InventoryService _inventoryService;

    public InventoryController(InventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet("{productId}")]
    public async Task<ActionResult<InventoryItem>> GetInventory(string productId)
    {
        var item = await _inventoryService.GetInventoryAsync(productId);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost("reserve")]
    public async Task<ActionResult> Reserve([FromBody] ReserveInventoryRequest request)
    {
        var success = await _inventoryService.ReserveInventoryAsync(request);
        if (!success) return Conflict("Insufficient inventory");
        return Ok();
    }
}
