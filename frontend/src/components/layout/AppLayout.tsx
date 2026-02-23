import { useState } from 'react';
import { Outlet, useNavigate, useLocation } from 'react-router-dom';
import { Layout, Menu, Button, Dropdown, Avatar, Flex, Typography, Switch } from 'antd';
import {
  MenuFoldOutlined,
  MenuUnfoldOutlined,
  UserOutlined,
  LogoutOutlined,
  SunOutlined,
  MoonOutlined,
} from '@ant-design/icons';
import { useAppStore } from '@/store/useAppStore';
import { useAuthStore } from '@/store/useAuthStore';
import { useLogout } from '@/hooks/useAuth';
import { menuItems } from './menuConfig';

const { Header, Sider, Content } = Layout;

export function AppLayout() {
  const navigate = useNavigate();
  const location = useLocation();
  const logout = useLogout();
  const user = useAuthStore((s) => s.user);
  const { sidebarCollapsed, toggleSidebar, themeMode, toggleTheme } = useAppStore();
  const [selectedKey, setSelectedKey] = useState(location.pathname);

  const handleMenuClick = ({ key }: { key: string }) => {
    setSelectedKey(key);
    navigate(key);
  };

  const userMenuItems = [
    {
      key: 'logout',
      icon: <LogoutOutlined />,
      label: 'Çıkış Yap',
      onClick: logout,
    },
  ];

  const isDark = themeMode === 'dark';

  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Sider
        trigger={null}
        collapsible
        collapsed={sidebarCollapsed}
        breakpoint="lg"
        width={240}
        onBreakpoint={(broken) => {
          if (broken) toggleSidebar();
        }}
        theme="dark"
        style={{
          overflow: 'auto',
          height: '100vh',
          position: 'fixed',
          left: 0,
          top: 0,
          bottom: 0,
          zIndex: 100,
        }}
      >
        <Flex
          align="center"
          justify="center"
          style={{
            height: 64,
            borderBottom: '1px solid rgba(255,255,255,0.1)',
          }}
        >
          <Typography.Text
            strong
            style={{ color: '#fff', fontSize: sidebarCollapsed ? 16 : 20 }}
          >
            {sidebarCollapsed ? 'AP' : 'Admin Panel'}
          </Typography.Text>
        </Flex>

        <Menu
          theme="dark"
          mode="inline"
          selectedKeys={[selectedKey]}
          items={menuItems}
          onClick={handleMenuClick}
          style={{ borderRight: 0 }}
        />
      </Sider>

      <Layout style={{ marginLeft: sidebarCollapsed ? 80 : 240, transition: 'all 0.2s' }}>
        <Header
          style={{
            padding: '0 24px',
            background: isDark ? '#1f1f1f' : '#fff',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
            borderBottom: `1px solid ${isDark ? '#303030' : '#f0f0f0'}`,
            position: 'sticky',
            top: 0,
            zIndex: 99,
          }}
        >
          <Button
            type="text"
            icon={sidebarCollapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />}
            onClick={toggleSidebar}
            style={{ fontSize: 16 }}
          />

          <Flex align="center" gap={16}>
            <Switch
              checked={isDark}
              onChange={toggleTheme}
              checkedChildren={<MoonOutlined />}
              unCheckedChildren={<SunOutlined />}
            />

            <Dropdown menu={{ items: userMenuItems }} placement="bottomRight">
              <Flex align="center" gap={8} style={{ cursor: 'pointer' }}>
                <Avatar icon={<UserOutlined />} size="small" />
                <Typography.Text style={{ color: isDark ? '#fff' : undefined }}>
                  {user?.fullName ?? 'Kullanıcı'}
                </Typography.Text>
              </Flex>
            </Dropdown>
          </Flex>
        </Header>

        <Content
          style={{
            margin: 24,
            padding: 24,
            background: isDark ? '#141414' : '#fff',
            borderRadius: 8,
            minHeight: 280,
          }}
        >
          <Outlet />
        </Content>
      </Layout>
    </Layout>
  );
}
