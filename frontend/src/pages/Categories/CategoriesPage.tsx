import { useState } from 'react';
import { Table, Tag, Button, Space, Input } from 'antd';
import { EditOutlined, DeleteOutlined, SearchOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import { PageHeader } from '@/components/shared/PageHeader';
import { DeleteConfirmModal } from '@/components/shared/DeleteConfirmModal';
import { CategoryForm } from './CategoryForm';
import { useCategories, useDeleteCategory } from '@/hooks/queries/useCategories';
import type { Category } from '@/api/types/category.types';

export function CategoriesPage() {
  const { data: categories = [], isLoading } = useCategories();
  const [search, setSearch] = useState('');

  // Drawer state
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [editingItem, setEditingItem] = useState<Category | null>(null);

  // Delete modal state
  const [deleteModalOpen, setDeleteModalOpen] = useState(false);
  const [deletingItem, setDeletingItem] = useState<Category | null>(null);

  const deleteMutation = useDeleteCategory(() => {
    setDeleteModalOpen(false);
    setDeletingItem(null);
  });

  const handleCreate = () => {
    setEditingItem(null);
    setDrawerOpen(true);
  };

  const handleEdit = (record: Category) => {
    setEditingItem(record);
    setDrawerOpen(true);
  };

  const handleDelete = (record: Category) => {
    setDeletingItem(record);
    setDeleteModalOpen(true);
  };

  const handleDrawerClose = () => {
    setDrawerOpen(false);
    setEditingItem(null);
  };

  const filteredData = categories.filter(
    (item) =>
      item.name.toLowerCase().includes(search.toLowerCase()) ||
      item.description.toLowerCase().includes(search.toLowerCase()),
  );

  const columns = [
    {
      title: 'Kategori Adı',
      dataIndex: 'name',
      key: 'name',
      sorter: (a: Category, b: Category) => a.name.localeCompare(b.name),
    },
    {
      title: 'Açıklama',
      dataIndex: 'description',
      key: 'description',
      ellipsis: true,
    },
    {
      title: 'Durum',
      dataIndex: 'status',
      key: 'status',
      width: 100,
      filters: [
        { text: 'Aktif', value: 'active' },
        { text: 'Pasif', value: 'inactive' },
      ],
      onFilter: (value: unknown, record: Category) => record.status === value,
      render: (status: string) => (
        <Tag color={status === 'active' ? 'green' : 'default'}>
          {status === 'active' ? 'Aktif' : 'Pasif'}
        </Tag>
      ),
    },
    {
      title: 'Oluşturulma',
      dataIndex: 'createdAt',
      key: 'createdAt',
      width: 140,
      sorter: (a: Category, b: Category) => a.createdAt.localeCompare(b.createdAt),
      render: (val: string) => dayjs(val).format('DD.MM.YYYY'),
    },
    {
      title: 'İşlemler',
      key: 'actions',
      width: 120,
      render: (_: unknown, record: Category) => (
        <Space>
          <Button
            type="text"
            icon={<EditOutlined />}
            onClick={() => handleEdit(record)}
          />
          <Button
            type="text"
            danger
            icon={<DeleteOutlined />}
            onClick={() => handleDelete(record)}
          />
        </Space>
      ),
    },
  ];

  return (
    <>
      <PageHeader
        title="Kategoriler"
        buttonText="Yeni Kategori"
        onButtonClick={handleCreate}
      />

      <Input
        placeholder="Kategori ara..."
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
      />

      <CategoryForm
        open={drawerOpen}
        editingItem={editingItem}
        onClose={handleDrawerClose}
      />

      <DeleteConfirmModal
        open={deleteModalOpen}
        description={`"${deletingItem?.name}" kategorisini silmek istediğinizden emin misiniz?`}
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
