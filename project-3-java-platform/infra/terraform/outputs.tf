output "alb_dns_name" {
  value = aws_lb.main.dns_name
}

output "ecr_repo_url" {
  value = aws_ecr_repository.app.repository_url
}

output "ecs_cluster_name" {
  value = aws_ecs_cluster.main.name
}

output "codedeploy_app_name" {
  value = aws_codedeploy_app.main.name
}
