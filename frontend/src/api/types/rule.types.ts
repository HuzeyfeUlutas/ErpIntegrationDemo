// src/api/types/rule.types.ts
import type { Campus, Title } from './common.types';
import {RoleDto} from "@/api/types/role.types.ts";

export interface RuleDto {
    id: string;          
    name: string;
    campus: Campus | null;
    title: Title | null;
    isActive: boolean;
    roles: RoleDto[];
}

export interface CreateRuleDto {
    name: string;
    campus: Campus | null;
    title: Title | null;
    roleIds: number[];
    applyToExistingPersonnel: boolean;
}

export interface UpdateRuleDto {
    id: string;
    name: string;
    campus: Campus | null;
    title: Title | null;
    isActive: boolean;
    roleIds: number[];
}

export interface RuleFilter {
    pageIndex: number;
    pageSize: number;
    name?: string;
    campus?: Campus;
    title?: Title;
}