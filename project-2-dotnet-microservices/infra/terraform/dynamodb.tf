resource "aws_dynamodb_table" "orders" {
  name         = "orders-${var.environment}"
  billing_mode = "PAY_PER_REQUEST"
  hash_key     = "Id"

  attribute {
    name = "Id"
    type = "S"
  }

  attribute {
    name = "CustomerId"
    type = "S"
  }

  global_secondary_index {
    name            = "CustomerId-index"
    hash_key        = "CustomerId"
    projection_type = "ALL"
  }

  point_in_time_recovery {
    enabled = true
  }
}

resource "aws_dynamodb_table" "inventory" {
  name         = "inventory-${var.environment}"
  billing_mode = "PAY_PER_REQUEST"
  hash_key     = "ProductId"

  attribute {
    name = "ProductId"
    type = "S"
  }

  point_in_time_recovery {
    enabled = true
  }
}
