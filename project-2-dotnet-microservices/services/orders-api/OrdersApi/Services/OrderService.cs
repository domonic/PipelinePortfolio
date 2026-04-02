using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.SQS;
using Amazon.SQS.Model;
using OrdersApi.Models;

namespace OrdersApi.Services;

public class OrderService
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly IAmazonSQS _sqs;
    private readonly IConfiguration _config;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IAmazonDynamoDB dynamoDb,
        IAmazonSQS sqs,
        IConfiguration config,
        ILogger<OrderService> logger)
    {
        _dynamoDb = dynamoDb;
        _sqs = sqs;
        _config = config;
        _logger = logger;
    }

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        var order = new Order
        {
            CustomerId = request.CustomerId,
            Items = request.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList(),
            Status = OrderStatus.Pending
        };

        // Save to DynamoDB
        var table = Table.LoadTable(_dynamoDb, _config["AWS:OrdersTable"] ?? "orders");
        var json = JsonSerializer.Serialize(order);
        await table.PutItemAsync(Document.FromJson(json));

        // Publish to SQS for inventory reservation
        var queueUrl = _config["AWS:InventoryQueueUrl"] ?? "";
        await _sqs.SendMessageAsync(new SendMessageRequest
        {
            QueueUrl = queueUrl,
            MessageBody = json,
            MessageGroupId = order.CustomerId
        });

        _logger.LogInformation("Order {OrderId} created for customer {CustomerId}",
            order.Id, order.CustomerId);

        return order;
    }

    public async Task<Order?> GetOrderAsync(string orderId)
    {
        var table = Table.LoadTable(_dynamoDb, _config["AWS:OrdersTable"] ?? "orders");
        var doc = await table.GetItemAsync(orderId);
        if (doc == null) return null;
        return JsonSerializer.Deserialize<Order>(doc.ToJson());
    }

    public async Task<List<Order>> GetOrdersByCustomerAsync(string customerId)
    {
        var table = Table.LoadTable(_dynamoDb, _config["AWS:OrdersTable"] ?? "orders");
        var filter = new QueryFilter("CustomerId", QueryOperator.Equal, customerId);
        var search = table.Query(new QueryOperationConfig
        {
            IndexName = "CustomerId-index",
            Filter = filter
        });

        var orders = new List<Order>();
        do
        {
            var docs = await search.GetNextSetAsync();
            orders.AddRange(docs.Select(d =>
                JsonSerializer.Deserialize<Order>(d.ToJson())!));
        } while (!search.IsDone);

        return orders;
    }
}
