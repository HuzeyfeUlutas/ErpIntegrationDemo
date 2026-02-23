import { useQuery } from '@tanstack/react-query';
import { jobApi } from '@/api/endpoints/jobs';
import type { JobFilter } from '@/api/types/job.types';

export const useJobs = (filter: JobFilter) =>
    useQuery({
        queryKey: ['jobs', filter],
        queryFn: () => jobApi.getAll(filter),
    });

export const useJobLogs = (jobId: string | null) =>
    useQuery({
        queryKey: ['job-logs', jobId],
        queryFn: () => jobApi.getLogs(jobId!),
        enabled: !!jobId,
    });