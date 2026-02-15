import { useEffect, useMemo } from 'react';
import { useForm } from 'react-hook-form';
import { Form } from 'antd';
import { RHFInput } from '@/components/form/RHFInput';
import { RHFTextArea } from '@/components/form/RHFTextArea';
import { RHFSelect } from '@/components/form/RHFSelect';
import { RHFSwitch } from '@/components/form/RHFSwitch';
import { FormDrawer } from '@/components/shared/FormDrawer';
import { useCreateProduct, useUpdateProduct } from '@/hooks/queries/useProducts';
import { useCategories } from '@/hooks/queries/useCategories';
import type { Product, CreateProductDto } from '@/api/types/product.types';

interface ProductFormProps {
  open: boolean;
  editingItem: Product | null;
  onClose: () => void;
}

export function ProductForm({ open, editingItem, onClose }: ProductFormProps) {
  const isEdit = !!editingItem;
  const { data: categories = [], isLoading: categoriesLoading } = useCategories();

  const categoryOptions = useMemo(
    () =>
      categories
        .filter((c) => c.status === 'active')
        .map((c) => ({ label: c.name, value: c.id })),
    [categories],
  );

  const { control, handleSubmit, reset } = useForm<CreateProductDto>({
    defaultValues: {
      name: '',
      description: '',
      price: 0,
      stock: 0,
      categoryId: '',
      status: 'active',
    },
  });

  useEffect(() => {
    if (editingItem) {
      reset({
        name: editingItem.name,
        description: editingItem.description,
        price: editingItem.price,
        stock: editingItem.stock,
        categoryId: editingItem.categoryId,
        status: editingItem.status,
      });
    } else {
      reset({ name: '', description: '', price: 0, stock: 0, categoryId: '', status: 'active' });
    }
  }, [editingItem, reset]);

  const createMutation = useCreateProduct(onClose);
  const updateMutation = useUpdateProduct(onClose);

  const isLoading = createMutation.isPending || updateMutation.isPending;

  const onSubmit = (data: CreateProductDto) => {
    if (isEdit && editingItem) {
      updateMutation.mutate({ id: editingItem.id, data });
    } else {
      createMutation.mutate(data);
    }
  };

  return (
    <FormDrawer
      open={open}
      title={isEdit ? 'Ürün Düzenle' : 'Yeni Ürün'}
      onClose={onClose}
      onSubmit={handleSubmit(onSubmit)}
      loading={isLoading}
    >
      <Form layout="vertical">
        <RHFInput
          name="name"
          control={control}
          label="Ürün Adı"
          placeholder="Ürün adını giriniz"
          required
        />

        <RHFTextArea
          name="description"
          control={control}
          label="Açıklama"
          placeholder="Ürün açıklaması"
          rows={3}
        />

        <RHFInput
          name="price"
          control={control}
          label="Fiyat (₺)"
          placeholder="0"
          type="number"
          required
        />

        <RHFInput
          name="stock"
          control={control}
          label="Stok"
          placeholder="0"
          type="number"
          required
        />

        <RHFSelect
          name="categoryId"
          control={control}
          label="Kategori"
          placeholder="Kategori seçiniz"
          options={categoryOptions}
          loading={categoriesLoading}
          required
        />

        <RHFSwitch
          name="status"
          control={control}
          label="Durum"
        />
      </Form>
    </FormDrawer>
  );
}
