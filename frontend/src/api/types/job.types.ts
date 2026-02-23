export interface JobDto {
    id: string;
    jobType: string;
    status: string;
    totalCount: number;
    successCount: number;
    failureCount: number;
    createdAt: string;
    updatedAt: string | null;
}

export interface JobLogDto {
    id: string;
    jobId: string;
    message: string;
    status: string;
    createdAt: string;
}

export interface JobFilter {
    pageIndex: number;
    pageSize: number;
    search?: string;
    status?: string;
    jobType?: string;
}

export const JOB_STATUS_LABEL: Record<string, string> = {
    Running: 'Çalışıyor',
    Done: 'Tamamlandı',
    CompletedWithErrors: 'Hatalı Tamamlandı',
    Failed: 'Başarısız',
};

export const JOB_STATUS_OPTIONS = [
    { label: 'Çalışıyor', value: 'Running' },
    { label: 'Tamamlandı', value: 'Done' },
    { label: 'Hatalı Tamamlandı', value: 'CompletedWithErrors' },
    { label: 'Başarısız', value: 'Failed' },
];

export const JOB_TYPE_LABEL: Record<string, string> = {
    RuleSync: 'Kural Senkronizasyonu',
    PersonnelSync: 'Personel Senkronizasyonu',
};

export const JOB_TYPE_OPTIONS = [
    { label: 'Kural Senkronizasyonu', value: 'RuleSync' },
    { label: 'Personel Senkronizasyonu', value: 'PersonnelSync' },
];

export const JOB_LOG_STATUS_LABEL: Record<string, string> = {
    INFO: 'Bilgi',
    FATAL: 'Kritik Hata',
    WARNING: 'Uyarı',
    ERROR: 'Hata',
    SUCCESS: 'Başarılı',
};