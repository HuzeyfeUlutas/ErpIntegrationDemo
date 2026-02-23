import { useEffect, useMemo } from 'react';
import { useForm } from 'react-hook-form';
import { Form, Descriptions } from 'antd';
import { RHFMultiSelect } from '@/components/form/RHFMultiSelect';
import { FormDrawer } from '@/components/shared/FormDrawer';
import { useUpdatePersonnel } from '@/hooks/queries/usePersonnels';
import { useRolesForSelect } from '@/hooks/queries/useRoles';
import type { PersonnelDto, UpdatePersonnelDto } from '@/api/types/personnel.types';
import {CAMPUS_LABEL, TITLE_LABEL} from "@/api/types/common.types.ts";

interface PersonnelFormProps {
    open: boolean;
    editingItem: PersonnelDto | null;
    onClose: () => void;
}

export function PersonnelForm({ open, editingItem, onClose }: PersonnelFormProps) {
    const { data: roles = [], isLoading: rolesLoading } = useRolesForSelect();

    const roleOptions = useMemo(
        () => roles.map((r) => ({ label: r.name, value: r.id })),
        [roles],
    );

    const { control, handleSubmit, reset } = useForm<UpdatePersonnelDto>({
        defaultValues: {
            employeeNo: 0,
            roleIds: [],
        },
    });

    useEffect(() => {
        if (editingItem) {
            reset({
                employeeNo: editingItem.employeeNo,
                roleIds: editingItem.roles.map((r) => r.id),
            });
        }
    }, [editingItem, reset]);

    const updateMutation = useUpdatePersonnel(onClose);

    const onSubmit = (data: UpdatePersonnelDto) => {
        updateMutation.mutate(data);
    };

    return (
        <FormDrawer
            open={open}
            title="Rol Atama"
            onClose={onClose}
            onSubmit={handleSubmit(onSubmit)}
            loading={updateMutation.isPending}
        >
            {/* Personel bilgileri read-only göster */}
            {editingItem && (
                <Descriptions column={1} size="small" style={{ marginBottom: 24 }}>
                    <Descriptions.Item label="Sicil No">{editingItem.employeeNo}</Descriptions.Item>
                    <Descriptions.Item label="Ad Soyad">{editingItem.fullName}</Descriptions.Item>
                    <Descriptions.Item label="Kampüs">{CAMPUS_LABEL[editingItem.campus]}</Descriptions.Item>
                    <Descriptions.Item label="Unvan">{TITLE_LABEL[editingItem.title]}</Descriptions.Item>
                </Descriptions>
            )}

            <Form layout="vertical">
                <RHFMultiSelect
                    name="roleIds"
                    control={control}
                    label="Roller"
                    options={roleOptions}
                    placeholder="Rol seçiniz"
                    loading={rolesLoading}
                    required
                />
            </Form>
        </FormDrawer>
    );
}