from fastapi import FastAPI, Depends, HTTPException, status
from fastapi.middleware.cors import CORSMiddleware
from sqlalchemy.orm import Session

from app.config import get_settings
from app.database import get_db
from app.models import User, Item
from app.schemas import (
    UserCreate, UserResponse, ItemCreate, ItemResponse,
    Token, HealthResponse,
)
from app.auth import hash_password, verify_password, create_access_token, get_current_user

settings = get_settings()

app = FastAPI(title=settings.app_name, docs_url="/docs", redoc_url="/redoc")

app.add_middleware(
    CORSMiddleware,
    allow_origins=settings.allowed_origins.split(","),
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)


@app.get("/health", response_model=HealthResponse)
def health_check():
    return HealthResponse(
        status="healthy",
        environment=settings.environment,
        version="1.0.0",
    )


@app.post("/auth/register", response_model=UserResponse, status_code=201)
def register(user_in: UserCreate, db: Session = Depends(get_db)):
    if db.query(User).filter(User.email == user_in.email).first():
        raise HTTPException(status_code=400, detail="Email already registered")
    user = User(
        email=user_in.email,
        hashed_password=hash_password(user_in.password),
        full_name=user_in.full_name,
    )
    db.add(user)
    db.commit()
    db.refresh(user)
    return user


@app.post("/auth/login", response_model=Token)
def login(user_in: UserCreate, db: Session = Depends(get_db)):
    user = db.query(User).filter(User.email == user_in.email).first()
    if not user or not verify_password(user_in.password, user.hashed_password):
        raise HTTPException(status_code=401, detail="Invalid credentials")
    token = create_access_token(data={"sub": str(user.id)})
    return Token(access_token=token, token_type="bearer")


@app.get("/items", response_model=list[ItemResponse])
def list_items(db: Session = Depends(get_db), current_user: User = Depends(get_current_user)):
    return db.query(Item).filter(Item.owner_id == current_user.id).all()


@app.post("/items", response_model=ItemResponse, status_code=201)
def create_item(
    item_in: ItemCreate,
    db: Session = Depends(get_db),
    current_user: User = Depends(get_current_user),
):
    item = Item(title=item_in.title, description=item_in.description, owner_id=current_user.id)
    db.add(item)
    db.commit()
    db.refresh(item)
    return item
