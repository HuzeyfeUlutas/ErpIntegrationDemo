import type { LoginRequest, AuthResponse } from '@/api/types/auth.types';
import axiosInstance from '@/api/axiosInstance';

export const authApi = {
  login: (data: LoginRequest): Promise<AuthResponse> => {
    return axiosInstance
        .post<AuthResponse>('/auth/login', data)
        .then((res) => res.data);
  },

  refresh: (refreshToken: string): Promise<AuthResponse> => {
    return axiosInstance
        .post<AuthResponse>('/auth/refresh', { refreshToken })
        .then((res) => res.data);
  },

  logout: (refreshToken: string): Promise<void> => {
    return axiosInstance
        .post('/auth/logout', { refreshToken })
        .then(() => undefined);
  },
};