// src/pages/KafkaEvents/KafkaEventsPage.tsx

import { useState } from 'react';
import { Table, Tag, Input, Select, Flex, Tooltip} from 'antd';
import { SearchOutlined, InfoCircleOutlined } from '@ant-design/icons';
import { PageHeader } from '@/components/shared/PageHeader';
import { useKafkaEvents } from '@/hooks/queries/useKafkaEvents';
import dayjs from 'dayjs';
import {
    EVENT_TYPE_LABEL,
    EVENT_TYPE_OPTIONS,
    KafkaEventLogDto,
    KafkaEventLogFilter,
    STATUS_LABEL, STATUS_OPTIONS
} from "@/api/types/kafkaEvent.types.ts";

const eventTypeColorMap: Record<string, string> = {
    Terminated: 'red',
    Hired: 'green',
    Transferred: 'blue',
    Promoted: 'gold',
    Updated: 'orange',
};

const statusColorMap: Record<string, string> = {
    FAILED: 'error',
    POISON: 'purple',
};

export function KafkaEventsPage() {
    const [filter, setFilter] = useState<KafkaEventLogFilter>({
        pageIndex: 1,
        pageSize: 20,
    });

    const { data, isLoading } = useKafkaEvents(filter);

    const columns = [
        {
            title: '#',
            key: 'index',
            width: 50,
            render: (_: unknown, __: unknown, index: number) =>
                (filter.pageIndex - 1) * filter.pageSize + index + 1,
        },
        {
            title: 'Sicil No',
            dataIndex: 'employeeNo',
            key: 'employeeNo',
            width: 100,
            render: (val: string | null) => val ?? '-',
        },
        {
            title: 'İşlem Tipi',
            dataIndex: 'eventType',
            key: 'eventType',
            width: 140,
            render: (val: string | null) => {
                if (!val) return <Tag color="default">Bilinmiyor</Tag>;
                const label = EVENT_TYPE_LABEL[val] ?? val;
                const color = eventTypeColorMap[val] ?? 'default';
                return <Tag color={color}>{label}</Tag>;
            },
        },
        {
            title: 'Geçerlilik Tarihi',
            dataIndex: 'effectiveDate',
            key: 'effectiveDate',
            width: 140,
            render: (val: string | null) =>
                val ? dayjs(val).format('DD.MM.YYYY') : '-',
        },
        {
            title: 'Durum',
            dataIndex: 'status',
            key: 'status',
            width: 120,
            render: (val: string) => (
                <Tag color={statusColorMap[val] ?? 'default'}>
                    {STATUS_LABEL[val] ?? val}
                </Tag>
            ),
        },
        {
            title: 'Deneme',
            dataIndex: 'retryCount',
            key: 'retryCount',
            width: 80,
            render: (val: number) => (
                <Tag color={val > 2 ? 'error' : 'default'}>{val}</Tag>
            ),
        },
        {
            title: 'İşlem Zamanı',
            dataIndex: 'occuredAtUtc',
            key: 'occuredAtUtc',
            width: 155,
            render: (val: string | null) =>
                val ? dayjs(val).format('DD.MM.YYYY HH:mm') : '-',
        },
        {
            title: 'Kayıt Zamanı',
            dataIndex: 'createdAtUtc',
            key: 'createdAtUtc',
            width: 155,
            render: (val: string) => dayjs(val).format('DD.MM.YYYY HH:mm'),
        },
        {
            title: '',
            key: 'detail',
            width: 40,
            render: (_: unknown, record: KafkaEventLogDto) => (
                <Tooltip
                    title={
                        <div>
                            <div><b>Event ID:</b> {record.eventId ?? '-'}</div>
                            <div><b>Correlation ID:</b> {record.correlationId ?? '-'}</div>
                            <div><b>Message Key:</b> {record.messageKey ?? '-'}</div>
                            {record.errorStackTrace && (
                                <div style={{ marginTop: 8 }}>
                                    <b>Stack Trace:</b>
                                    <pre style={{ fontSize: 10, maxHeight: 200, overflow: 'auto', whiteSpace: 'pre-wrap' }}>
                    {record.errorStackTrace}
                  </pre>
                                </div>
                            )}
                        </div>
                    }
                    overlayStyle={{ maxWidth: 500 }}
                >
                    <InfoCircleOutlined style={{ color: '#1677ff', cursor: 'pointer' }} />
                </Tooltip>
            ),
        },
    ];

    return (
        <>
            <PageHeader title="SAP Personel İşlemleri" />

            {/* Filtreler */}
            <Flex gap={12} wrap="wrap" style={{ marginBottom: 16 }}>
                <Input
                    placeholder="Sicil no ara..."
                    prefix={<SearchOutlined />}
                    allowClear
                    style={{ width: 250 }}
                    onPressEnter={(e) =>
                        setFilter((prev) => ({
                            ...prev,
                            pageIndex: 1,
                            search: (e.target as HTMLInputElement).value || undefined,
                        }))
                    }
                    onChange={(e) => {
                        if (!e.target.value) {
                            setFilter((prev) => ({ ...prev, pageIndex: 1, search: undefined }));
                        }
                    }}
                />

                <Select
                    placeholder="İşlem Tipi"
                    allowClear
                    style={{ width: 160 }}
                    options={EVENT_TYPE_OPTIONS}
                    onChange={(val) =>
                        setFilter((prev) => ({ ...prev, pageIndex: 1, eventType: val ?? undefined }))
                    }
                />

                <Select
                    placeholder="Durum"
                    allowClear
                    style={{ width: 160 }}
                    options={STATUS_OPTIONS}
                    onChange={(val) =>
                        setFilter((prev) => ({ ...prev, pageIndex: 1, status: val ?? undefined }))
                    }
                />
            </Flex>

            <Table
                columns={columns}
                dataSource={data?.result ?? []}
                rowKey="id"
                loading={isLoading}
                pagination={{
                    current: filter.pageIndex,
                    pageSize: filter.pageSize,
                    total: data?.rowCount ?? 0,
                    showSizeChanger: true,
                    pageSizeOptions: ['10', '20', '50', '100'],
                    showTotal: (total) => `Toplam ${total} kayıt`,
                    onChange: (page, pageSize) =>
                        setFilter((prev) => ({ ...prev, pageIndex: page, pageSize })),
                }}
                scroll={{ x: 1400 }}
                size="small"
            />
        </>
    );
}