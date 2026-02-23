import axiosInstance from '@/api/axiosInstance';
import type { EventDto, EventLogDto, EventFilter } from '@/api/types/event.types';
import {PagedResult} from "@/api/types/common.types.ts";

export const eventApi = {
    getAll: (filter: EventFilter): Promise<PagedResult<EventDto[]>> =>
        axiosInstance
            .get('/events', { params: filter })
            .then((res) => res.data),

    getLogs: (eventId: string): Promise<EventLogDto[]> =>
        axiosInstance
            .get(`/events/${eventId}/logs`)
            .then((res) => res.data),

    exportLogs: async (eventId: string): Promise<void> => {
        const res = await axiosInstance.get(`/events/${eventId}/logs/export`, {
            responseType: 'blob',
        });
        const url = window.URL.createObjectURL(new Blob([res.data]));
        const link = document.createElement('a');
        link.href = url;
        link.download = `event-logs-${eventId}.xlsx`;
        link.click();
        window.URL.revokeObjectURL(url);
    },
};