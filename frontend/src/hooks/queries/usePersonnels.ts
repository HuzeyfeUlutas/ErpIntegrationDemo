import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { App } from 'antd';
import { personnelApi } from '@/api/endpoints/personnels';
import type { PersonnelFilter, UpdatePersonnelDto } from '@/api/types/personnel.types';

const QUERY_KEY = ['personnels'];

export const usePersonnels = (filter: PersonnelFilter) =>
    useQuery({
        queryKey: [...QUERY_KEY, filter],
        queryFn: () => personnelApi.getAll(filter),
    });

export const useUpdatePersonnel = (onSuccess?: () => void) => {
    const qc = useQueryClient();
    const { message } = App.useApp();

    return useMutation({
        mutationFn: (data: UpdatePersonnelDto) => personnelApi.update(data),
        onSuccess: () => {
            qc.invalidateQueries({ queryKey: QUERY_KEY });
            message.success('Roller başarıyla güncellendi');
            onSuccess?.();
        },
        onError: (err: any) => {
            const data = err?.response?.data;
            const msg = data?.detail ?? data?.title ?? 'Roller güncellenirken bir hata oluştu';
            message.error(msg);
        },
    });
};