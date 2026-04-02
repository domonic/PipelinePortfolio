# CloudWatch alarm used by CodeDeploy for auto-rollback
resource "aws_cloudwatch_metric_alarm" "high_5xx" {
  alarm_name          = "project4-${var.environment}-high-5xx"
  comparison_operator = "GreaterThanThreshold"
  evaluation_periods  = 2
  metric_name         = "HTTPCode_Target_5XX_Count"
  namespace           = "AWS/ApplicationELB"
  period              = 60
  statistic           = "Sum"
  threshold           = 10
  alarm_description   = "Triggers CodeDeploy rollback on high 5xx rate"

  dimensions = {
    LoadBalancer = aws_lb.main.arn_suffix
  }
}

resource "aws_cloudwatch_log_group" "ecs" {
  name              = "/ecs/project4-platform-${var.environment}"
  retention_in_days = 30
}

resource "aws_cloudwatch_log_group" "otel" {
  name              = "/otel/project4-platform-${var.environment}"
  retention_in_days = 14
}
