import {
    DashboardOutlined, SafetyCertificateOutlined,
    UserOutlined,
} from '@ant-design/icons';
import type {MenuProps} from 'antd';

type MenuItem = Required<MenuProps>['items'][number];

export const menuItems: MenuItem[] = [
    {
        key: '/',
        icon: <DashboardOutlined/>,
        label: 'Dashboard',
    },
    {
        key: '/personnels',
        icon: <UserOutlined/>,
        label: 'Personnels',
    },
    {key: '/rules', icon: <SafetyCertificateOutlined/>, label: 'Kurallar'},
];
