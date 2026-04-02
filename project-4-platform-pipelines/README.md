# Project 4 - Reusable Pipeline Library & Infrastructure Platform

Shared CI/CD components for GitHub Actions and GitLab CI, plus a Terraform-managed infrastructure repo that uses them.

## Repository Structure

In production, this would be split across multiple repos. For this portfolio, everything
is co-located to show the full picture.

```
├── github-actions-library/     # Would be its own repo: org/pipeline-library
│   ├── .github/workflows/      # Reusable workflows (workflow_call)
│   │   ├── reusable-docker-build.yml
│   │   ├── reusable-terraform.yml
│   │   └── reusable-deploy-ecs.yml
│   └── actions/                # Composite actions
│       ├── docker-build-push/
│       ├── setup-aws-oidc/
│       └── slack-notify/
├── gitlab-ci-templates/        # Would be its own repo: org/gitlab-ci-templates
│   ├── docker.yml
│   ├── terraform.yml
│   ├── deploy-ecs.yml
│   └── security-scanning.yml
├── infrastructure/             # Would be its own repo: org/infrastructure
│   ├── .github/workflows/      # Consumes reusable workflows from pipeline-library
│   ├── environments/
│   │   ├── dev/
│   │   └── prod/
│   └── modules/
│       ├── networking/
│       ├── ecs-cluster/
│       └── rds/
└── policies/                   # OPA/Conftest policies (shared across repos)
    └── opa/
        ├── s3.rego             # No public buckets
        ├── encryption.rego     # Encryption required
        └── tagging.rego        # Mandatory tags
```

The `infrastructure/.github/workflows/terraform.yml` references reusable workflows via
`domonic/pipeline-library/.github/workflows/reusable-terraform.yml@main`. This is the
standard cross-repo pattern — the pipeline-library repo would be published separately
and consumed by any team's infrastructure repo.

## GitHub Actions Library

### Reusable Workflows

Called with `uses: org/repo/.github/workflows/reusable-*.yml@main`

| Workflow | Purpose | Key Features |
|----------|---------|-------------|
| `reusable-docker-build` | Build, scan, push to ECR | Buildx caching, Trivy scan, OIDC auth |
| `reusable-terraform` | Plan, cost estimate, policy check, apply | Infracost, OPA/Conftest, PR comments |
| `reusable-deploy-ecs` | Deploy to ECS Fargate | Task def update, stability wait, health check |

### Composite Actions

| Action | Purpose |
|--------|---------|
| `setup-aws-oidc` | Configure OIDC-based AWS credentials |
| `docker-build-push` | Build + Trivy scan + ECR push (single step) |
| `slack-notify` | Formatted deployment notifications |

## GitLab CI Templates

Included with `include: remote` or `include: project`

| Template | Purpose |
|----------|---------|
| `docker.yml` | `.docker-build-scan-push` template |
| `terraform.yml` | `.terraform-plan`, `.terraform-apply`, `.terraform-drift-check` |
| `deploy-ecs.yml` | `.deploy-ecs` template |
| `security-scanning.yml` | `.sonarqube-analysis`, `.trivy-fs-scan`, `.trivy-config-scan` |

## Infrastructure Pipeline

| Trigger | Action |
|---------|--------|
| PR to main | Terraform plan + Infracost + OPA policy check (both envs) |
| Push to main | Auto-apply dev → then prod (sequential) |
| Cron (weekday 6am) | Drift detection for both environments |

## OPA Policies

- No public S3 buckets
- Encryption required on RDS, EBS, SQS
- Mandatory tags: Project, Environment, ManagedBy

## Dependabot

- Weekly updates for GitHub Actions versions
- Weekly updates for Terraform provider versions
- Auto-grouped PRs by ecosystem
