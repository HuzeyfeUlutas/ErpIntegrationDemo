import { theme } from 'antd';
import type { ThemeConfig } from 'antd';
import type { ThemeMode } from '@/store/useAppStore';

export const getThemeConfig = (mode: ThemeMode): ThemeConfig => ({
  algorithm: mode === 'dark' ? theme.darkAlgorithm : theme.defaultAlgorithm,
  token: {
    colorPrimary: '#1677ff',
    borderRadius: 6,
    fontFamily:
      "-apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif",
  },
  components: {
    Layout: {
      siderBg: mode === 'dark' ? '#141414' : '#001529',
      headerBg: mode === 'dark' ? '#1f1f1f' : '#fff',
      bodyBg: mode === 'dark' ? '#000000' : '#f5f5f5',
    },
    Menu: {
      darkItemBg: mode === 'dark' ? '#141414' : '#001529',
    },
  },
});
