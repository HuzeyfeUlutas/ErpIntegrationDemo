import axiosInstance from '@/api/axiosInstance';
import type { JobDto, JobLogDto, JobFilter } from '@/api/types/job.types';
import {PagedResult} from "@/api/types/common.types.ts";

export const jobApi = {
    getAll: (filter: JobFilter): Promise<PagedResult<JobDto[]>> =>
        axiosInstance
            .get('/jobs', { params: filter })
            .then((res) => res.data),

    getLogs: (jobId: string): Promise<JobLogDto[]> =>
        axiosInstance
            .get(`/jobs/${jobId}/logs`)
            .then((res) => res.data),

    exportLogs: async (jobId: string): Promise<void> => {
        const res = await axiosInstance.get(`/jobs/${jobId}/logs/export`, {
            responseType: 'blob',
        });
        const url = window.URL.createObjectURL(new Blob([res.data]));
        const link = document.createElement('a');
        link.href = url;
        link.download = `job-logs-${jobId}.xlsx`;
        link.click();
        window.URL.revokeObjectURL(url);
    },
};