export interface RoleDto {
    id: number;
    name: string;
}

export interface RoleFilter {
    pageIndex: number;
    pageSize: number;
    name?: string;
}