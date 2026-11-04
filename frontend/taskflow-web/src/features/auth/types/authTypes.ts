export interface AuthResponse {
  userId: string;
  email: string;
  accessToken: string;
  accessTokenExpiresAtUtc: string;
}

export interface AuthUser {
  userId: string;
  email: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}
