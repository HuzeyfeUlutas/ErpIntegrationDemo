import { Controller, type Control, type FieldValues, type Path } from 'react-hook-form';
import { Form, Select } from 'antd';
import type { SelectOption } from '@/api/types/common.types';

interface RHFSelectProps<T extends FieldValues> {
  name: Path<T>;
  control: Control<T>;
  label: string;
  options: SelectOption[];
  placeholder?: string;
  required?: boolean;
  disabled?: boolean;
  loading?: boolean;
  allowClear?: boolean;
}

export function RHFSelect<T extends FieldValues>({
  name,
  control,
  label,
  options,
  placeholder = 'Seçiniz...',
  required,
  disabled,
  loading,
  allowClear = true,
}: RHFSelectProps<T>) {
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
            showSearch
            optionFilterProp="label"
            placeholder={placeholder}
            options={options}
            disabled={disabled}
            loading={loading}
            allowClear={allowClear}
            style={{ width: '100%' }}
          />
        </Form.Item>
      )}
    />
  );
}
