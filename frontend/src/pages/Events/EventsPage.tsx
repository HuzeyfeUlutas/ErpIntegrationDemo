// src/pages/Events/EventsPage.tsx

import { useState } from 'react';
import { Table, Tag, Button, Input, Select, Flex, Space, Progress, Spin } from 'antd';
import {SearchOutlined, FileExcelOutlined} from '@ant-design/icons';
import { PageHeader } from '@/components/shared/PageHeader';
import { useEvents, useEventLogs } from '@/hooks/queries/useEvents';
import { eventApi } from '@/api/endpoints/events';
import { EVENT_TYPE_LABEL, EVENT_TYPE_OPTIONS, COMPLETED_OPTIONS } from '@/api/types/event.types';
import type { EventDto, EventFilter } from '@/api/types/event.types';
import dayjs from 'dayjs';

const eventTypeColorMap: Record<string, string> = {
    RuleApplied: 'blue',
    RoleAssigned: 'green',
    RoleRevoked: 'red',
    Sync: 'orange',
};

function EventLogTable({ eventId }: { eventId: string }) {
    const { data: logs, isLoading } = useEventLogs(eventId);

    if (isLoading) return <Spin size="small" />;

    const columns = [
        {
            title: '#',
            key: 'index',
            width: 50,
            render: (_: unknown, __: unknown, i: number) => i + 1,
        },
        {
            title: 'Sicil No',
            dataIndex: 'employeeNo',
            key: 'employeeNo',
            width: 90,
        },
        {
            title: 'Personel Adı',
            dataIndex: 'personnelName',
            key: 'personnelName',
        },
        {
            title: 'Rol',
            dataIndex: 'roleName',
            key: 'roleName',
        },
        {
            title: 'İşlem',
            dataIndex: 'action',
            key: 'action',
            width: 120,
        },
        {
            title: 'Durum',
            dataIndex: 'status',
            key: 'status',
            width: 100,
            render: (val: string) => (
                <Tag color={val === 'Success' ? 'success' : 'error'}>
                    {val === 'Success' ? 'Başarılı' : 'Başarısız'}
                </Tag>
            ),
        },
        {
            title: 'Hata',
            dataIndex: 'error',
            key: 'error',
            ellipsis: true,
            render: (val: string | null) => val ?? '-',
        },
        {
            title: 'Tarih',
            dataIndex: 'createdAt',
            key: 'createdAt',
            width: 140,
            render: (val: string) => dayjs(val).format('DD.MM.YYYY HH:mm'),
        },
    ];

    return (
        <Table
            columns={columns}
            dataSource={logs ?? []}
            rowKey="id"
            pagination={false}
            size="small"
        />
    );
}

export function EventsPage() {
    const [filter, setFilter] = useState<EventFilter>({
        pageIndex: 1,
        pageSize: 10,
    });

    const { data, isLoading } = useEvents(filter);
    const [exporting, setExporting] = useState<string | null>(null);

    const handleExport = async (eventId: string) => {
        setExporting(eventId);
        try {
            await eventApi.exportLogs(eventId);
        } finally {
            setExporting(null);
        }
    };

    const columns = [
        {
            title: '#',
            key: 'index',
            width: 50,
            render: (_: unknown, __: unknown, i: number) =>
                (filter.pageIndex - 1) * filter.pageSize + i + 1,
        },
        {
            title: 'Olay Tipi',
            dataIndex: 'eventType',
            key: 'eventType',
            width: 160,
            render: (val: string) => {
                const label = EVENT_TYPE_LABEL[val] ?? val;
                const color = eventTypeColorMap[val] ?? 'default';
                return <Tag color={color}>{label}</Tag>;
            },
        },
        {
            title: 'Kaynak',
            dataIndex: 'sourceId',
            key: 'sourceId',
            width: 140,
        },
        {
            title: 'Detay',
            dataIndex: 'sourceDetail',
            key: 'sourceDetail',
            ellipsis: true,
            render: (val: string | null) => val ?? '-',
        },
        {
            title: 'Sonuç',
            key: 'result',
            width: 160,
            render: (_: unknown, record: EventDto) => {
                if (!record.isCompleted) return <Tag color="processing">Devam ediyor</Tag>;
                const total = record.totalCount || 1;
                const percent = Math.round((record.successCount / total) * 100);
                return (
                    <Space size={4}>
                        <Progress type="circle" size={26} percent={percent}
                                  strokeColor={percent === 100 ? '#52c41a' : '#faad14'} />
                        <span style={{ fontSize: 12 }}>
              {record.successCount}/{record.totalCount}
            </span>
                        {record.failCount > 0 && (
                            <Tag color="error" style={{ fontSize: 11 }}>{record.failCount} hata</Tag>
                        )}
                    </Space>
                );
            },
        },
        {
            title: 'Durum',
            dataIndex: 'isCompleted',
            key: 'isCompleted',
            width: 110,
            render: (val: boolean) =>
                val ? <Tag color="success">Tamamlandı</Tag> : <Tag color="processing">İşleniyor</Tag>,
        },
        {
            title: 'Tarih',
            dataIndex: 'occurredAt',
            key: 'occurredAt',
            width: 150,
            render: (val: string) => dayjs(val).format('DD.MM.YYYY HH:mm'),
        },
        {
            title: '',
            key: 'export',
            width: 50,
            render: (_: unknown, record: EventDto) =>
                record.totalCount > 0 ? (
                    <Button
                        type="text"
                        icon={<FileExcelOutlined style={{ color: '#52c41a' }} />}
                        loading={exporting === record.id}
                        onClick={() => handleExport(record.id)}
                        title="Excel'e Aktar"
                    />
                ) : null,
        },
    ];

    return (
        <>
            <PageHeader title="Olaylar (Events)" />

            <Flex gap={12} wrap="wrap" style={{ marginBottom: 16 }}>
                <Input
                    placeholder="Kaynak veya Correlation ID..."
                    prefix={<SearchOutlined />}
                    allowClear
                    style={{ width: 260 }}
                    onPressEnter={(e) =>
                        setFilter((prev) => ({
                            ...prev,
                            pageIndex: 1,
                            search: (e.target as HTMLInputElement).value || undefined,
                        }))
                    }
                    onChange={(e) => {
                        if (!e.target.value)
                            setFilter((prev) => ({ ...prev, pageIndex: 1, search: undefined }));
                    }}
                />
                <Select
                    placeholder="Olay Tipi"
                    allowClear
                    style={{ width: 180 }}
                    options={EVENT_TYPE_OPTIONS}
                    onChange={(val) =>
                        setFilter((prev) => ({ ...prev, pageIndex: 1, eventType: val ?? undefined }))
                    }
                />
                <Select
                    placeholder="Durum"
                    allowClear
                    style={{ width: 160 }}
                    options={COMPLETED_OPTIONS}
                    onChange={(val) =>
                        setFilter((prev) => ({ ...prev, pageIndex: 1, isCompleted: val ?? undefined }))
                    }
                />
            </Flex>

            <Table
                columns={columns}
                dataSource={data?.result ?? []}
                rowKey="id"
                loading={isLoading}
                expandable={{
                    expandedRowRender: (record) => <EventLogTable eventId={record.id} />,
                }}
                pagination={{
                    current: filter.pageIndex,
                    pageSize: filter.pageSize,
                    total: data?.rowCount ?? 0,
                    showSizeChanger: true,
                    showTotal: (total) => `Toplam ${total} olay`,
                    onChange: (page, pageSize) =>
                        setFilter((prev) => ({ ...prev, pageIndex: page, pageSize })),
                }}
                scroll={{ x: 1100 }}
                size="small"
            />
        </>
    );
}