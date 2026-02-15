import type { Product, CreateProductDto, UpdateProductDto } from '@/api/types/product.types';
import { delay } from './delay';
import { mockCategoryApi } from './categories';

let products: Product[] = [
  { id: '1', name: 'iPhone 16 Pro', description: 'Apple iPhone 16 Pro 256GB', price: 84999, stock: 25, categoryId: '1', categoryName: 'Elektronik', status: 'active', createdAt: '2025-01-20T10:00:00Z' },
  { id: '2', name: 'MacBook Air M3', description: 'Apple MacBook Air 15 inç M3 çip', price: 54999, stock: 12, categoryId: '1', categoryName: 'Elektronik', status: 'active', createdAt: '2025-01-21T10:00:00Z' },
  { id: '3', name: 'Nike Air Max 90', description: 'Erkek spor ayakkabı', price: 4299, stock: 50, categoryId: '4', categoryName: 'Spor', status: 'active', createdAt: '2025-01-22T10:00:00Z' },
  { id: '4', name: 'Slim Fit Gömlek', description: 'Erkek slim fit beyaz gömlek', price: 899, stock: 100, categoryId: '2', categoryName: 'Giyim', status: 'active', createdAt: '2025-01-23T10:00:00Z' },
  { id: '5', name: 'Koltuk Takımı', description: '3+3+1 modern koltuk takımı', price: 34999, stock: 3, categoryId: '3', categoryName: 'Ev & Yaşam', status: 'active', createdAt: '2025-01-24T10:00:00Z' },
  { id: '6', name: 'Kulaklık Sony WH-1000XM5', description: 'Kablosuz gürültü önleyici kulaklık', price: 12499, stock: 0, categoryId: '1', categoryName: 'Elektronik', status: 'inactive', createdAt: '2025-01-25T10:00:00Z' },
  { id: '7', name: 'Roman - Suç ve Ceza', description: 'Dostoyevski klasik eser', price: 89, stock: 200, categoryId: '5', categoryName: 'Kitap', status: 'active', createdAt: '2025-01-26T10:00:00Z' },
  { id: '8', name: 'Yoga Matı', description: 'Kaymaz yoga ve pilates matı', price: 499, stock: 75, categoryId: '4', categoryName: 'Spor', status: 'active', createdAt: '2025-01-27T10:00:00Z' },
  { id: '9', name: 'Parfüm - Chanel No.5', description: 'Kadın parfüm 100ml', price: 8999, stock: 15, categoryId: '6', categoryName: 'Kozmetik', status: 'active', createdAt: '2025-02-01T10:00:00Z' },
  { id: '10', name: 'Lego Technic Set', description: 'Lego Technic yarış arabası seti', price: 2499, stock: 30, categoryId: '7', categoryName: 'Oyuncak', status: 'active', createdAt: '2025-02-02T10:00:00Z' },
];

let nextId = 11;

export const mockProductApi = {
  getAll: async (): Promise<Product[]> => {
    await delay();
    return [...products];
  },

  getById: async (id: string): Promise<Product | undefined> => {
    await delay();
    return products.find((p) => p.id === id);
  },

  create: async (data: CreateProductDto): Promise<Product> => {
    await delay();
    const category = await mockCategoryApi.getById(data.categoryId);
    const newProduct: Product = {
      ...data,
      id: String(nextId++),
      categoryName: category?.name ?? 'Bilinmiyor',
      createdAt: new Date().toISOString(),
    };
    products = [newProduct, ...products];
    return newProduct;
  },

  update: async (id: string, data: UpdateProductDto): Promise<Product> => {
    await delay();
    const index = products.findIndex((p) => p.id === id);
    if (index === -1) throw new Error('Ürün bulunamadı');
    const category = await mockCategoryApi.getById(data.categoryId);
    const updated: Product = {
      ...products[index]!,
      ...data,
      categoryName: category?.name ?? 'Bilinmiyor',
    };
    products[index] = updated;
    return updated;
  },

  delete: async (id: string): Promise<void> => {
    await delay();
    products = products.filter((p) => p.id !== id);
  },
};
