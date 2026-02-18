import {EventDto} from "@/api/types/event.types.ts";

export interface DashboardStats {
  totalPersonnel: number;
  totalRules: number;
  activeRules: number;
  totalEvents: number;
  totalJobs: number;
}

export interface DashboardDto {
  stats: DashboardStats;
  recentEvents: EventDto[];
}
