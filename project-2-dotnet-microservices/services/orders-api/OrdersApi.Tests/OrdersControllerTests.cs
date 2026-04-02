using Microsoft.AspNetCore.Mvc;
using Moq;
using OrdersApi.Controllers;
using OrdersApi.Models;
using OrdersApi.Services;
using Xunit;

namespace OrdersApi.Tests;

public class OrdersControllerTests
{
    private readonly Mock<OrderService> _mockService;
    private readonly OrdersController _controller;

    public OrdersControllerTests()
    {
        _mockService = new Mock<OrderService>(null!, null!, null!, null!);
        _controller = new OrdersController(_mockService.Object);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenOrderDoesNotExist()
    {
        _mockService.Setup(s => s.GetOrderAsync("nonexistent"))
            .ReturnsAsync((Order?)null);

        var result = await _controller.GetById("nonexistent");

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetById_ReturnsOrder_WhenExists()
    {
        var order = new Order { Id = "test-123", CustomerId = "cust-1" };
        _mockService.Setup(s => s.GetOrderAsync("test-123"))
            .ReturnsAsync(order);

        var result = await _controller.GetById("test-123");

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<Order>(okResult.Value);
        Assert.Equal("test-123", returned.Id);
    }
}
