import { useQuery } from '@tanstack/react-query';
import { roleApi } from '@/api/endpoints/roles';

export const useRolesForSelect = () =>
    useQuery({
        queryKey: ['roles', 'select'],
        queryFn: async () => {
            const result = await roleApi.getAll({ pageIndex: 1, pageSize: 9999 });
            console.log('roles API response:', result);
            return result;
        },
        staleTime: 1000 * 60 * 30,
        select: (result) => result.result,
    });