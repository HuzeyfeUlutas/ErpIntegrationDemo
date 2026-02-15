import type { DashboardStats } from '@/api/types/dashboard.types';
import { delay } from './delay';

export const mockDashboardApi = {
  getStats: async (): Promise<DashboardStats> => {
    await delay();
    return {
      totalUsers: 8,
      totalProducts: 10,
      totalCategories: 8,
      activeUsers: 6,
      lowStockProducts: 2,
      recentActivities: [
        { id: '1', type: 'product_added', description: 'Lego Technic Set ürünü eklendi', timestamp: '2025-02-02T14:30:00Z' },
        { id: '2', type: 'user_created', description: 'Selin Koç kullanıcısı oluşturuldu', timestamp: '2025-01-17T09:15:00Z' },
        { id: '3', type: 'category_updated', description: 'Otomotiv kategorisi güncellendi', timestamp: '2025-02-10T11:00:00Z' },
        { id: '4', type: 'product_added', description: 'Parfüm - Chanel No.5 ürünü eklendi', timestamp: '2025-02-01T16:45:00Z' },
        { id: '5', type: 'user_deleted', description: 'Test kullanıcısı silindi', timestamp: '2025-01-28T10:20:00Z' },
      ],
    };
  },
};
