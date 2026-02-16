import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { App } from 'antd';
import { ruleApi } from '@/api/endpoints/rules';
import type { RuleFilter, CreateRuleDto, UpdateRuleDto } from '@/api/types/rule.types';

const QUERY_KEY = ['rules'];

export const useRules = (filter: RuleFilter) =>
    useQuery({
        queryKey: [...QUERY_KEY, filter],
        queryFn: () => ruleApi.getAll(filter),
    });

export const useCreateRule = (onSuccess?: () => void) => {
    const qc = useQueryClient();
    const { message } = App.useApp();

    return useMutation({
        mutationFn: (data: CreateRuleDto) => ruleApi.create(data),
        onSuccess: () => {
            qc.invalidateQueries({ queryKey: QUERY_KEY });
            message.success('Kural başarıyla oluşturuldu');
            onSuccess?.();
        },
        onError: (err: any) => {
            const data = err?.response?.data;
            const msg = data?.detail ?? data?.title ?? 'Kural oluşturulurken bir hata oluştu';
            message.error(msg);
        },
    });
};

export const useUpdateRule = (onSuccess?: () => void) => {
    const qc = useQueryClient();
    const { message } = App.useApp();

    return useMutation({
        mutationFn: (data: UpdateRuleDto) => ruleApi.update(data),
        onSuccess: () => {
            qc.invalidateQueries({ queryKey: QUERY_KEY });
            message.success('Kural başarıyla güncellendi');
            onSuccess?.();
        },
        onError: (err: any) => {
            const msg = err?.response?.data?.detail ?? 'Kural güncellenirken bir hata oluştu';
            message.error(msg);
        },
    });
};

export const useDeleteRule = () => {
    const qc = useQueryClient();
    const { message } = App.useApp();

    return useMutation({
        mutationFn: (id: string) => ruleApi.delete(id),
        onSuccess: () => {
            qc.invalidateQueries({ queryKey: QUERY_KEY });
            message.success('Kural başarıyla silindi');
        },
        onError: (err: any) => {
            const msg = err?.response?.data?.detail ?? 'Kural silinirken bir hata oluştu';
            message.error(msg);
        },
    });
};