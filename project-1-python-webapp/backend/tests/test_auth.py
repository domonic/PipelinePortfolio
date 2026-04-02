def test_register_user(client):
    response = client.post("/auth/register", json={
        "email": "test@example.com",
        "password": "securepassword123",
        "full_name": "Test User",
    })
    assert response.status_code == 201
    data = response.json()
    assert data["email"] == "test@example.com"
    assert "id" in data


def test_register_duplicate_email(client):
    payload = {"email": "dup@example.com", "password": "pass123"}
    client.post("/auth/register", json=payload)
    response = client.post("/auth/register", json=payload)
    assert response.status_code == 400


def test_login_success(client):
    client.post("/auth/register", json={
        "email": "login@example.com",
        "password": "pass123",
    })
    response = client.post("/auth/login", json={
        "email": "login@example.com",
        "password": "pass123",
    })
    assert response.status_code == 200
    assert "access_token" in response.json()


def test_login_invalid_credentials(client):
    response = client.post("/auth/login", json={
        "email": "nobody@example.com",
        "password": "wrong",
    })
    assert response.status_code == 401
