import { Controller, type Control, type FieldValues, type Path } from 'react-hook-form';
import { Form, Select } from 'antd';
import type { SelectOption } from '@/api/types/common.types';

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
  maxTagCount = 'responsive',
}: RHFMultiSelectProps<T>) {
  return (
    <Controller
      name={name}
      control={control}
      render={({ field, fieldState }) => (
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
            maxTagCount={maxTagCount}
            allowClear
            style={{ width: '100%' }}
          />
        </Form.Item>
      )}
    />
  );
}
