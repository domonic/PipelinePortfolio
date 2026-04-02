using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace InventoryApi.Services;

public class SqsConsumerService : BackgroundService
{
    private readonly IAmazonSQS _sqs;
    private readonly InventoryService _inventoryService;
    private readonly IConfiguration _config;
    private readonly ILogger<SqsConsumerService> _logger;

    public SqsConsumerService(
        IAmazonSQS sqs,
        InventoryService inventoryService,
        IConfiguration config,
        ILogger<SqsConsumerService> logger)
    {
        _sqs = sqs;
        _inventoryService = inventoryService;
        _config = config;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueUrl = _config["AWS:InventoryQueueUrl"] ?? "";
        _logger.LogInformation("Starting SQS consumer for queue: {QueueUrl}", queueUrl);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var response = await _sqs.ReceiveMessageAsync(new ReceiveMessageRequest
                {
                    QueueUrl = queueUrl,
                    MaxNumberOfMessages = 10,
                    WaitTimeSeconds = 20
                }, stoppingToken);

                foreach (var message in response.Messages)
                {
                    await ProcessMessageAsync(message, queueUrl);
                }
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing SQS messages");
                await Task.Delay(5000, stoppingToken);
            }
        }
    }

    private async Task ProcessMessageAsync(Message message, string queueUrl)
    {
        try
        {
            using var doc = JsonDocument.Parse(message.Body);
            var root = doc.RootElement;

            var orderId = root.GetProperty("Id").GetString() ?? "";
            var items = root.GetProperty("Items");

            foreach (var item in items.EnumerateArray())
            {
                var request = new Models.ReserveInventoryRequest
                {
                    OrderId = orderId,
                    ProductId = item.GetProperty("ProductId").GetString() ?? "",
                    Quantity = item.GetProperty("Quantity").GetInt32()
                };

                await _inventoryService.ReserveInventoryAsync(request);
            }

            await _sqs.DeleteMessageAsync(queueUrl, message.ReceiptHandle);
            _logger.LogInformation("Processed order {OrderId} from SQS", orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process message {MessageId}", message.MessageId);
        }
    }
}
