import { RouterProvider } from 'react-router-dom';
import { QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { ConfigProvider, App as AntApp } from 'antd';
import trTR from 'antd/locale/tr_TR';
import { queryClient } from '@/config/queryClient';
import { getThemeConfig } from '@/config/theme';
import { useAppStore } from '@/store/useAppStore';
import { router } from '@/router';

export default function App() {
  const themeMode = useAppStore((s) => s.themeMode);

  return (
    <QueryClientProvider client={queryClient}>
      <ConfigProvider theme={getThemeConfig(themeMode)} locale={trTR}>
        <AntApp>
          <RouterProvider router={router} />
        </AntApp>
      </ConfigProvider>
      <ReactQueryDevtools initialIsOpen={false} />
    </QueryClientProvider>
  );
}
