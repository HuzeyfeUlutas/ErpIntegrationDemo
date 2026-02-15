import { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { Form } from 'antd';
import { RHFInput } from '@/components/form/RHFInput';
import { RHFSelect } from '@/components/form/RHFSelect';
import { RHFMultiSelect } from '@/components/form/RHFMultiSelect';
import { RHFSwitch } from '@/components/form/RHFSwitch';
import { FormDrawer } from '@/components/shared/FormDrawer';
import { useCreateUser, useUpdateUser } from '@/hooks/queries/useUsers';
import { USER_ROLE_OPTIONS, USER_TAG_OPTIONS } from '@/api/types/user.types';
import type { User, CreateUserDto } from '@/api/types/user.types';

interface UserFormProps {
  open: boolean;
  editingItem: User | null;
  onClose: () => void;
}

export function UserForm({ open, editingItem, onClose }: UserFormProps) {
  const isEdit = !!editingItem;

  const { control, handleSubmit, reset } = useForm<CreateUserDto>({
    defaultValues: {
      name: '',
      email: '',
      phone: '',
      role: 'viewer',
      tags: [],
      status: 'active',
    },
  });

  useEffect(() => {
    if (editingItem) {
      reset({
        name: editingItem.name,
        email: editingItem.email,
        phone: editingItem.phone,
        role: editingItem.role,
        tags: editingItem.tags,
        status: editingItem.status,
      });
    } else {
      reset({ name: '', email: '', phone: '', role: 'viewer', tags: [], status: 'active' });
    }
  }, [editingItem, reset]);

  const createMutation = useCreateUser(onClose);
  const updateMutation = useUpdateUser(onClose);

  const isLoading = createMutation.isPending || updateMutation.isPending;

  const onSubmit = (data: CreateUserDto) => {
    if (isEdit && editingItem) {
      updateMutation.mutate({ id: editingItem.id, data });
    } else {
      createMutation.mutate(data);
    }
  };

  return (
    <FormDrawer
      open={open}
      title={isEdit ? 'Kullanıcı Düzenle' : 'Yeni Kullanıcı'}
      onClose={onClose}
      onSubmit={handleSubmit(onSubmit)}
      loading={isLoading}
    >
      <Form layout="vertical">
        <RHFInput
          name="name"
          control={control}
          label="Ad Soyad"
          placeholder="Kullanıcı adını giriniz"
          required
        />

        <RHFInput
          name="email"
          control={control}
          label="E-posta"
          placeholder="E-posta adresi"
          type="email"
          required
        />

        <RHFInput
          name="phone"
          control={control}
          label="Telefon"
          placeholder="0532 000 0000"
        />

        <RHFSelect
          name="role"
          control={control}
          label="Rol"
          options={[...USER_ROLE_OPTIONS]}
          placeholder="Rol seçiniz"
          required
          allowClear={false}
        />

        <RHFMultiSelect
          name="tags"
          control={control}
          label="Etiketler"
          options={[...USER_TAG_OPTIONS]}
          placeholder="Etiket seçiniz"
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
