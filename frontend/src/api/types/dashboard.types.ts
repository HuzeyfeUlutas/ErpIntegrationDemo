export interface DashboardStats {
  totalUsers: number;
  totalProducts: number;
  totalCategories: number;
  activeUsers: number;
  lowStockProducts: number;
  recentActivities: Activity[];
}

export interface Activity {
  id: string;
  type: 'user_created' | 'product_added' | 'category_updated' | 'user_deleted';
  description: string;
  timestamp: string;
}
