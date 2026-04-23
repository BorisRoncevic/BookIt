export function buildHeaders(token: string | null): Headers {
  const headers = new Headers();

  headers.append("Content-Type", "application/json");

  if (token) {
    headers.append("Authorization", `Bearer ${token}`);
  }

  return headers;
}

export function getToken(): string | null {
  return localStorage.getItem("token");
}