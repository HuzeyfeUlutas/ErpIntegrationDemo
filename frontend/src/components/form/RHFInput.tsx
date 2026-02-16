import {Controller, type Control, type FieldValues, type Path, RegisterOptions} from 'react-hook-form';
import {Form, Input, InputNumber} from 'antd';

interface RHFInputProps<T extends FieldValues> {
    name: Path<T>;
    control: Control<T>;
    label: string;
    placeholder?: string;
    required?: boolean;
    type?: 'text' | 'password' | 'number' | 'email';
    disabled?: boolean;
    rules?: RegisterOptions<T>;
}

export function RHFInput<T extends FieldValues>({
                                                    name,
                                                    control,
                                                    label,
                                                    placeholder,
                                                    required,
                                                    type = 'text',
                                                    disabled,
                                                    rules,
                                                }: RHFInputProps<T>) {
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
                    {type === 'password' ? (
                        <Input.Password
                            {...field}
                            placeholder={placeholder}
                            disabled={disabled}
                        />
                    ) : type === 'number' ? (
                        <InputNumber
                            {...field}
                            placeholder={placeholder}
                            disabled={disabled}
                            style={{width: '100%'}}
                        />
                    ) : (
                        <Input
                            {...field}
                            placeholder={placeholder}
                            type={type}
                            disabled={disabled}
                        />
                    )}
                </Form.Item>
            )}
        />
    );
}
