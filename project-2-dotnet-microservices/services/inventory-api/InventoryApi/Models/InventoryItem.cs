namespace InventoryApi.Models;

public class InventoryItem
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int QuantityAvailable { get; set; }
    public int QuantityReserved { get; set; }
    public string Warehouse { get; set; } = "default";
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

public class ReserveInventoryRequest
{
    public string OrderId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
