variable "name" { type = string }
variable "environment" { type = string }
variable "vpc_id" { type = string }
variable "subnet_ids" { type = list(string) }
variable "instance_class" { type = string }
variable "engine" { type = string }
variable "engine_version" { type = string }
variable "allowed_security_groups" { type = list(string) }

resource "aws_db_subnet_group" "this" {
  name       = "${var.name}-${var.environment}"
  subnet_ids = var.subnet_ids
}

resource "aws_security_group" "this" {
  name_prefix = "${var.name}-rds-"
  vpc_id      = var.vpc_id

  ingress {
    from_port       = var.engine == "mysql" ? 3306 : 5432
    to_port         = var.engine == "mysql" ? 3306 : 5432
    protocol        = "tcp"
    security_groups = var.allowed_security_groups
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

resource "aws_db_instance" "this" {
  identifier     = "${var.name}-${var.environment}"
  engine         = var.engine
  engine_version = var.engine_version
  instance_class = var.instance_class

  allocated_storage     = 20
  max_allocated_storage = 100
  storage_encrypted     = true

  db_subnet_group_name   = aws_db_subnet_group.this.name
  vpc_security_group_ids = [aws_security_group.this.id]

  backup_retention_period = var.environment == "prod" ? 7 : 1
  multi_az                = var.environment == "prod"
  deletion_protection     = var.environment == "prod"
  skip_final_snapshot     = var.environment != "prod"

  tags = { Name = "${var.name}-${var.environment}" }
}

output "endpoint" { value = aws_db_instance.this.endpoint }
output "security_group_id" { value = aws_security_group.this.id }
