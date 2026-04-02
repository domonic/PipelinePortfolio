output "eks_cluster_name" {
  value = aws_eks_cluster.main.name
}

output "eks_cluster_endpoint" {
  value = aws_eks_cluster.main.endpoint
}

output "ecr_orders_url" {
  value = aws_ecr_repository.orders.repository_url
}

output "ecr_inventory_url" {
  value = aws_ecr_repository.inventory.repository_url
}

output "sqs_inventory_queue_url" {
  value = aws_sqs_queue.inventory.url
}

output "orders_pod_role_arn" {
  value = aws_iam_role.orders_pod.arn
}
