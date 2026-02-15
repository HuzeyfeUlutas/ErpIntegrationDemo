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
      setAuth(response.token, response.user);
      message.success(`Hoş geldiniz, ${response.user.name}!`);
      navigate('/', { replace: true });
    },
    onError: (error: Error) => {
      message.error(error.message || 'Giriş başarısız');
    },
  });
};

export const useLogout = () => {
  const navigate = useNavigate();
  const logout = useAuthStore((s) => s.logout);

  return () => {
    logout();
    message.info('Çıkış yapıldı');
    navigate('/login', { replace: true });
  };
};
