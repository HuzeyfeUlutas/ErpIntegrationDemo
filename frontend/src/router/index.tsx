import { createBrowserRouter } from 'react-router-dom';
import { AppLayout } from '@/components/layout/AppLayout';
import { ProtectedRoute } from '@/components/layout/ProtectedRoute';
import { LoginPage } from '@/pages/Login/LoginPage';
import { DashboardPage } from '@/pages/Dashboard/DashboardPage';
import {PersonnelsPage} from "@/pages/Personnels/PersonnelsPage.tsx";
import {RulesPage} from "@/pages/Rules/RulesPage.tsx";

export const router = createBrowserRouter([
  {
    path: '/login',
    element: <LoginPage />,
  },
  {
    path: '/',
    element: (
      <ProtectedRoute>
        <AppLayout />
      </ProtectedRoute>
    ),
    children: [
      {
        index: true,
        element: <DashboardPage />,
      },
      {
        path: 'personnels',
        element: <PersonnelsPage />,
      },
      { path: 'rules', element: <RulesPage /> },
    ],
  },
]);
