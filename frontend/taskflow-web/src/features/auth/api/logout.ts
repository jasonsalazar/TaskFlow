import { authApi } from "./authApi";
import { useAuthStore } from "../../../stores/authStore";

export async function logout() {
  try {
    await authApi.logout();
  } finally {
    useAuthStore.getState().clearAuth();
  }
}
