import type { DashboardStats } from '@/api/types/dashboard.types';
import { mockDashboardApi } from '@/mocks/dashboard';
// import axiosInstance from '@/api/axiosInstance';

export const dashboardApi = {
  getStats: (): Promise<DashboardStats> => {
    return mockDashboardApi.getStats();
    // return axiosInstance.get<DashboardStats>('/dashboard/stats').then(res => res.data);
  },
};
