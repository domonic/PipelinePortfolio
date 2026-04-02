resource "aws_sqs_queue" "inventory" {
  name                        = "project2-inventory-${var.environment}.fifo"
  fifo_queue                  = true
  content_based_deduplication = true
  visibility_timeout_seconds  = 60
  message_retention_seconds   = 86400
  receive_wait_time_seconds   = 20

  redrive_policy = jsonencode({
    deadLetterTargetArn = aws_sqs_queue.inventory_dlq.arn
    maxReceiveCount     = 3
  })
}

resource "aws_sqs_queue" "inventory_dlq" {
  name                      = "project2-inventory-dlq-${var.environment}.fifo"
  fifo_queue                = true
  message_retention_seconds = 1209600  # 14 days
}
