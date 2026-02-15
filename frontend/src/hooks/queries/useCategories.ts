import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { message } from 'antd';
import { categoryApi } from '@/api/endpoints/categories';
import type { CreateCategoryDto, UpdateCategoryDto } from '@/api/types/category.types';

const QUERY_KEY = ['categories'];

export const useCategories = () => {
  return useQuery({
    queryKey: QUERY_KEY,
    queryFn: categoryApi.getAll,
  });
};

export const useCreateCategory = (onSuccess?: () => void) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CreateCategoryDto) => categoryApi.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Kategori başarıyla oluşturuldu');
      onSuccess?.();
    },
    onError: () => {
      message.error('Kategori oluşturulurken bir hata oluştu');
    },
  });
};

export const useUpdateCategory = (onSuccess?: () => void) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateCategoryDto }) =>
      categoryApi.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Kategori başarıyla güncellendi');
      onSuccess?.();
    },
    onError: () => {
      message.error('Kategori güncellenirken bir hata oluştu');
    },
  });
};

export const useDeleteCategory = (onSuccess?: () => void) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => categoryApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Kategori başarıyla silindi');
      onSuccess?.();
    },
    onError: () => {
      message.error('Kategori silinirken bir hata oluştu');
    },
  });
};
