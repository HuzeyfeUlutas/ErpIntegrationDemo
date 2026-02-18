import type {DashboardDto} from '@/api/types/dashboard.types';
import axiosInstance from '../axiosInstance';

export const dashboardApi = {
  get: (): Promise<DashboardDto> =>
      axiosInstance.get('/dashboard').then((res) => res.data),
};
