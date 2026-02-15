import axiosInstance from '@/api/axiosInstance';
import type {
    PersonnelDto,
    UpdatePersonnelDto,
    PersonnelFilter,
} from '@/api/types/personnel.types';
import {PagedResult} from "@/api/types/common.types.ts";

export const personnelApi = {
    getAll: (filter: PersonnelFilter): Promise<PagedResult<PersonnelDto[]>> =>
        axiosInstance
            .get('/personnels', { params: filter })
            .then((res) => res.data),

    update: (data: UpdatePersonnelDto): Promise<void> =>
        axiosInstance.put('/personnels', data),
};