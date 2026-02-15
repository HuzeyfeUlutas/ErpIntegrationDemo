import { Drawer, Button, Space } from 'antd';
import type { ReactNode } from 'react';

interface FormDrawerProps {
  open: boolean;
  title: string;
  width?: number;
  loading?: boolean;
  onClose: () => void;
  onSubmit: () => void;
  children: ReactNode;
}

export function FormDrawer({
  open,
  title,
  width = 480,
  loading = false,
  onClose,
  onSubmit,
  children,
}: FormDrawerProps) {
  return (
    <Drawer
      title={title}
      open={open}
      onClose={onClose}
      width={width}
      destroyOnClose
      extra={
        <Space>
          <Button onClick={onClose}>İptal</Button>
          <Button type="primary" onClick={onSubmit} loading={loading}>
            Kaydet
          </Button>
        </Space>
      }
    >
      {children}
    </Drawer>
  );
}
