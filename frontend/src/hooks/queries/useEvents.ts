import { useQuery } from '@tanstack/react-query';
import { eventApi } from '@/api/endpoints/events';
import type { EventFilter } from '@/api/types/event.types';

export const useEvents = (filter: EventFilter) =>
    useQuery({
        queryKey: ['events', filter],
        queryFn: () => eventApi.getAll(filter),
    });

export const useEventLogs = (eventId: string | null) =>
    useQuery({
        queryKey: ['event-logs', eventId],
        queryFn: () => eventApi.getLogs(eventId!),
        enabled: !!eventId,
    });