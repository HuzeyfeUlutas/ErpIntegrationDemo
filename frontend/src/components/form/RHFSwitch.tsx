import { Controller, type Control, type FieldValues, type Path } from 'react-hook-form';
import { Form, Switch } from 'antd';

interface RHFSwitchProps<T extends FieldValues> {
  name: Path<T>;
  control: Control<T>;
  label: string;
  checkedLabel?: string;
  uncheckedLabel?: string;
  disabled?: boolean;
}

export function RHFSwitch<T extends FieldValues>({
  name,
  control,
  label,
  checkedLabel = 'Aktif',
  uncheckedLabel = 'Pasif',
  disabled,
}: RHFSwitchProps<T>) {
  return (
    <Controller
      name={name}
      control={control}
      render={({ field, fieldState }) => (
        <Form.Item
          label={label}
          validateStatus={fieldState.error ? 'error' : ''}
          help={fieldState.error?.message}
        >
          <Switch
            checked={field.value === 'active'}
            onChange={(checked) => field.onChange(checked ? 'active' : 'inactive')}
            checkedChildren={checkedLabel}
            unCheckedChildren={uncheckedLabel}
            disabled={disabled}
          />
        </Form.Item>
      )}
    />
  );
}
