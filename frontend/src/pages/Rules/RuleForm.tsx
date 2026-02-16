import { useEffect, useMemo } from 'react';
import { useForm } from 'react-hook-form';
import { Form } from 'antd';
import { RHFInput } from '@/components/form/RHFInput';
import { RHFSelect } from '@/components/form/RHFSelect';
import { RHFMultiSelect } from '@/components/form/RHFMultiSelect';
import { RHFSwitch } from '@/components/form/RHFSwitch';
import { FormDrawer } from '@/components/shared/FormDrawer';
import { useCreateRule, useUpdateRule } from '@/hooks/queries/useRules';
import { useRolesForSelect } from '@/hooks/queries/useRoles';
import { CAMPUS_OPTIONS, TITLE_OPTIONS } from '@/api/types/common.types';
import type { RuleDto, CreateRuleDto, UpdateRuleDto } from '@/api/types/rule.types';

interface RuleFormProps {
    open: boolean;
    editingItem: RuleDto | null;
    onClose: () => void;
}

interface RuleFormValues {
    name: string;
    campus: string | null;
    title: string | null;
    isActive: boolean;
    roleIds: number[];
}

const CAMPUS_OPTIONS_NULLABLE = [
    { label: 'Tümü', value: '' },
    ...CAMPUS_OPTIONS,
];

const TITLE_OPTIONS_NULLABLE = [
    { label: 'Tümü', value: '' },
    ...TITLE_OPTIONS,
];

export function RuleForm({ open, editingItem, onClose }: RuleFormProps) {
    const isEdit = !!editingItem;

    const { data: roles = [], isLoading: rolesLoading } = useRolesForSelect();

    const roleOptions = useMemo(
        () => roles.map((r) => ({ label: r.name, value: r.id })),
        [roles],
    );

    const { control, handleSubmit, reset } = useForm<RuleFormValues>({
        defaultValues: {
            name: '',
            campus: null,
            title: null,
            isActive: true,
            roleIds: [],
        },
    });

    useEffect(() => {
        if (editingItem) {
            reset({
                name: editingItem.name,
                campus: editingItem.campus ?? '',
                title: editingItem.title ?? '',
                isActive: editingItem.isActive,
                roleIds: editingItem.roles.map((r) => r.id),
            });
        } else {
            reset({
                name: '',
                campus: '',
                title: '',
                isActive: true,
                roleIds: [],
            });
        }
    }, [editingItem, reset]);

    const createMutation = useCreateRule(onClose);
    const updateMutation = useUpdateRule(onClose);

    const onSubmit = (values: RuleFormValues) => {
        const campus = values.campus || null;
        const title = values.title || null;

        if (isEdit) {
            const dto: UpdateRuleDto = {
                id: editingItem!.id,
                name: values.name,
                campus: campus as UpdateRuleDto['campus'],
                title: title as UpdateRuleDto['title'],
                isActive: values.isActive,
                roleIds: values.roleIds,
            };
            updateMutation.mutate(dto);
        } else {
            const dto: CreateRuleDto = {
                name: values.name,
                campus: campus as CreateRuleDto['campus'],
                title: title as CreateRuleDto['title'],
                roleIds: values.roleIds,
            };
            createMutation.mutate(dto);
        }
    };

    return (
        <FormDrawer
            open={open}
            title={isEdit ? 'Kural Düzenle' : 'Yeni Kural'}
            onClose={onClose}
            onSubmit={handleSubmit(onSubmit)}
            loading={createMutation.isPending || updateMutation.isPending}
        >
            <Form layout="vertical">
                <RHFInput
                    name="name"
                    control={control}
                    label="Kural Adı"
                    placeholder="Kural adı giriniz"
                    rules={{
                        required: 'Kural adı zorunludur',
                        maxLength: { value: 200, message: 'En fazla 200 karakter' },
                    }}
                    required
                />

                <RHFSelect
                    name="campus"
                    control={control}
                    label="Kampüs"
                    placeholder="Kampüs seçiniz (boş = tümü)"
                    options={[...CAMPUS_OPTIONS_NULLABLE]}
                    allowClear
                />

                <RHFSelect
                    name="title"
                    control={control}
                    label="Ünvan"
                    placeholder="Ünvan seçiniz (boş = tümü)"
                    options={[...TITLE_OPTIONS_NULLABLE]}
                    allowClear
                />

                <RHFMultiSelect
                    name="roleIds"
                    control={control}
                    label="Roller"
                    options={roleOptions}
                    placeholder="Rol seçiniz"
                    loading={rolesLoading}
                    rules={{
                        required: 'En az bir rol seçilmelidir'
                    }}
                    required
                />

                {isEdit && (
                    <RHFSwitch
                        name="isActive"
                        control={control}
                        label="Aktif"
                    />
                )}
            </Form>
        </FormDrawer>
    );
}