import { useState } from 'react';
import { Table, Tag, Button, Space, Input } from 'antd';
import { EditOutlined, DeleteOutlined, SearchOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import { PageHeader } from '@/components/shared/PageHeader';
import { DeleteConfirmModal } from '@/components/shared/DeleteConfirmModal';
import { UserForm } from './UserForm';
import { useUsers, useDeleteUser } from '@/hooks/queries/useUsers';
import type { User, UserRole } from '@/api/types/user.types';

const roleColorMap: Record<UserRole, string> = {
  admin: 'red',
  editor: 'blue',
  viewer: 'default',
};

const roleLabelMap: Record<UserRole, string> = {
  admin: 'Admin',
  editor: 'Editör',
  viewer: 'İzleyici',
};

export function UsersPage() {
  const { data: users = [], isLoading } = useUsers();
  const [search, setSearch] = useState('');

  const [drawerOpen, setDrawerOpen] = useState(false);
  const [editingItem, setEditingItem] = useState<User | null>(null);

  const [deleteModalOpen, setDeleteModalOpen] = useState(false);
  const [deletingItem, setDeletingItem] = useState<User | null>(null);

  const deleteMutation = useDeleteUser(() => {
    setDeleteModalOpen(false);
    setDeletingItem(null);
  });

  const handleCreate = () => {
    setEditingItem(null);
    setDrawerOpen(true);
  };

  const handleEdit = (record: User) => {
    setEditingItem(record);
    setDrawerOpen(true);
  };

  const handleDelete = (record: User) => {
    setDeletingItem(record);
    setDeleteModalOpen(true);
  };

  const handleDrawerClose = () => {
    setDrawerOpen(false);
    setEditingItem(null);
  };

  const filteredData = users.filter(
    (item) =>
      item.name.toLowerCase().includes(search.toLowerCase()) ||
      item.email.toLowerCase().includes(search.toLowerCase()),
  );

  const columns = [
    {
      title: 'Ad Soyad',
      dataIndex: 'name',
      key: 'name',
      sorter: (a: User, b: User) => a.name.localeCompare(b.name),
    },
    {
      title: 'E-posta',
      dataIndex: 'email',
      key: 'email',
    },
    {
      title: 'Telefon',
      dataIndex: 'phone',
      key: 'phone',
      width: 150,
    },
    {
      title: 'Rol',
      dataIndex: 'role',
      key: 'role',
      width: 100,
      filters: [
        { text: 'Admin', value: 'admin' },
        { text: 'Editör', value: 'editor' },
        { text: 'İzleyici', value: 'viewer' },
      ],
      onFilter: (value: unknown, record: User) => record.role === value,
      render: (role: UserRole) => (
        <Tag color={roleColorMap[role]}>{roleLabelMap[role]}</Tag>
      ),
    },
    {
      title: 'Etiketler',
      dataIndex: 'tags',
      key: 'tags',
      width: 200,
      render: (tags: string[]) => (
        <Space size={[0, 4]} wrap>
          {tags.map((tag) => (
            <Tag key={tag} color="geekblue">
              {tag}
            </Tag>
          ))}
        </Space>
      ),
    },
    {
      title: 'Durum',
      dataIndex: 'status',
      key: 'status',
      width: 90,
      filters: [
        { text: 'Aktif', value: 'active' },
        { text: 'Pasif', value: 'inactive' },
      ],
      onFilter: (value: unknown, record: User) => record.status === value,
      render: (status: string) => (
        <Tag color={status === 'active' ? 'green' : 'default'}>
          {status === 'active' ? 'Aktif' : 'Pasif'}
        </Tag>
      ),
    },
    {
      title: 'Tarih',
      dataIndex: 'createdAt',
      key: 'createdAt',
      width: 120,
      render: (val: string) => dayjs(val).format('DD.MM.YYYY'),
    },
    {
      title: 'İşlemler',
      key: 'actions',
      width: 120,
      render: (_: unknown, record: User) => (
        <Space>
          <Button type="text" icon={<EditOutlined />} onClick={() => handleEdit(record)} />
          <Button type="text" danger icon={<DeleteOutlined />} onClick={() => handleDelete(record)} />
        </Space>
      ),
    },
  ];

  return (
    <>
      <PageHeader title="Kullanıcılar" buttonText="Yeni Kullanıcı" onButtonClick={handleCreate} />

      <Input
        placeholder="Ad veya e-posta ara..."
        prefix={<SearchOutlined />}
        value={search}
        onChange={(e) => setSearch(e.target.value)}
        style={{ marginBottom: 16, maxWidth: 320 }}
        allowClear
      />

      <Table
        columns={columns}
        dataSource={filteredData}
        rowKey="id"
        loading={isLoading}
        pagination={{ pageSize: 10, showSizeChanger: true, showTotal: (total) => `Toplam ${total} kayıt` }}
        scroll={{ x: 1000 }}
      />

      <UserForm open={drawerOpen} editingItem={editingItem} onClose={handleDrawerClose} />

      <DeleteConfirmModal
        open={deleteModalOpen}
        description={`"${deletingItem?.name}" kullanıcısını silmek istediğinizden emin misiniz?`}
        loading={deleteMutation.isPending}
        onConfirm={() => deletingItem && deleteMutation.mutate(deletingItem.id)}
        onCancel={() => {
          setDeleteModalOpen(false);
          setDeletingItem(null);
        }}
      />
    </>
  );
}
