import { useQuery } from '@tanstack/react-query';
import { kafkaEventApi } from '@/api/endpoints/kafkaEvents';
import {KafkaEventLogFilter} from "@/api/types/kafkaEvent.types.ts";

export const useKafkaEvents = (filter: KafkaEventLogFilter) =>
    useQuery({
        queryKey: ['kafka-events', filter],
        queryFn: () => kafkaEventApi.getAll(filter),
    });