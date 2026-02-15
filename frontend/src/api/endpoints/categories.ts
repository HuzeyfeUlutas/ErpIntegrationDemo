import type { Category, CreateCategoryDto, UpdateCategoryDto } from '@/api/types/category.types';
import { mockCategoryApi } from '@/mocks/categories';
// import axiosInstance from '@/api/axiosInstance';

export const categoryApi = {
  getAll: (): Promise<Category[]> => {
    return mockCategoryApi.getAll();
    // return axiosInstance.get<Category[]>('/categories').then(res => res.data);
  },

  getById: (id: string): Promise<Category | undefined> => {
    return mockCategoryApi.getById(id);
    // return axiosInstance.get<Category>(`/categories/${id}`).then(res => res.data);
  },

  create: (data: CreateCategoryDto): Promise<Category> => {
    return mockCategoryApi.create(data);
    // return axiosInstance.post<Category>('/categories', data).then(res => res.data);
  },

  update: (id: string, data: UpdateCategoryDto): Promise<Category> => {
    return mockCategoryApi.update(id, data);
    // return axiosInstance.put<Category>(`/categories/${id}`, data).then(res => res.data);
  },

  delete: (id: string): Promise<void> => {
    return mockCategoryApi.delete(id);
    // return axiosInstance.delete(`/categories/${id}`).then(res => res.data);
  },
};
