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

export function getCurrentUserId(): string | null {
  const token = getToken();
  if (!token) return null;

  try {
    const [, payload] = token.split(".");
    if (!payload) return null;

    const normalizedPayload = payload
      .replace(/-/g, "+")
      .replace(/_/g, "/")
      .padEnd(Math.ceil(payload.length / 4) * 4, "=");
    const decodedPayload = JSON.parse(window.atob(normalizedPayload));

    return (
      decodedPayload.nameid ??
      decodedPayload.sub ??
      decodedPayload[
        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
      ] ??
      null
    );
  } catch {
    return null;
  }
}
