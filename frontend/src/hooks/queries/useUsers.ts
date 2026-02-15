import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { message } from 'antd';
import { userApi } from '@/api/endpoints/users';
import type { CreateUserDto, UpdateUserDto } from '@/api/types/user.types';

const QUERY_KEY = ['users'];

export const useUsers = () => {
  return useQuery({
    queryKey: QUERY_KEY,
    queryFn: userApi.getAll,
  });
};

export const useCreateUser = (onSuccess?: () => void) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CreateUserDto) => userApi.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Kullanıcı başarıyla oluşturuldu');
      onSuccess?.();
    },
    onError: () => {
      message.error('Kullanıcı oluşturulurken bir hata oluştu');
    },
  });
};

export const useUpdateUser = (onSuccess?: () => void) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateUserDto }) =>
      userApi.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Kullanıcı başarıyla güncellendi');
      onSuccess?.();
    },
    onError: () => {
      message.error('Kullanıcı güncellenirken bir hata oluştu');
    },
  });
};

export const useDeleteUser = (onSuccess?: () => void) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => userApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Kullanıcı başarıyla silindi');
      onSuccess?.();
    },
    onError: () => {
      message.error('Kullanıcı silinirken bir hata oluştu');
    },
  });
};
