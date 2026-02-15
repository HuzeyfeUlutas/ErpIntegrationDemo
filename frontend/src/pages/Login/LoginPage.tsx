import { useForm } from 'react-hook-form';
import { Card, Typography, Form, Flex } from 'antd';
import { LockOutlined } from '@ant-design/icons';
import { Navigate } from 'react-router-dom';
import { RHFInput } from '@/components/form/RHFInput';
import { useLogin } from '@/hooks/useAuth';
import { useAuthStore } from '@/store/useAuthStore';
import { useAppStore } from '@/store/useAppStore';
import type { LoginRequest } from '@/api/types/auth.types';

export function LoginPage() {
  const isAuthenticated = useAuthStore((s) => s.isAuthenticated);
  const themeMode = useAppStore((s) => s.themeMode);
  const loginMutation = useLogin();

  const { control, handleSubmit } = useForm<LoginRequest>({
    defaultValues: {
      email: 'admin@admin.com',
      password: '123456',
    },
  });

  if (isAuthenticated) {
    return <Navigate to="/" replace />;
  }

  const onSubmit = (data: LoginRequest) => {
    loginMutation.mutate(data);
  };

  const isDark = themeMode === 'dark';

  return (
    <Flex
      justify="center"
      align="center"
      style={{
        minHeight: '100vh',
        background: isDark ? '#000' : '#f0f2f5',
      }}
    >
      <Card
        style={{ width: 400 }}
        styles={{ body: { paddingTop: 32 } }}
      >
        <Flex vertical align="center" gap={8} style={{ marginBottom: 32 }}>
          <LockOutlined style={{ fontSize: 32, color: '#1677ff' }} />
          <Typography.Title level={3} style={{ margin: 0 }}>
            Admin Panel
          </Typography.Title>
          <Typography.Text type="secondary">
            Devam etmek için giriş yapın
          </Typography.Text>
        </Flex>

        <Form layout="vertical" onFinish={handleSubmit(onSubmit)}>
          <RHFInput
            name="email"
            control={control}
            label="E-posta"
            placeholder="E-posta adresiniz"
            type="email"
            required
          />

          <RHFInput
            name="password"
            control={control}
            label="Şifre"
            placeholder="Şifreniz"
            type="password"
            required
          />

          <Form.Item style={{ marginBottom: 0 }}>
            <button
              type="submit"
              disabled={loginMutation.isPending}
              style={{
                width: '100%',
                height: 40,
                background: '#1677ff',
                color: '#fff',
                border: 'none',
                borderRadius: 6,
                fontSize: 15,
                cursor: loginMutation.isPending ? 'not-allowed' : 'pointer',
                opacity: loginMutation.isPending ? 0.7 : 1,
              }}
            >
              {loginMutation.isPending ? 'Giriş yapılıyor...' : 'Giriş Yap'}
            </button>
          </Form.Item>
        </Form>

        <Typography.Text
          type="secondary"
          style={{ display: 'block', textAlign: 'center', marginTop: 16, fontSize: 12 }}
        >
          Demo: admin@admin.com / 123456
        </Typography.Text>
      </Card>
    </Flex>
  );
}
