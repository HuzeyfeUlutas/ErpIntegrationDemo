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
  RuleCreated: 'Kural Oluşturuldu',
  RuleUpdated: 'Kural Güncellendi',
  RuleDeleted: 'Kural Silindi'
};

export const EVENT_TYPE_OPTIONS = [
  { label: 'Kural Oluşturuldu', value: 'RuleCreated' },
  { label: 'Kural Güncellendi', value: 'RuleUpdated' },
  { label: 'Kural Silindi', value: 'RuleDeleted' },
];

export const COMPLETED_OPTIONS = [
  { label: 'Tamamlandı', value: true },
  { label: 'Devam Ediyor', value: false },
];