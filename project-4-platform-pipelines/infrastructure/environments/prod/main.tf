terraform {
  required_version = ">= 1.5"

  backend "s3" {
    bucket         = "platform-terraform-state"
    key            = "platform/prod/terraform.tfstate"
    region         = "us-east-1"
    dynamodb_table = "terraform-locks"
    encrypt        = true
  }
}

provider "aws" {
  region = "us-east-1"

  default_tags {
    tags = {
      Project     = "platform"
      Environment = "prod"
      ManagedBy   = "terraform"
    }
  }
}

module "networking" {
  source      = "../../modules/networking"
  name        = "platform"
  environment = "prod"
  vpc_cidr    = "10.1.0.0/16"
}

module "ecs" {
  source      = "../../modules/ecs-cluster"
  name        = "platform"
  environment = "prod"
}
