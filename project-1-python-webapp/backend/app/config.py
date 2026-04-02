from pydantic_settings import BaseSettings
from functools import lru_cache


class Settings(BaseSettings):
    app_name: str = "Project 1 API"
    environment: str = "development"
    debug: bool = False

    # Database
    database_url: str = "postgresql://postgres:postgres@localhost:5432/app_db"

    # Auth
    secret_key: str = "change-me-in-production"
    algorithm: str = "HS256"
    access_token_expire_minutes: int = 30

    # CORS
    allowed_origins: str = "http://localhost:3000"

    model_config = {"env_prefix": "APP_"}


@lru_cache
def get_settings() -> Settings:
    return Settings()
