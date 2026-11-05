import { api } from "../../../lib/api";
import type { UserProfile, UpdateProfileRequest } from "../types/profile.types";

export const profileApi = {
  async getMyProfile(): Promise<UserProfile> {
    const response = await api.get<UserProfile>("/users/me");
    return response.data;
  },

  async updateMyProfile(data: UpdateProfileRequest): Promise<UserProfile> {
    const response = await api.put<UserProfile>("/users/me", data);

    return response.data;
  },
};
