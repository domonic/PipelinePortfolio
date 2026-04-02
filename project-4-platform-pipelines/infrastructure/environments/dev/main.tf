terraform {
  required_version = ">= 1.5"

  backend "s3" {
    bucket         = "platform-terraform-state"
    key            = "platform/dev/terraform.tfstate"
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
      Environment = "dev"
      ManagedBy   = "terraform"
    }
  }
}

module "networking" {
  source      = "../../modules/networking"
  name        = "platform"
  environment = "dev"
  vpc_cidr    = "10.0.0.0/16"
}

module "ecs" {
  source      = "../../modules/ecs-cluster"
  name        = "platform"
  environment = "dev"
}
