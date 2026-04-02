import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../api";

interface Item {
  id: number;
  title: string;
  description: string | null;
}

export default function Dashboard() {
  const [items, setItems] = useState<Item[]>([]);
  const [title, setTitle] = useState("");
  const navigate = useNavigate();

  useEffect(() => {
    api.get("/items").then((r) => setItems(r.data)).catch(() => navigate("/login"));
  }, [navigate]);

  const addItem = async () => {
    if (!title.trim()) return;
    const { data } = await api.post("/items", { title });
    setItems((prev) => [...prev, data]);
    setTitle("");
  };

  const logout = () => {
    localStorage.removeItem("token");
    navigate("/login");
  };

  return (
    <main style={{ maxWidth: 600, margin: "40px auto", padding: 24 }}>
      <header style={{ display: "flex", justifyContent: "space-between" }}>
        <h1>Dashboard</h1>
        <button onClick={logout}>Logout</button>
      </header>
      <section>
        <h2>My Items</h2>
        <div style={{ display: "flex", gap: 8, marginBottom: 16 }}>
          <label htmlFor="new-item" className="sr-only">New item title</label>
          <input
            id="new-item"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Item title"
            style={{ flex: 1 }}
          />
          <button onClick={addItem}>Add</button>
        </div>
        <ul>
          {items.map((item) => (
            <li key={item.id}>{item.title}</li>
          ))}
        </ul>
      </section>
    </main>
  );
}
