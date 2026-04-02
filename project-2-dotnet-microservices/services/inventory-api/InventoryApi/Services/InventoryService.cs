using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using InventoryApi.Models;

namespace InventoryApi.Services;

public class InventoryService
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly IConfiguration _config;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(
        IAmazonDynamoDB dynamoDb,
        IConfiguration config,
        ILogger<InventoryService> logger)
    {
        _dynamoDb = dynamoDb;
        _config = config;
        _logger = logger;
    }

    public async Task<InventoryItem?> GetInventoryAsync(string productId)
    {
        var table = Table.LoadTable(_dynamoDb, _config["AWS:InventoryTable"] ?? "inventory");
        var doc = await table.GetItemAsync(productId);
        if (doc == null) return null;
        return JsonSerializer.Deserialize<InventoryItem>(doc.ToJson());
    }

    public async Task<bool> ReserveInventoryAsync(ReserveInventoryRequest request)
    {
        var item = await GetInventoryAsync(request.ProductId);
        if (item == null || item.QuantityAvailable < request.Quantity)
        {
            _logger.LogWarning(
                "Insufficient inventory for product {ProductId}. Requested: {Qty}, Available: {Available}",
                request.ProductId, request.Quantity, item?.QuantityAvailable ?? 0);
            return false;
        }

        item.QuantityAvailable -= request.Quantity;
        item.QuantityReserved += request.Quantity;
        item.LastUpdated = DateTime.UtcNow;

        var table = Table.LoadTable(_dynamoDb, _config["AWS:InventoryTable"] ?? "inventory");
        var json = JsonSerializer.Serialize(item);
        await table.PutItemAsync(Document.FromJson(json));

        _logger.LogInformation(
            "Reserved {Qty} of product {ProductId} for order {OrderId}",
            request.Quantity, request.ProductId, request.OrderId);

        return true;
    }
}
