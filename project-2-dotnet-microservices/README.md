# Project 2 - .NET Microservices with GitLab CI

Two .NET 8 microservices (Orders + Inventory) communicating via SQS, deployed to EKS with Helm via GitLab CI monorepo pipelines.

## Architecture

- **Language**: C# / .NET 8
- **Services**: Orders API, Inventory API
- **Messaging**: SQS FIFO queue with dead-letter queue
- **Database**: DynamoDB (per-service tables)
- **Orchestration**: EKS with Helm charts
- **CI/CD**: GitLab CI with parent/child pipelines and monorepo change detection

## Service Communication

```
Orders API → SQS FIFO Queue → Inventory API (BackgroundService consumer)
```

When an order is created, the Orders API publishes to SQS. The Inventory API's `SqsConsumerService` polls the queue and reserves inventory for each item.

## Pipeline Architecture

```
.gitlab-ci.yml (parent)
├── services/orders-api/.gitlab-ci.yml (child - triggered on orders-api changes)
└── services/inventory-api/.gitlab-ci.yml (child - triggered on inventory-api changes)
```

Both child pipelines use shared templates from `shared/gitlab-templates/`:
- `dotnet-build.yml` — test, build, deploy templates
- `security.yml` — SonarQube and Trivy templates

### Monorepo Change Detection

The parent pipeline uses GitLab's `changes` rules. If only `services/orders-api/` files changed, only the orders pipeline runs. Changes to `shared/` trigger both.

## Pipeline Stages (per service)

| Stage | Job | Description |
|-------|-----|-------------|
| test | dotnet test | xUnit tests with 70% coverage threshold |
| scan | sonarqube, trivy | Code quality + filesystem vulnerability scan |
| build | docker build | Build → Trivy image scan → ECR push |
| deploy | helm upgrade | Rolling deploy to EKS namespace |
| verify | health-check | Post-deploy /health validation |

## Required GitLab CI/CD Variables

| Variable | Description |
|----------|-------------|
| `ECR_REGISTRY_URL` | ECR registry URL |
| `AWS_ACCESS_KEY_ID` / `AWS_SECRET_ACCESS_KEY` | AWS credentials |
| `SONAR_HOST_URL` / `SONAR_TOKEN` | SonarQube config |
| `ORDERS_DEV_ROLE_ARN` / `ORDERS_PROD_ROLE_ARN` | IRSA roles for Orders pods |
| `INVENTORY_DEV_ROLE_ARN` / `INVENTORY_PROD_ROLE_ARN` | IRSA roles for Inventory pods |
| `DEV_ORDERS_URL` / `DEV_INVENTORY_URL` | Dev environment URLs |
