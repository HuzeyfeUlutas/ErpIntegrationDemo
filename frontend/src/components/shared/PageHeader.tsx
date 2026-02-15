import { Button, Flex, Typography } from 'antd';
import { PlusOutlined } from '@ant-design/icons';

interface PageHeaderProps {
  title: string;
  buttonText?: string;
  onButtonClick?: () => void;
}

export function PageHeader({ title, buttonText, onButtonClick }: PageHeaderProps) {
  return (
    <Flex justify="space-between" align="center" style={{ marginBottom: 24 }}>
      <Typography.Title level={4} style={{ margin: 0 }}>
        {title}
      </Typography.Title>
      {buttonText && onButtonClick && (
        <Button type="primary" icon={<PlusOutlined />} onClick={onButtonClick}>
          {buttonText}
        </Button>
      )}
    </Flex>
  );
}
