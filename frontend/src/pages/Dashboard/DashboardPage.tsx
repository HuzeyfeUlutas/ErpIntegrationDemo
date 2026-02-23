import { Card, Col, Row, Statistic, Table, Tag, Spin, Progress, Space } from 'antd';
import {
  TeamOutlined,
  SafetyCertificateOutlined,
  ThunderboltOutlined,
  CheckCircleOutlined,
} from '@ant-design/icons';
import { useDashboard } from '@/hooks/queries/useDashboard';
import { PageHeader } from '@/components/shared/PageHeader';
import dayjs from 'dayjs';
import {EventDto} from "@/api/types/event.types.ts";
import {JobDto} from "@/api/types/job.types.ts";

const eventTypeColorMap: Record<string, { color: string; label: string }> = {
  RuleApplied: { color: 'blue', label: 'Kural Uygulandı' },
  RoleAssigned: { color: 'green', label: 'Rol Atandı' },
  RoleRevoked: { color: 'red', label: 'Rol Kaldırıldı' },
  Sync: { color: 'orange', label: 'Senkronizasyon' },
};

const jobStatusColorMap: Record<string, { color: string; label: string }> = {
  Running: { color: 'processing', label: 'Çalışıyor' },
  Done: { color: 'success', label: 'Tamamlandı' },
  CompletedWithErrors: { color: 'warning', label: 'Kısmi Hata' },
  Failed: { color: 'error', label: 'Başarısız' },
};

const getJobStatusTag = (status: string) => {
  const config = jobStatusColorMap[status];
  if (config) return <Tag color={config.color}>{config.label}</Tag>;
  return <Tag color="default">{status}</Tag>;
};

const getEventTag = (type: string) => {
  const config = eventTypeColorMap[type];
  if (config) return <Tag color={config.color}>{config.label}</Tag>;
  return <Tag color="default">{type}</Tag>;
};

const jobColumns = [
  {
    title: '#',
    key: 'index',
    width: 50,
    render: (_: unknown, __: unknown, index: number) => index + 1,
  },
  {
    title: 'Tip',
    dataIndex: 'jobType',
    key: 'jobType',
    width: 160,
    render: (type: string) => <Tag color="purple">{type}</Tag>,
  },
  {
    title: 'Durum',
    dataIndex: 'status',
    key: 'status',
    width: 140,
    render: (status: string) => getJobStatusTag(status),
  },
  {
    title: 'Sonuç',
    key: 'result',
    width: 200,
    render: (_: unknown, record: JobDto) => {
      if (record.status === 'Running') {
        return <Tag color="processing">Devam ediyor...</Tag>;
      }
      const total = record.totalCount || 1;
      const percent = Math.round((record.successCount / total) * 100);
      return (
          <Space size={4}>
            <Progress
                type="circle"
                size={28}
                percent={percent}
                strokeColor={percent === 100 ? '#52c41a' : '#faad14'}
            />
            <span style={{ fontSize: 12 }}>
            {record.successCount}/{record.totalCount}
          </span>
            {record.failureCount > 0 && (
                <Tag color="error" style={{ fontSize: 11 }}>{record.failureCount} hata</Tag>
            )}
          </Space>
      );
    },
  },
  {
    title: 'Başlangıç',
    dataIndex: 'createdAt',
    key: 'createdAt',
    width: 150,
    render: (val: string) => dayjs(val).format('DD.MM.YYYY HH:mm'),
  },
  {
    title: 'Bitiş',
    dataIndex: 'updatedAt',
    key: 'updatedAt',
    width: 150,
    render: (val: string | null) => val ? dayjs(val).format('DD.MM.YYYY HH:mm') : '-',
  },
];

const eventColumns = [
  {
    title: '#',
    key: 'index',
    width: 50,
    render: (_: unknown, __: unknown, index: number) => index + 1,
  },
  {
    title: 'Tip',
    dataIndex: 'eventType',
    key: 'eventType',
    width: 160,
    render: (type: string) => getEventTag(type),
  },
  {
    title: 'Kaynak',
    dataIndex: 'sourceId',
    key: 'sourceId',
    width: 140,
  },
  {
    title: 'Detay',
    dataIndex: 'sourceDetail',
    key: 'sourceDetail',
    ellipsis: true,
    render: (val: string | null) => val ?? '-',
  },
  {
    title: 'Sonuç',
    key: 'result',
    width: 180,
    render: (_: unknown, record: EventDto) => {
      if (!record.isCompleted) {
        return <Tag color="processing">Devam ediyor...</Tag>;
      }
      const total = record.totalCount || 1;
      const percent = Math.round((record.successCount / total) * 100);
      return (
          <Space size={4}>
            <Progress
                type="circle"
                size={28}
                percent={percent}
                strokeColor={percent === 100 ? '#52c41a' : '#faad14'}
            />
            <span style={{ fontSize: 12 }}>
            {record.successCount}/{record.totalCount}
          </span>
            {record.failCount > 0 && (
                <Tag color="error" style={{ fontSize: 11 }}>{record.failCount} hata</Tag>
            )}
          </Space>
      );
    },
  },
  {
    title: 'Durum',
    dataIndex: 'isCompleted',
    key: 'isCompleted',
    width: 100,
    render: (val: boolean) =>
        val
            ? <Tag color="success">Tamamlandı</Tag>
            : <Tag color="processing">İşleniyor</Tag>,
  },
  {
    title: 'Tarih',
    dataIndex: 'occurredAt',
    key: 'occurredAt',
    width: 150,
    render: (val: string) => dayjs(val).format('DD.MM.YYYY HH:mm'),
  },
];

export function DashboardPage() {
  const { data, isLoading } = useDashboard();

  if (isLoading && !data) {
    return (
        <div style={{ textAlign: 'center', padding: 80 }}>
          <Spin size="large" />
        </div>
    );
  }

  const stats = data?.stats;

  return (
      <>
        <PageHeader title="Dashboard" />

        <Row gutter={[16, 16]} style={{ marginBottom: 24 }}>
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                  title="Toplam Personel"
                  value={stats?.totalPersonnel ?? 0}
                  prefix={<TeamOutlined />}
              />
            </Card>
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                  title="Toplam Kural"
                  value={stats?.totalRules ?? 0}
                  prefix={<SafetyCertificateOutlined />}
                  suffix={
                    <span style={{ fontSize: 14, color: '#52c41a' }}>
                  ({stats?.activeRules ?? 0} aktif)
                </span>
                  }
              />
            </Card>
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                  title="Toplam Event"
                  value={stats?.totalEvents ?? 0}
                  prefix={<ThunderboltOutlined />}
                  valueStyle={{ color: '#1677ff' }}
              />
            </Card>
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                  title="Toplam Job"
                  value={stats?.totalJobs ?? 0}
                  prefix={<CheckCircleOutlined />}
                  valueStyle={{ color: '#52c41a' }}
              />
            </Card>
          </Col>
        </Row>

        <Card title="Son Eventler">
          <Table
              columns={eventColumns}
              dataSource={data?.recentEvents ?? []}
              rowKey="id"
              pagination={false}
              size="small"
              scroll={{ x: 900 }}
          />
        </Card>
        <div style={{marginTop: 20}}>

        </div>

        <Card title="Son Joblar">
          <Table
              columns={jobColumns}
              dataSource={data?.recentJobs ?? []}
              rowKey="id"
              pagination={false}
              size="small"
              scroll={{ x: 900 }}
          />
        </Card>
      </>
  );
}