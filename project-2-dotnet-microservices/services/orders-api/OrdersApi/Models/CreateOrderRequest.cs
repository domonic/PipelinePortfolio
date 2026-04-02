using System.ComponentModel.DataAnnotations;

namespace OrdersApi.Models;

public class CreateOrderRequest
{
    [Required]
    public string CustomerId { get; set; } = string.Empty;

    [Required, MinLength(1)]
    public List<OrderItemRequest> Items { get; set; } = new();
}

public class OrderItemRequest
{
    [Required]
    public string ProductId { get; set; } = string.Empty;

    [Range(1, 1000)]
    public int Quantity { get; set; }
}
