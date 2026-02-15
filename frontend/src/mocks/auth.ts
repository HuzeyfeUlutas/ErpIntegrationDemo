import type { LoginRequest, LoginResponse } from '@/api/types/auth.types';
import { delay } from './delay';

export const mockAuthApi = {
  login: async (data: LoginRequest): Promise<LoginResponse> => {
    await delay(600);

    if (data.email === 'admin@admin.com' && data.password === '123456') {
      return {
        token: 'mock-jwt-token-' + Date.now(),
        user: {
          id: '1',
          name: 'Ahmet Yılmaz',
          email: 'admin@admin.com',
          role: 'admin',
        },
      };
    }

    throw new Error('E-posta veya şifre hatalı');
  },

  me: async (): Promise<LoginResponse['user']> => {
    await delay(200);
    return {
      id: '1',
      name: 'Ahmet Yılmaz',
      email: 'admin@admin.com',
      role: 'admin',
    };
  },
};
