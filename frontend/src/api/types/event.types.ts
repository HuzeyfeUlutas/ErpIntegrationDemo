export interface EventDto {
  id: string;
  eventType: string;
  sourceId: string;
  sourceDetail: string | null;
  correlationId: string;
  occurredAt: string;
  totalCount: number;
  successCount: number;
  failCount: number;
  isCompleted: boolean;
  createdAt: string;
}

export interface EventLogDto {
  id: string;
  eventId: string;
  employeeNo: number;
  personnelName: string;
  roleId: number;
  roleName: string;
  action: string;
  status: string;
  error: string | null;
  createdAt: string;
}

export interface EventFilter {
  pageIndex: number;
  pageSize: number;
  search?: string;
  eventType?: string;
  isCompleted?: boolean;
}

export const EVENT_TYPE_LABEL: Record<string, string> = {
  RuleApplied: 'Kural Uygulandı',
  RoleAssigned: 'Rol Atandı',
  RoleRevoked: 'Rol Kaldırıldı',
  Sync: 'Senkronizasyon',
};

export const EVENT_TYPE_OPTIONS = [
  { label: 'Kural Uygulandı', value: 'RuleApplied' },
  { label: 'Rol Atandı', value: 'RoleAssigned' },
  { label: 'Rol Kaldırıldı', value: 'RoleRevoked' },
  { label: 'Senkronizasyon', value: 'Sync' },
];

export const COMPLETED_OPTIONS = [
  { label: 'Tamamlandı', value: true },
  { label: 'Devam Ediyor', value: false },
];