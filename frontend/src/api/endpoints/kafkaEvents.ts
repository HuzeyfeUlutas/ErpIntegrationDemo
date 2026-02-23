import axiosInstance from '@/api/axiosInstance';
import {PagedResult} from "@/api/types/common.types.ts";
import {KafkaEventLogDto, KafkaEventLogFilter} from "@/api/types/kafkaEvent.types.ts";

export const kafkaEventApi = {
    getAll: (filter: KafkaEventLogFilter): Promise<PagedResult<KafkaEventLogDto[]>> =>
        axiosInstance
            .get('/kafka-events', { params: filter })
            .then((res) => res.data),
};