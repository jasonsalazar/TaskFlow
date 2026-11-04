import { api } from "../../../lib/api";
import type {
  AuthResponse,
  LoginRequest,
  RegisterRequest,
} from "../types/authTypes";

export const authApi = {
  async login(data: LoginRequest): Promise<AuthResponse> {
    const response = await api.post<AuthResponse>("/auth/login", data);

    return response.data;
  },

  async register(data: RegisterRequest): Promise<AuthResponse> {
    const response = await api.post<AuthResponse>("/auth/register", data);

    return response.data;
  },

  async refresh(): Promise<AuthResponse> {
    const response = await api.post<AuthResponse>("/auth/refresh");

    return response.data;
  },

  async logout(): Promise<void> {
    await api.post("/auth/logout");
  },
};
