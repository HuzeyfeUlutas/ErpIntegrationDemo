import { useState } from 'react';
import { Table, Tag, Button, Space, Input } from 'antd';
import { EditOutlined, DeleteOutlined, SearchOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import { PageHeader } from '@/components/shared/PageHeader';
import { DeleteConfirmModal } from '@/components/shared/DeleteConfirmModal';
import { ProductForm } from './ProductForm';
import { useProducts, useDeleteProduct } from '@/hooks/queries/useProducts';
import type { Product } from '@/api/types/product.types';

export function ProductsPage() {
  const { data: products = [], isLoading } = useProducts();
  const [search, setSearch] = useState('');

  const [drawerOpen, setDrawerOpen] = useState(false);
  const [editingItem, setEditingItem] = useState<Product | null>(null);

  const [deleteModalOpen, setDeleteModalOpen] = useState(false);
  const [deletingItem, setDeletingItem] = useState<Product | null>(null);

  const deleteMutation = useDeleteProduct(() => {
    setDeleteModalOpen(false);
    setDeletingItem(null);
  });

  const handleCreate = () => {
    setEditingItem(null);
    setDrawerOpen(true);
  };

  const handleEdit = (record: Product) => {
    setEditingItem(record);
    setDrawerOpen(true);
  };

  const handleDelete = (record: Product) => {
    setDeletingItem(record);
    setDeleteModalOpen(true);
  };

  const handleDrawerClose = () => {
    setDrawerOpen(false);
    setEditingItem(null);
  };

  const filteredData = products.filter(
    (item) =>
      item.name.toLowerCase().includes(search.toLowerCase()) ||
      item.categoryName.toLowerCase().includes(search.toLowerCase()),
  );

  const columns = [
    {
      title: 'Ürün Adı',
      dataIndex: 'name',
      key: 'name',
      sorter: (a: Product, b: Product) => a.name.localeCompare(b.name),
    },
    {
      title: 'Kategori',
      dataIndex: 'categoryName',
      key: 'categoryName',
      width: 130,
      render: (val: string) => <Tag>{val}</Tag>,
    },
    {
      title: 'Fiyat',
      dataIndex: 'price',
      key: 'price',
      width: 120,
      sorter: (a: Product, b: Product) => a.price - b.price,
      render: (val: number) =>
        new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(val),
    },
    {
      title: 'Stok',
      dataIndex: 'stock',
      key: 'stock',
      width: 80,
      sorter: (a: Product, b: Product) => a.stock - b.stock,
      render: (val: number) => (
        <span style={{ color: val === 0 ? '#cf1322' : val < 10 ? '#faad14' : undefined }}>
          {val}
        </span>
      ),
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
      onFilter: (value: unknown, record: Product) => record.status === value,
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
      render: (_: unknown, record: Product) => (
        <Space>
          <Button type="text" icon={<EditOutlined />} onClick={() => handleEdit(record)} />
          <Button type="text" danger icon={<DeleteOutlined />} onClick={() => handleDelete(record)} />
        </Space>
      ),
    },
  ];

  return (
    <>
      <PageHeader title="Ürünler" buttonText="Yeni Ürün" onButtonClick={handleCreate} />

      <Input
        placeholder="Ürün veya kategori ara..."
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
        scroll={{ x: 800 }}
      />

      <ProductForm open={drawerOpen} editingItem={editingItem} onClose={handleDrawerClose} />

      <DeleteConfirmModal
        open={deleteModalOpen}
        description={`"${deletingItem?.name}" ürününü silmek istediğinizden emin misiniz?`}
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
