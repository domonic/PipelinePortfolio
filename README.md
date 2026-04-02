# Pipeline Portfolio

CI/CD pipeline implementations across multiple tech stacks, CI systems, and AWS deployment patterns.

## Projects

| # | Project | CI System | Language | Deploy Target | Key Patterns |
|---|---------|-----------|----------|---------------|-------------|
| 1 | [Python Web App](project-1-python-webapp/) | GitHub Actions | Python / TypeScript | ECS Fargate | Multi-stage Docker, OIDC auth, auto-rollback |
| 2 | [.NET Microservices](project-2-dotnet-microservices/) | GitLab CI | C# .NET 8 | EKS via Helm | Monorepo change detection, parent/child pipelines, SQS async messaging, IRSA |
| 3 | [Java Platform](project-3-java-platform/) | GitLab CI | Java 21 | ECS Blue/Green | CodeDeploy canary (10%/5min), load testing, OpenTelemetry |
| 4 | [Platform Pipelines](project-4-platform-pipelines/) | Both | Terraform / Rego | Multi-environment | Reusable workflows, CI templates, OPA policy-as-code, drift detection |

## Tech Stack

**CI/CD Systems**: GitHub Actions, GitLab CI

**Languages**: Python, C#, Java, TypeScript

**AWS Services**: ECS Fargate, EKS, ECR, ALB, RDS, DynamoDB, SQS, ElastiCache, Secrets Manager, CodeDeploy, CloudWatch, X-Ray, IAM (OIDC)

**Infrastructure**: Terraform (modules, remote state, multi-environment), Helm, Docker

**Security & Quality**: Trivy (container scanning), Bandit (Python SAST), SonarQube (quality gates), OWASP Dependency Check, OPA/Conftest (policy-as-code)

**Observability**: OpenTelemetry, AWS X-Ray, Micrometer, CloudWatch

## Pipeline Patterns Covered

- Environment promotion with manual approval gates
- OIDC federation (no long-lived AWS credentials)
- Automated rollback on health check failure
- Blue/green deployments with canary traffic shifting
- Monorepo change detection (build only what changed)
- Reusable workflows and CI templates
- Container image scanning before registry push
- Policy-as-code enforcement (OPA)
- Scheduled drift detection
- Post-deploy load testing 
- Slack notifications on deploy success/failure

## Repository Structure

```
.
├── project-1-python-webapp/          # FastAPI + React -> GitHub Actions -> ECS Fargate
├── project-2-dotnet-microservices/   # Orders + Inventory APIs -> GitLab CI -> EKS
├── project-3-java-platform/         # Spring Boot -> GitLab CI -> ECS Blue/Green
└── project-4-platform-pipelines/    # Reusable pipeline library + Terraform platform
```

Each project has its own README with architecture details, pipeline documentation, and setup instructions.
