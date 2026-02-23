export interface KafkaEventLogDto {
    id: number;
    topic: string;
    partitionNo: number;
    offset: number;
    messageKey: string | null;
    status: string;
    retryCount: number;
    errorMessage: string;
    errorStackTrace: string | null;
    createdAtUtc: string;
    // MessageValue'dan parse edilen alanlar
    eventType: string | null;
    employeeNo: string | null;
    effectiveDate: string | null;
    occuredAtUtc: string | null;
    correlationId: string | null;
    eventId: string | null;
}

export interface KafkaEventLogFilter {
    pageIndex: number;
    pageSize: number;
    search?: string;
    status?: string;
    eventType?: string;
}

// EventType Türkçe etiketler
export const EVENT_TYPE_LABEL: Record<string, string> = {
    Terminated: 'İşten Çıkış',
    Hired: 'İşe Giriş',
    Updated: 'Güncelleme',
};

export const EVENT_TYPE_OPTIONS = [
    { label: 'İşten Çıkış', value: 'Terminated' },
    { label: 'İşe Giriş', value: 'Hired' },
    { label: 'Güncelleme', value: 'Updated' },
];

export const STATUS_OPTIONS = [
    { label: 'Başarılı', value: 'SUCCESS' },
    { label: 'Başarısız', value: 'FAILED' },
    { label: 'Zehirli Mesaj', value: 'POISON' },
    { label: 'Bilinmiyor', value: 'UNKNOWN' },
];

export const STATUS_LABEL: Record<string, string> = {
    SUCCESS: 'Başarılı',
    FAILED: 'Başarısız',
    POISON: 'Zehirli Mesaj',
    UNKNOWN: 'Bilinmiyor',
};