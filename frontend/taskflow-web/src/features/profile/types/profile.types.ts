export interface UserProfile {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  avatarUrl: string | null;
  createdAtUtc: string;
}

export interface UpdateProfileRequest {
  firstName: string;
  lastName: string;
  avatarUrl: string | null;
}
