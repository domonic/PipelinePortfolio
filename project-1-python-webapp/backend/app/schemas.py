from pydantic import BaseModel, EmailStr
from datetime import datetime


class UserCreate(BaseModel):
    email: str
    password: str
    full_name: str | None = None


class UserResponse(BaseModel):
    id: int
    email: str
    full_name: str | None
    is_active: bool
    created_at: datetime

    model_config = {"from_attributes": True}


class ItemCreate(BaseModel):
    title: str
    description: str | None = None


class ItemResponse(BaseModel):
    id: int
    title: str
    description: str | None
    owner_id: int
    created_at: datetime

    model_config = {"from_attributes": True}


class Token(BaseModel):
    access_token: str
    token_type: str


class HealthResponse(BaseModel):
    status: str
    environment: str
    version: str
