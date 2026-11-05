import { authApi } from "./authApi";
import { useAuthStore } from "../../../stores/authStore";

export async function initializeAuth() {
  const store = useAuthStore.getState();

  const location = window.location.pathname;
  if (location === "/login" || location === "/register") {
    store.setInitialized(true);
    return;
  }

  try {
    const response = await authApi.refresh();

    if (!response.accessToken) {
      throw new Error("Refresh response did not contain an access token.");
    }

    store.setAuth(
      {
        userId: response.userId,
        email: response.email,
      },
      response.accessToken,
    );
  } catch {
    store.clearAuth();
  } finally {
    store.setInitialized(true);
  }
}
