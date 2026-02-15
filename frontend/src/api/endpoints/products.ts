import type { Product, CreateProductDto, UpdateProductDto } from '@/api/types/product.types';
import { mockProductApi } from '@/mocks/products';
// import axiosInstance from '@/api/axiosInstance';

export const productApi = {
  getAll: (): Promise<Product[]> => {
    return mockProductApi.getAll();
    // return axiosInstance.get<Product[]>('/products').then(res => res.data);
  },

  getById: (id: string): Promise<Product | undefined> => {
    return mockProductApi.getById(id);
    // return axiosInstance.get<Product>(`/products/${id}`).then(res => res.data);
  },

  create: (data: CreateProductDto): Promise<Product> => {
    return mockProductApi.create(data);
    // return axiosInstance.post<Product>('/products', data).then(res => res.data);
  },

  update: (id: string, data: UpdateProductDto): Promise<Product> => {
    return mockProductApi.update(id, data);
    // return axiosInstance.put<Product>(`/products/${id}`, data).then(res => res.data);
  },

  delete: (id: string): Promise<void> => {
    return mockProductApi.delete(id);
    // return axiosInstance.delete(`/products/${id}`).then(res => res.data);
  },
};
