import { createBrowserRouter } from 'react-router-dom';
import { AppLayout } from '@/components/layout/AppLayout';
import { ProtectedRoute } from '@/components/layout/ProtectedRoute';
import { LoginPage } from '@/pages/Login/LoginPage';
import { DashboardPage } from '@/pages/Dashboard/DashboardPage';
import { PersonnelsPage } from '@/pages/Personnels/PersonnelsPage';
import { RulesPage } from '@/pages/Rules/RulesPage';
import {UnauthorizedPage} from "@/pages/UnauthorizedPage/UnauthorizedPage.tsx";
// ← ekle

export const router = createBrowserRouter([
  {
    path: '/login',
    element: <LoginPage />,
  },
  {
    path: '/unauthorized',       // ← ekle
    element: <UnauthorizedPage />,
  },
  {
    path: '/',
    element: (
        <ProtectedRoute>
          <AppLayout />
        </ProtectedRoute>
    ),
    children: [
      { index: true, element: <DashboardPage /> },
      { path: 'personnels', element: <PersonnelsPage /> },
      { path: 'rules', element: <RulesPage /> },
    ],
  },
]);