// src/pages/Rules/RulesPage.tsx

import { useState } from 'react';
import { Table, Tag, Button, Space, Input, Select, Flex, Popconfirm } from 'antd';
import { EditOutlined, DeleteOutlined, SearchOutlined } from '@ant-design/icons';
import { PageHeader } from '@/components/shared/PageHeader';
import { RuleForm } from './RuleForm';
import { useRules, useDeleteRule } from '@/hooks/queries/useRules';
import { CAMPUS_LABEL, TITLE_LABEL, CAMPUS_OPTIONS, TITLE_OPTIONS } from '@/api/types/common.types';
import type { Campus, Title } from '@/api/types/common.types';
import type { RuleDto, RuleFilter } from '@/api/types/rule.types';

const campusColorMap: Record<string, string> = {
    Istanbul: 'blue',
    Ankara: 'orange',
    Izmir: 'green',
};

const titleColorMap: Record<string, string> = {
    Engineer: 'geekblue',
    Supervisor: 'purple',
    Manager: 'red',
    Technician: 'cyan',
};

export function RulesPage() {
    const [filter, setFilter] = useState<RuleFilter>({
        pageIndex: 1,
        pageSize: 10,
    });

    const { data, isLoading } = useRules(filter);
    const deleteMutation = useDeleteRule();

    // Drawer state
    const [drawerOpen, setDrawerOpen] = useState(false);
    const [editingItem, setEditingItem] = useState<RuleDto | null>(null);

    const handleCreate = () => {
        setEditingItem(null);
        setDrawerOpen(true);
    };

    const handleEdit = (record: RuleDto) => {
        setEditingItem(record);
        setDrawerOpen(true);
    };

    const handleDelete = (id: string) => {
        deleteMutation.mutate(id);
    };

    const handleDrawerClose = () => {
        setDrawerOpen(false);
        setEditingItem(null);
    };

    const columns = [
        {
            title: '#',
            key: 'index',
            width: 60,
            render: (_: unknown, __: unknown, index: number) =>
                (filter.pageIndex - 1) * filter.pageSize + index + 1,
        },
        {
            title: 'Kural Adı',
            dataIndex: 'name',
            key: 'name',
        },
        {
            title: 'Kampüs',
            dataIndex: 'campus',
            key: 'campus',
            width: 130,
            render: (campus: Campus | null) =>
                campus ? (
                    <Tag color={campusColorMap[campus]}>{CAMPUS_LABEL[campus]}</Tag>
                ) : (
                    <Tag color="default">Tümü</Tag>
                ),
        },
        {
            title: 'Ünvan',
            dataIndex: 'title',
            key: 'title',
            width: 130,
            render: (title: Title | null) =>
                title ? (
                    <Tag color={titleColorMap[title]}>{TITLE_LABEL[title]}</Tag>
                ) : (
                    <Tag color="default">Tümü</Tag>
                ),
        },
        {
            title: 'Roller',
            dataIndex: 'roles',
            key: 'roles',
            width: 280,
            render: (roles: RuleDto['roles']) => (
                <Space size={[0, 4]} wrap>
                    {roles.slice(0, 3).map((role) => (
                        <Tag key={role.id} color="volcano">{role.name}</Tag>
                    ))}
                    {roles.length > 3 && (
                        <Tag color="default">+{roles.length - 3} rol</Tag>
                    )}
                    {roles.length === 0 && <Tag color="default">Rol atanmamış</Tag>}
                </Space>
            ),
        },
        {
            title: 'Durum',
            dataIndex: 'isActive',
            key: 'isActive',
            width: 90,
            render: (isActive: boolean) => (
                <Tag color={isActive ? 'success' : 'error'}>
                    {isActive ? 'Aktif' : 'Pasif'}
                </Tag>
            ),
        },
        {
            title: 'İşlemler',
            key: 'actions',
            width: 110,
            render: (_: unknown, record: RuleDto) => (
                <Space>
                    <Button
                        type="text"
                        icon={<EditOutlined />}
                        onClick={() => handleEdit(record)}
                    />
                    <Popconfirm
                        title="Bu kuralı silmek istediğinize emin misiniz?"
                        onConfirm={() => handleDelete(record.id)}
                        okText="Evet"
                        cancelText="Hayır"
                    >
                        <Button type="text" danger icon={<DeleteOutlined />} />
                    </Popconfirm>
                </Space>
            ),
        },
    ];

    return (
        <>
            <PageHeader
                title="Kurallar"
                buttonText="Yeni Kural"
                onButtonClick={handleCreate}
            />

            {/* Filtreler */}
            <Flex gap={12} wrap="wrap" style={{ marginBottom: 16 }}>
                <Input
                    placeholder="Kural adı..."
                    prefix={<SearchOutlined />}
                    allowClear
                    style={{ width: 240 }}
                    onPressEnter={(e) =>
                        setFilter((prev) => ({
                            ...prev,
                            pageIndex: 1,
                            name: (e.target as HTMLInputElement).value || undefined,
                        }))
                    }
                    onChange={(e) => {
                        if (!e.target.value) {
                            setFilter((prev) => ({ ...prev, pageIndex: 1, name: undefined }));
                        }
                    }}
                />

                <Select
                    placeholder="Kampüs"
                    allowClear
                    style={{ width: 160 }}
                    options={[...CAMPUS_OPTIONS]}
                    onChange={(val) =>
                        setFilter((prev) => ({ ...prev, pageIndex: 1, campus: val ?? undefined }))
                    }
                />

                <Select
                    placeholder="Ünvan"
                    allowClear
                    style={{ width: 160 }}
                    options={[...TITLE_OPTIONS]}
                    onChange={(val) =>
                        setFilter((prev) => ({ ...prev, pageIndex: 1, title: val ?? undefined }))
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
                    showTotal: (total) => `Toplam ${total} kural`,
                    onChange: (page, pageSize) =>
                        setFilter((prev) => ({ ...prev, pageIndex: page, pageSize })),
                }}
                scroll={{ x: 900 }}
            />

            <RuleForm
                open={drawerOpen}
                editingItem={editingItem}
                onClose={handleDrawerClose}
            />
        </>
    );
}