import { Controller, type Control, type FieldValues, type Path } from 'react-hook-form';
import { Form, Input } from 'antd';

interface RHFTextAreaProps<T extends FieldValues> {
  name: Path<T>;
  control: Control<T>;
  label: string;
  placeholder?: string;
  required?: boolean;
  disabled?: boolean;
  rows?: number;
}

export function RHFTextArea<T extends FieldValues>({
  name,
  control,
  label,
  placeholder,
  required,
  disabled,
  rows = 4,
}: RHFTextAreaProps<T>) {
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
          <Input.TextArea
            {...field}
            placeholder={placeholder}
            disabled={disabled}
            rows={rows}
          />
        </Form.Item>
      )}
    />
  );
}
