import { useState } from 'react';
import { Table, Tag, Button, Input, Select, Flex, Spin } from 'antd';
import {SearchOutlined, FileExcelOutlined} from '@ant-design/icons';
import { PageHeader } from '@/components/shared/PageHeader';
import { useJobs, useJobLogs } from '@/hooks/queries/useJobs';
import { jobApi } from '@/api/endpoints/jobs';
import {
    JOB_STATUS_LABEL,
    JOB_STATUS_OPTIONS,
    JOB_TYPE_LABEL,
    JOB_LOG_STATUS_LABEL,
} from '@/api/types/job.types';
import type { JobDto, JobFilter } from '@/api/types/job.types';
import dayjs from 'dayjs';

const statusColorMap: Record<string, string> = {
    Running: 'processing',
    Done: 'success',
    CompletedWithErrors: 'warning',
    Failed: 'error',
};

const logStatusColorMap: Record<string, string> = {
    INFO: 'blue',
    SUCCESS: 'success',
    WARNING: 'warning',
    ERROR: 'error',
    FATAL: 'magenta',
};

function JobLogTable({ jobId }: { jobId: string }) {
    const { data: logs, isLoading } = useJobLogs(jobId);

    if (isLoading) return <Spin size="small" />;

    const columns = [
        {
            title: '#',
            key: 'index',
            width: 50,
            render: (_: unknown, __: unknown, i: number) => i + 1,
        },
        {
            title: 'Mesaj',
            dataIndex: 'message',
            key: 'message',
        },
        {
            title: 'Durum',
            dataIndex: 'status',
            key: 'status',
            width: 120,
            render: (val: string) => (
                <Tag color={logStatusColorMap[val] ?? 'default'}>
                    {JOB_LOG_STATUS_LABEL[val] ?? val}
                </Tag>
            ),
        },
        {
            title: 'Tarih',
            dataIndex: 'createdAt',
            key: 'createdAt',
            width: 160,
            render: (val: string) => dayjs(val).format('DD.MM.YYYY HH:mm:ss'),
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

export function JobsPage() {
    const [filter, setFilter] = useState<JobFilter>({
        pageIndex: 1,
        pageSize: 10,
    });

    const { data, isLoading } = useJobs(filter);
    const [exporting, setExporting] = useState<string | null>(null);

    const handleExport = async (jobId: string) => {
        setExporting(jobId);
        try {
            await jobApi.exportLogs(jobId);
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
            title: 'Job Tipi',
            dataIndex: 'jobType',
            key: 'jobType',
            width: 200,
            render: (val: string) => (
                <Tag color="blue">{JOB_TYPE_LABEL[val] ?? val}</Tag>
            ),
        },
        {
            title: 'Durum',
            dataIndex: 'status',
            key: 'status',
            width: 160,
            render: (val: string) => (
                <Tag color={statusColorMap[val] ?? 'default'}>
                    {JOB_STATUS_LABEL[val] ?? val}
                </Tag>
            ),
        },
        {
            title: 'Toplam',
            dataIndex: 'totalCount',
            key: 'totalCount',
            width: 80,
        },
        {
            title: 'Başarılı',
            dataIndex: 'successCount',
            key: 'successCount',
            width: 80,
            render: (val: number) => <span style={{ color: '#52c41a' }}>{val}</span>,
        },
        {
            title: 'Başarısız',
            dataIndex: 'failureCount',
            key: 'failureCount',
            width: 80,
            render: (val: number) => (
                <span style={{ color: val > 0 ? '#ff4d4f' : undefined }}>{val}</span>
            ),
        },
        {
            title: 'Başlangıç',
            dataIndex: 'createdAt',
            key: 'createdAt',
            width: 155,
            render: (val: string) => dayjs(val).format('DD.MM.YYYY HH:mm'),
        },
        {
            title: 'Bitiş',
            dataIndex: 'updatedAt',
            key: 'updatedAt',
            width: 155,
            render: (val: string | null) =>
                val ? dayjs(val).format('DD.MM.YYYY HH:mm') : '-',
        },
        {
            title: '',
            key: 'export',
            width: 50,
            render: (_: unknown, record: JobDto) =>
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
            <PageHeader title="Görevler (Jobs)" />

            <Flex gap={12} wrap="wrap" style={{ marginBottom: 16 }}>
                <Input
                    placeholder="Ara..."
                    prefix={<SearchOutlined />}
                    allowClear
                    style={{ width: 220 }}
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
                    placeholder="Durum"
                    allowClear
                    style={{ width: 180 }}
                    options={JOB_STATUS_OPTIONS}
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
                expandable={{
                    expandedRowRender: (record) => <JobLogTable jobId={record.id} />,
                }}
                pagination={{
                    current: filter.pageIndex,
                    pageSize: filter.pageSize,
                    total: data?.rowCount ?? 0,
                    showSizeChanger: true,
                    showTotal: (total) => `Toplam ${total} görev`,
                    onChange: (page, pageSize) =>
                        setFilter((prev) => ({ ...prev, pageIndex: page, pageSize })),
                }}
                scroll={{ x: 1100 }}
                size="small"
            />
        </>
    );
}