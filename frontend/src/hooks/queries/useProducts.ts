import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { message } from 'antd';
import { productApi } from '@/api/endpoints/products';
import type { CreateProductDto, UpdateProductDto } from '@/api/types/product.types';

const QUERY_KEY = ['products'];

export const useProducts = () => {
  return useQuery({
    queryKey: QUERY_KEY,
    queryFn: productApi.getAll,
  });
};

export const useCreateProduct = (onSuccess?: () => void) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CreateProductDto) => productApi.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Ürün başarıyla oluşturuldu');
      onSuccess?.();
    },
    onError: () => {
      message.error('Ürün oluşturulurken bir hata oluştu');
    },
  });
};

export const useUpdateProduct = (onSuccess?: () => void) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateProductDto }) =>
      productApi.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Ürün başarıyla güncellendi');
      onSuccess?.();
    },
    onError: () => {
      message.error('Ürün güncellenirken bir hata oluştu');
    },
  });
};

export const useDeleteProduct = (onSuccess?: () => void) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => productApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEY });
      message.success('Ürün başarıyla silindi');
      onSuccess?.();
    },
    onError: () => {
      message.error('Ürün silinirken bir hata oluştu');
    },
  });
};
