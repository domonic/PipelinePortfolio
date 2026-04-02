# Project 4 - Java Spring Boot Platform with GitLab CI

Spring Boot 3 application with GitLab CI blue/green deployments on ECS via CodeDeploy.

## Architecture

- **Language**: Java 21 / Spring Boot 3.3
- **Database**: MySQL 8 (RDS) with Flyway migrations
- **Cache**: Redis (ElastiCache)
- **Observability**: OpenTelemetry → X-Ray, Micrometer → CloudWatch
- **Deployment**: ECS Fargate with CodeDeploy blue/green (canary 10% for 5 min)
- **CI/CD**: GitLab CI with SonarQube, OWASP dependency check, Trivy, k6 load tests

## Local Development

```bash
docker compose up --build
```

- API: http://localhost:8080/api/products
- Health: http://localhost:8080/health
- Actuator: http://localhost:8080/actuator

## Pipeline Stages

| Stage | Jobs | Description |
|-------|------|-------------|
| test | unit-tests | Maven verify with JaCoCo 80% coverage gate |
| scan | sonarqube, owasp-dependency-check | Code quality + vulnerability scanning |
| build | build-image | Docker build → Trivy scan → ECR push |
| migrate | flyway-migrate | Flyway database migrations |
| deploy | deploy-dev / deploy-prod | Blue/green via CodeDeploy (prod = manual) |
| verify | health-check, k6-load-test | Post-deploy validation |
| notify | slack notifications | Success/failure alerts |
| scheduled | nightly-regression | Full regression suite on cron |

## Required GitLab CI/CD Variables

| Variable | Description |
|----------|-------------|
| `ECR_REGISTRY_URL` | ECR registry URL |
| `AWS_ACCESS_KEY_ID` | AWS credentials (or use OIDC) |
| `AWS_SECRET_ACCESS_KEY` | AWS credentials |
| `DEV_DB_URL` / `PROD_DB_URL` | JDBC connection strings |
| `DEV_DB_USERNAME` / `PROD_DB_USERNAME` | Database users |
| `DEV_DB_PASSWORD` / `PROD_DB_PASSWORD` | Database passwords |
| `SONAR_HOST_URL` / `SONAR_TOKEN` | SonarQube config |
| `SLACK_WEBHOOK_URL` | Slack notifications |
| `DEV_APP_URL` / `PROD_APP_URL` | Deployed app URLs |
