import type { User, CreateUserDto, UpdateUserDto } from '@/api/types/user.types';
import { delay } from './delay';

let users: User[] = [
  { id: '1', name: 'Ahmet Yılmaz', email: 'ahmet@example.com', phone: '0532 111 2233', role: 'admin', tags: ['management', 'backend'], status: 'active', createdAt: '2025-01-10T10:00:00Z' },
  { id: '2', name: 'Elif Demir', email: 'elif@example.com', phone: '0533 222 3344', role: 'editor', tags: ['frontend', 'design'], status: 'active', createdAt: '2025-01-11T10:00:00Z' },
  { id: '3', name: 'Mehmet Kaya', email: 'mehmet@example.com', phone: '0534 333 4455', role: 'viewer', tags: ['marketing'], status: 'active', createdAt: '2025-01-12T10:00:00Z' },
  { id: '4', name: 'Ayşe Çelik', email: 'ayse@example.com', phone: '0535 444 5566', role: 'editor', tags: ['frontend', 'backend'], status: 'active', createdAt: '2025-01-13T10:00:00Z' },
  { id: '5', name: 'Can Özkan', email: 'can@example.com', phone: '0536 555 6677', role: 'viewer', tags: ['devops'], status: 'inactive', createdAt: '2025-01-14T10:00:00Z' },
  { id: '6', name: 'Zeynep Arslan', email: 'zeynep@example.com', phone: '0537 666 7788', role: 'admin', tags: ['management', 'frontend', 'backend'], status: 'active', createdAt: '2025-01-15T10:00:00Z' },
  { id: '7', name: 'Burak Şahin', email: 'burak@example.com', phone: '0538 777 8899', role: 'editor', tags: ['design', 'marketing'], status: 'active', createdAt: '2025-01-16T10:00:00Z' },
  { id: '8', name: 'Selin Koç', email: 'selin@example.com', phone: '0539 888 9900', role: 'viewer', tags: ['frontend'], status: 'inactive', createdAt: '2025-01-17T10:00:00Z' },
];

let nextId = 9;

export const mockUserApi = {
  getAll: async (): Promise<User[]> => {
    await delay();
    return [...users];
  },

  getById: async (id: string): Promise<User | undefined> => {
    await delay();
    return users.find((u) => u.id === id);
  },

  create: async (data: CreateUserDto): Promise<User> => {
    await delay();
    const newUser: User = {
      ...data,
      id: String(nextId++),
      createdAt: new Date().toISOString(),
    };
    users = [newUser, ...users];
    return newUser;
  },

  update: async (id: string, data: UpdateUserDto): Promise<User> => {
    await delay();
    const index = users.findIndex((u) => u.id === id);
    if (index === -1) throw new Error('Kullanıcı bulunamadı');
    const updated = { ...users[index]!, ...data };
    users[index] = updated;
    return updated;
  },

  delete: async (id: string): Promise<void> => {
    await delay();
    users = users.filter((u) => u.id !== id);
  },
};
