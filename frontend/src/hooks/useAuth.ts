import { useMutation } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { message } from 'antd';
import { authApi } from '@/api/endpoints/auth';
import { useAuthStore } from '@/store/useAuthStore';
import type { LoginRequest } from '@/api/types/auth.types';

export const useLogin = () => {
  const navigate = useNavigate();
  const setAuth = useAuthStore((s) => s.setAuth);

  return useMutation({
    mutationFn: (data: LoginRequest) => authApi.login(data),
    onSuccess: (response) => {
      setAuth(response);
      message.success(`Hoş geldiniz, ${response.fullName}!`);
      navigate('/', { replace: true });
    },
    onError: () => {
      message.error('Sicil numarası veya şifre hatalı');
    },
  });
};

export const useLogout = () => {
  const navigate = useNavigate();
  const { refreshToken, logout } = useAuthStore.getState();

  return async () => {
    try {
      if (refreshToken) {
        await authApi.logout(refreshToken);
      }
    } catch {
      // Logout API hatası olsa bile local state'i temizle
    } finally {
      logout();
      message.info('Çıkış yapıldı');
      navigate('/login', { replace: true });
    }
  };
};