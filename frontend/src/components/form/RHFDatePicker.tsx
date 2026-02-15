import { Controller, type Control, type FieldValues, type Path } from 'react-hook-form';
import { Form, DatePicker } from 'antd';
import dayjs from 'dayjs';

interface RHFDatePickerProps<T extends FieldValues> {
  name: Path<T>;
  control: Control<T>;
  label: string;
  placeholder?: string;
  required?: boolean;
  disabled?: boolean;
  showTime?: boolean;
}

export function RHFDatePicker<T extends FieldValues>({
  name,
  control,
  label,
  placeholder = 'Tarih seçiniz',
  required,
  disabled,
  showTime,
}: RHFDatePickerProps<T>) {
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
          <DatePicker
            {...field}
            value={field.value ? dayjs(field.value as string) : null}
            onChange={(_date, dateString) => field.onChange(dateString)}
            placeholder={placeholder}
            disabled={disabled}
            showTime={showTime}
            style={{ width: '100%' }}
          />
        </Form.Item>
      )}
    />
  );
}
