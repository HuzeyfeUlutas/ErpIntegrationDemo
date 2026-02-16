import axiosInstance from '@/api/axiosInstance';
import type {
    RuleDto,
    CreateRuleDto,
    UpdateRuleDto,
    RuleFilter,
} from '@/api/types/rule.types';
import {PagedResult} from "@/api/types/common.types.ts";

export const ruleApi = {
    getAll: (filter: RuleFilter): Promise<PagedResult<RuleDto[]>> =>
        axiosInstance
            .get('/rules', { params: filter })
            .then((res) => res.data),

    create: (data: CreateRuleDto): Promise<{ id: string }> =>
        axiosInstance.post('/rules', data).then((res) => res.data),

    update: (data: UpdateRuleDto): Promise<void> =>
        axiosInstance.put('/rules', data),

    delete: (id: string): Promise<void> =>
        axiosInstance.delete(`/rules/${id}`),
};