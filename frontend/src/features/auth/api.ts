export const BASE_URL = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5080/api";
const AUTH_URL = `${BASE_URL}/auth`;

export async function login(data:unknown) {
  const res = await fetch(`${AUTH_URL}/login`, {
    method: "POST",
    headers : {
        "Content-Type" : "application/json"
    },
    body : JSON.stringify(data)

});
  if (!res.ok) {
    throw new Error("Login failed");
  }

  const result = await res.json();
  localStorage.setItem("token",result.token);
  localStorage.setItem("username",result.username)
}

export async function register(data: unknown) {
  console.log("Saljem na backend:", data);

  const res = await fetch(`${AUTH_URL}/register`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(data),
  });

  const text = await res.text();

  if (!res.ok) {
    throw new Error(text || "Register failed");
  }

  try {
    return text ? JSON.parse(text) : null;
  } catch {
    return text;
  }
}

export async function logout() {
  localStorage.removeItem("token");
}
