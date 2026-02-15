import type { Category, CreateCategoryDto, UpdateCategoryDto } from '@/api/types/category.types';
import { delay } from './delay';

let categories: Category[] = [
  { id: '1', name: 'Elektronik', description: 'Telefon, bilgisayar ve diğer elektronik ürünler', status: 'active', createdAt: '2025-01-15T10:00:00Z' },
  { id: '2', name: 'Giyim', description: 'Erkek, kadın ve çocuk giyim ürünleri', status: 'active', createdAt: '2025-01-16T10:00:00Z' },
  { id: '3', name: 'Ev & Yaşam', description: 'Mobilya, dekorasyon ve ev gereçleri', status: 'active', createdAt: '2025-01-17T10:00:00Z' },
  { id: '4', name: 'Spor', description: 'Spor ekipmanları ve giyim', status: 'inactive', createdAt: '2025-01-18T10:00:00Z' },
  { id: '5', name: 'Kitap', description: 'Roman, akademik ve çocuk kitapları', status: 'active', createdAt: '2025-01-19T10:00:00Z' },
  { id: '6', name: 'Kozmetik', description: 'Cilt bakım, makyaj ve parfüm ürünleri', status: 'active', createdAt: '2025-02-01T10:00:00Z' },
  { id: '7', name: 'Oyuncak', description: 'Çocuk oyuncakları ve eğitici materyaller', status: 'active', createdAt: '2025-02-05T10:00:00Z' },
  { id: '8', name: 'Otomotiv', description: 'Araç aksesuarları ve yedek parçalar', status: 'inactive', createdAt: '2025-02-10T10:00:00Z' },
];

let nextId = 9;

export const mockCategoryApi = {
  getAll: async (): Promise<Category[]> => {
    await delay();
    return [...categories];
  },

  getById: async (id: string): Promise<Category | undefined> => {
    await delay();
    return categories.find((c) => c.id === id);
  },

  create: async (data: CreateCategoryDto): Promise<Category> => {
    await delay();
    const newCategory: Category = {
      ...data,
      id: String(nextId++),
      createdAt: new Date().toISOString(),
    };
    categories = [newCategory, ...categories];
    return newCategory;
  },

  update: async (id: string, data: UpdateCategoryDto): Promise<Category> => {
    await delay();
    const index = categories.findIndex((c) => c.id === id);
    if (index === -1) throw new Error('Kategori bulunamadı');
    const updated = { ...categories[index]!, ...data };
    categories[index] = updated;
    return updated;
  },

  delete: async (id: string): Promise<void> => {
    await delay();
    categories = categories.filter((c) => c.id !== id);
  },
};
