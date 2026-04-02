def _get_auth_header(client):
    client.post("/auth/register", json={
        "email": "items@example.com",
        "password": "pass123",
    })
    resp = client.post("/auth/login", json={
        "email": "items@example.com",
        "password": "pass123",
    })
    token = resp.json()["access_token"]
    return {"Authorization": f"Bearer {token}"}


def test_create_item(client):
    headers = _get_auth_header(client)
    response = client.post("/items", json={
        "title": "Test Item",
        "description": "A test item",
    }, headers=headers)
    assert response.status_code == 201
    assert response.json()["title"] == "Test Item"


def test_list_items(client):
    headers = _get_auth_header(client)
    client.post("/items", json={"title": "Item 1"}, headers=headers)
    client.post("/items", json={"title": "Item 2"}, headers=headers)
    response = client.get("/items", headers=headers)
    assert response.status_code == 200
    assert len(response.json()) == 2


def test_items_requires_auth(client):
    response = client.get("/items")
    assert response.status_code == 403
