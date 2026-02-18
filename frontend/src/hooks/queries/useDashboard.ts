import { useQuery } from '@tanstack/react-query';
import { dashboardApi } from '@/api/endpoints/dashboard';

export const useDashboard = () =>
    useQuery({
      queryKey: ['dashboard'],
      queryFn: () => dashboardApi.get(),
      // refetchInterval: 5000,
    });
