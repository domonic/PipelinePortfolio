# Project 1 - Full-Stack Python Web App with GitHub Actions CI/CD

FastAPI backend + React frontend deployed to AWS ECS Fargate via GitHub Actions.

## Architecture

- **Backend**: Python 3.12 / FastAPI / SQLAlchemy / Alembic
- **Frontend**: React 18 / TypeScript / Vite
- **Database**: PostgreSQL 16 (RDS)
- **Infrastructure**: ECS Fargate / ALB / ECR / Secrets Manager
- **CI/CD**: GitHub Actions with OIDC authentication

## Local Development

```bash
docker compose up --build
```

- Backend: http://localhost:8000/docs
- Frontend: http://localhost:3000

## CI/CD Pipelines

| Workflow | Trigger | What it does |
|----------|---------|-------------|
| `ci.yml` | PR to main/develop | Lint, test, security scan (Bandit), SonarQube quality gate, Docker build, Trivy container scan |
| `cd-dev.yml` | Push to develop | Build → Trivy scan → ECR push → Migrate DB → Deploy ECS → Health check |
| `cd-prod.yml` | Manual dispatch | Approval gate → Migrate → Deploy → Health check → Auto-rollback |

## Required GitHub Secrets

| Secret | Description |
|--------|-------------|
| `AWS_DEPLOY_ROLE_ARN` | IAM role ARN for OIDC (dev) |
| `AWS_PROD_ROLE_ARN` | IAM role ARN for OIDC (prod) |
| `DEV_DATABASE_URL` | Dev PostgreSQL connection string |
| `PROD_DATABASE_URL` | Prod PostgreSQL connection string |
| `SLACK_WEBHOOK_URL` | Slack incoming webhook |
| `SONAR_TOKEN` | SonarQube authentication token |

## Required GitHub Variables

| Variable | Description |
|----------|-------------|
| `ECR_REGISTRY` | ECR registry URL (e.g., 123456789.dkr.ecr.us-east-1.amazonaws.com) |
| `SONAR_HOST_URL` | SonarQube server URL |
| `DEV_BACKEND_URL` | Dev environment backend URL |
| `PROD_BACKEND_URL` | Prod environment backend URL |

## Running Tests

```bash
# Backend
cd backend && pip install -r requirements-dev.txt && pytest -v

# Frontend
cd frontend && npm ci && npm test
```
