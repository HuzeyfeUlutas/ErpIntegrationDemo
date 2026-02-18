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
}