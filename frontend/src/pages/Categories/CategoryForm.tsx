import { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { Form } from 'antd';
import { RHFInput } from '@/components/form/RHFInput';
import { RHFTextArea } from '@/components/form/RHFTextArea';
import { RHFSwitch } from '@/components/form/RHFSwitch';
import { FormDrawer } from '@/components/shared/FormDrawer';
import { useCreateCategory, useUpdateCategory } from '@/hooks/queries/useCategories';
import type { Category, CreateCategoryDto } from '@/api/types/category.types';

interface CategoryFormProps {
  open: boolean;
  editingItem: Category | null;
  onClose: () => void;
}

export function CategoryForm({ open, editingItem, onClose }: CategoryFormProps) {
  const isEdit = !!editingItem;

  const { control, handleSubmit, reset } = useForm<CreateCategoryDto>({
    defaultValues: {
      name: '',
      description: '',
      status: 'active',
    },
  });

  useEffect(() => {
    if (editingItem) {
      reset({
        name: editingItem.name,
        description: editingItem.description,
        status: editingItem.status,
      });
    } else {
      reset({ name: '', description: '', status: 'active' });
    }
  }, [editingItem, reset]);

  const createMutation = useCreateCategory(onClose);
  const updateMutation = useUpdateCategory(onClose);

  const isLoading = createMutation.isPending || updateMutation.isPending;

  const onSubmit = (data: CreateCategoryDto) => {
    if (isEdit && editingItem) {
      updateMutation.mutate({ id: editingItem.id, data });
    } else {
      createMutation.mutate(data);
    }
  };

  return (
    <FormDrawer
      open={open}
      title={isEdit ? 'Kategori Düzenle' : 'Yeni Kategori'}
      onClose={onClose}
      onSubmit={handleSubmit(onSubmit)}
      loading={isLoading}
    >
      <Form layout="vertical">
        <RHFInput
          name="name"
          control={control}
          label="Kategori Adı"
          placeholder="Kategori adını giriniz"
          required
        />

        <RHFTextArea
          name="description"
          control={control}
          label="Açıklama"
          placeholder="Kategori açıklaması"
          rows={3}
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
