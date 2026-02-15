import {RoleDto} from "@/api/types/role.types.ts";
import {Campus, Title} from "@/api/types/common.types.ts";

export interface PersonnelDto {
    employeeNo: number;
    fullName: string;
    campus: Campus;
    title: Title;
    roles: RoleDto[];
}

// Update sadece rol atama
export interface UpdatePersonnelDto {
    employeeNo: number;
    roleIds: number[];
}

// Server-side filtre
export interface PersonnelFilter {
    pageIndex: number;
    pageSize: number;
    search?: string;
    campus?: Campus;
    title?: Title;
}

