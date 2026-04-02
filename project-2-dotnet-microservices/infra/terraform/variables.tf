variable "aws_region" {
  type    = string
  default = "us-east-1"
}

variable "environment" {
  type = string
}

variable "vpc_cidr" {
  type    = string
  default = "10.2.0.0/16"
}

variable "eks_node_instance_type" {
  type    = string
  default = "t3.medium"
}

variable "eks_desired_capacity" {
  type    = number
  default = 2
}
