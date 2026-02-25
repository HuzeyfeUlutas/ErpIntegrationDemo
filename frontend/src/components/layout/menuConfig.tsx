import {
    CloudServerOutlined,
    DashboardOutlined, RocketOutlined, SafetyCertificateOutlined, ThunderboltOutlined,
    UserOutlined,
} from '@ant-design/icons';
import type {MenuProps} from 'antd';

type MenuItem = Required<MenuProps>['items'][number];

export const menuItems: MenuItem[] = [
    {
        key: '/',
        icon: <DashboardOutlined/>,
        label: 'Panel',
    },
    {
        key: '/personnels',
        icon: <UserOutlined/>,
        label: 'Personeller',
    },
    {key: '/rules', icon: <SafetyCertificateOutlined/>, label: 'Kurallar'},
    { key: '/kafka-events', icon: <CloudServerOutlined />, label: 'SAP Personel İşlemleri' },
    { key: '/events', icon: <ThunderboltOutlined />, label: 'Events' },
    { key: '/jobs', icon: <RocketOutlined />, label: 'Scheduled Jobs' }

];
