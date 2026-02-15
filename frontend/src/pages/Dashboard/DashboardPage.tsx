import { Card, Col, Row, Statistic, Table, Tag, Spin } from 'antd';
import {
  UserOutlined,
  ShoppingOutlined,
  AppstoreOutlined,
  ThunderboltOutlined,
  WarningOutlined,
} from '@ant-design/icons';
import { useDashboardStats } from '@/hooks/queries/useDashboard';
import { PageHeader } from '@/components/shared/PageHeader';
import type { Activity } from '@/api/types/dashboard.types';
import dayjs from 'dayjs';

const activityTypeMap: Record<Activity['type'], { color: string; label: string }> = {
  user_created: { color: 'green', label: 'Kullanıcı' },
  product_added: { color: 'blue', label: 'Ürün' },
  category_updated: { color: 'orange', label: 'Kategori' },
  user_deleted: { color: 'red', label: 'Silme' },
};

const activityColumns = [
  {
    title: 'Tip',
    dataIndex: 'type',
    key: 'type',
    width: 100,
    render: (type: Activity['type']) => {
      const config = activityTypeMap[type];
      return <Tag color={config.color}>{config.label}</Tag>;
    },
  },
  {
    title: 'Açıklama',
    dataIndex: 'description',
    key: 'description',
  },
  {
    title: 'Tarih',
    dataIndex: 'timestamp',
    key: 'timestamp',
    width: 160,
    render: (val: string) => dayjs(val).format('DD.MM.YYYY HH:mm'),
  },
];

export function DashboardPage() {
  const { data: stats, isLoading } = useDashboardStats();

  if (isLoading) {
    return (
      <div style={{ textAlign: 'center', padding: 80 }}>
        <Spin size="large" />
      </div>
    );
  }

  return (
    <>
      <PageHeader title="Dashboard" />

      <Row gutter={[16, 16]} style={{ marginBottom: 24 }}>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="Toplam Kullanıcı"
              value={stats?.totalUsers}
              prefix={<UserOutlined />}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="Aktif Kullanıcı"
              value={stats?.activeUsers}
              prefix={<ThunderboltOutlined />}
              valueStyle={{ color: '#3f8600' }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="Toplam Ürün"
              value={stats?.totalProducts}
              prefix={<ShoppingOutlined />}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="Düşük Stok"
              value={stats?.lowStockProducts}
              prefix={<WarningOutlined />}
              valueStyle={{ color: '#cf1322' }}
            />
          </Card>
        </Col>
      </Row>

      <Row gutter={[16, 16]}>
        <Col xs={24} lg={16}>
          <Card title="Son Aktiviteler">
            <Table
              columns={activityColumns}
              dataSource={stats?.recentActivities}
              rowKey="id"
              pagination={false}
              size="small"
            />
          </Card>
        </Col>
        <Col xs={24} lg={8}>
          <Card title="Hızlı Bilgi">
            <Statistic
              title="Toplam Kategori"
              value={stats?.totalCategories}
              prefix={<AppstoreOutlined />}
              style={{ marginBottom: 16 }}
            />
            <Statistic
              title="Toplam Ürün"
              value={stats?.totalProducts}
              prefix={<ShoppingOutlined />}
            />
          </Card>
        </Col>
      </Row>
    </>
  );
}
