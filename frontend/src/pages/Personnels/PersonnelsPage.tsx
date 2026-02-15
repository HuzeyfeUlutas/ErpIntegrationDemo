import {useEffect, useState} from 'react';
import { Table, Tag, Button, Space, Input, Select, Flex } from 'antd';
import { EditOutlined, SearchOutlined } from '@ant-design/icons';
import { PageHeader } from '@/components/shared/PageHeader';
import { PersonnelForm } from './PersonnelForm';
import { usePersonnels } from '@/hooks/queries/usePersonnels';
import {
} from '@/api/types/personnel.types';
import type { PersonnelDto, PersonnelFilter } from '@/api/types/personnel.types';
import {Campus, CAMPUS_LABEL, CAMPUS_OPTIONS, Title, TITLE_LABEL, TITLE_OPTIONS} from "@/api/types/common.types.ts";

const campusColorMap: Record<Campus, string> = {
    [Campus.Istanbul]: 'blue',
    [Campus.Ankara]: 'orange',
    [Campus.Izmir]: 'green',
};

const titleColorMap: Record<Title, string> = {
    [Title.Engineer]: 'geekblue',
    [Title.Supervisor]: 'purple',
    [Title.Manager]: 'red',
    [Title.Technician]: 'cyan',
    [Title.Emergency]: 'orange',
};

export function PersonnelsPage() {
    // Server-side filter state
    const [filter, setFilter] = useState<PersonnelFilter>({
        pageIndex: 1,
        pageSize: 50,
    });

    const { data, isLoading } = usePersonnels(filter);

    useEffect(() => {
        console.log(data);
    },[data])

    // Drawer state
    const [drawerOpen, setDrawerOpen] = useState(false);
    const [editingItem, setEditingItem] = useState<PersonnelDto | null>(null);

    const handleEdit = (record: PersonnelDto) => {
        setEditingItem(record);
        setDrawerOpen(true);
    };

    const handleDrawerClose = () => {
        setDrawerOpen(false);
        setEditingItem(null);
    };

    const columns = [
        {
            title: 'Sicil No', dataIndex: 'employeeNo', key: 'employeeNo', width: 100,
        },
        {
            title: 'Ad Soyad', dataIndex: 'fullName', key: 'fullName',
        },
        {
            title: 'Yerleşke', dataIndex: 'campus', key: 'campus', width: 120,
            render: (campus: Campus) => (
                <Tag color={campusColorMap[campus]}>{CAMPUS_LABEL[campus]}</Tag>
            ),
        },
        {
            title: 'Ünvan', dataIndex: 'title', key: 'title', width: 130,
            render: (title: Title) => (
                <Tag color={titleColorMap[title]}>{TITLE_LABEL[title]}</Tag>
            ),
        },
        {
            title: 'Roller', dataIndex: 'roles', key: 'roles', width: 220,
            render: (roles: PersonnelDto['roles']) => (
                <Space size={[0, 4]} wrap>
                    {roles.map((role) => <Tag key={role.id} color="volcano">{role.name}</Tag>)}
                    {roles.length === 0 && <Tag color="default">Rol atanmamış</Tag>}
                </Space>
            ),
        },
        {
            title: 'İşlemler', key: 'actions', width: 100,
            render: (_: unknown, record: PersonnelDto) => (
                <Button type="text" icon={<EditOutlined />} onClick={() => handleEdit(record)} />
            ),
        },
    ];

    return (
        <>
            <PageHeader title="Personeller" />

            {/* Filtreler */}
            <Flex gap={12} wrap="wrap" style={{ marginBottom: 16 }}>
                <Input
                    placeholder="Ad veya sicil no..."
                    prefix={<SearchOutlined />}
                    allowClear
                    style={{ width: 240 }}
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
                    placeholder="Yerleşke"
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
                rowKey="employeeNo"
                loading={isLoading}
                pagination={{
                    current: filter.pageIndex,
                    pageSize: filter.pageSize,
                    total: data?.rowCount ?? 0,
                    showSizeChanger: true,
                    showTotal: (total) => `Toplam ${total} personel`,
                    onChange: (page, pageSize) =>
                        setFilter((prev) => ({ ...prev, pageIndex: page, pageSize })),
                }}
                scroll={{ x: 900 }}
            />

            <PersonnelForm
                open={drawerOpen}
                editingItem={editingItem}
                onClose={handleDrawerClose}
            />
        </>
    );
}