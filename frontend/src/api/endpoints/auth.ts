import type { LoginRequest, LoginResponse, AuthUser } from '@/api/types/auth.types';
import { mockAuthApi } from '@/mocks/auth';
// import axiosInstance from '@/api/axiosInstance';

// Mock implementasyon - API hazır olduğunda axios ile değiştir
export const authApi = {
  login: (data: LoginRequest): Promise<LoginResponse> => {
    return mockAuthApi.login(data);
    // return axiosInstance.post<LoginResponse>('/auth/login', data).then(res => res.data);
  },

  me: (): Promise<AuthUser> => {
    return mockAuthApi.me();
    // return axiosInstance.get<AuthUser>('/auth/me').then(res => res.data);
  },
};
