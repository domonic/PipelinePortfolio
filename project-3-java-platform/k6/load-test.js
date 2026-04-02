import http from "k6/http";
import { check, sleep } from "k6";

const BASE_URL = __ENV.BASE_URL || "http://localhost:8080";

export const options = {
  stages: [
    { duration: "30s", target: 10 },   // ramp up
    { duration: "1m", target: 50 },    // sustained load
    { duration: "30s", target: 100 },  // peak
    { duration: "30s", target: 0 },    // ramp down
  ],
  thresholds: {
    http_req_duration: ["p(95)<500", "p(99)<1000"],
    http_req_failed: ["rate<0.01"],
  },
};

export default function () {
  // Health check
  const healthRes = http.get(`${BASE_URL}/health`);
  check(healthRes, {
    "health status 200": (r) => r.status === 200,
    "health response time < 200ms": (r) => r.timings.duration < 200,
  });

  // List products
  const listRes = http.get(`${BASE_URL}/api/products`);
  check(listRes, {
    "list status 200": (r) => r.status === 200,
    "list response time < 500ms": (r) => r.timings.duration < 500,
  });

  // Create product
  const payload = JSON.stringify({
    name: `Load Test Product ${Date.now()}`,
    description: "Created during k6 load test",
    price: 9.99,
    category: "test",
  });

  const createRes = http.post(`${BASE_URL}/api/products`, payload, {
    headers: { "Content-Type": "application/json" },
  });
  check(createRes, {
    "create status 201": (r) => r.status === 201,
  });

  sleep(1);
}
