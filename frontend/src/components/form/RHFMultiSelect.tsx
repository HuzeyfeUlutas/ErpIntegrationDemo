import {Controller, type Control, type FieldValues, type Path, RegisterOptions} from 'react-hook-form';
import {Form, Select} from 'antd';
import type {SelectOption} from '@/api/types/common.types';

interface RHFMultiSelectProps<T extends FieldValues> {
    name: Path<T>;
    control: Control<T>;
    label: string;
    options: SelectOption[];
    placeholder?: string;
    required?: boolean;
    disabled?: boolean;
    loading?: boolean;
    maxTagCount?: number | 'responsive';
    rules?: RegisterOptions<T>;
}

export function RHFMultiSelect<T extends FieldValues>({
                                                          name,
                                                          control,
                                                          label,
                                                          options,
                                                          placeholder = 'Seçiniz...',
                                                          required,
                                                          disabled,
                                                          loading,
                                                          rules,
                                                      }: RHFMultiSelectProps<T>) {
    return (
        <Controller
            name={name}
            control={control}
            rules={rules}
            render={({field, fieldState}) => (
                <Form.Item
                    label={label}
                    required={required}
                    validateStatus={fieldState.error ? 'error' : ''}
                    help={fieldState.error?.message}
                >
                    <Select
                        {...field}
                        mode="multiple"
                        showSearch
                        optionFilterProp="label"
                        placeholder={placeholder}
                        options={options}
                        disabled={disabled}
                        loading={loading}
                        allowClear
                        style={{ width: '100%' }}
                    />
                </Form.Item>
            )}
        />
    );
}
