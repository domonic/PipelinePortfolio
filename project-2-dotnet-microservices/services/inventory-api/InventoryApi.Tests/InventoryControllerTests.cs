using InventoryApi.Controllers;
using InventoryApi.Models;
using InventoryApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace InventoryApi.Tests;

public class InventoryControllerTests
{
    private readonly Mock<InventoryService> _mockService;
    private readonly InventoryController _controller;

    public InventoryControllerTests()
    {
        _mockService = new Mock<InventoryService>(null!, null!, null!);
        _controller = new InventoryController(_mockService.Object);
    }

    [Fact]
    public async Task GetInventory_ReturnsNotFound_WhenProductDoesNotExist()
    {
        _mockService.Setup(s => s.GetInventoryAsync("nonexistent"))
            .ReturnsAsync((InventoryItem?)null);

        var result = await _controller.GetInventory("nonexistent");

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Reserve_ReturnsConflict_WhenInsufficientInventory()
    {
        var request = new ReserveInventoryRequest
        {
            OrderId = "order-1",
            ProductId = "prod-1",
            Quantity = 100
        };
        _mockService.Setup(s => s.ReserveInventoryAsync(request))
            .ReturnsAsync(false);

        var result = await _controller.Reserve(request);

        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task Reserve_ReturnsOk_WhenInventoryAvailable()
    {
        var request = new ReserveInventoryRequest
        {
            OrderId = "order-1",
            ProductId = "prod-1",
            Quantity = 5
        };
        _mockService.Setup(s => s.ReserveInventoryAsync(request))
            .ReturnsAsync(true);

        var result = await _controller.Reserve(request);

        Assert.IsType<OkResult>(result);
    }
}
