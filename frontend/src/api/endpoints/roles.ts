import axiosInstance from '@/api/axiosInstance';
import {RoleDto, RoleFilter} from "@/api/types/role.types.ts";
import {PagedResult} from "@/api/types/common.types.ts";

export const roleApi = {
    getAll: (filter: RoleFilter): Promise<PagedResult<RoleDto[]>> =>
        axiosInstance
            .get('/roles', { params: filter })
            .then((res) => res.data),
};