import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import type { AuthUser, AuthResponse } from '@/api/types/auth.types';

interface AuthState {
    accessToken: string | null;
    refreshToken: string | null;
    user: AuthUser | null;
    isAuthenticated: boolean;
    setAuth: (response: AuthResponse) => void;
    logout: () => void;
}

/**
 * JWT payload'dan claim'leri parse eder.
 */
function parseJwt(token: string): Record<string, unknown> {
    try {
        const base64Url = token.split('.')[1];
        if (!base64Url) return {};

        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(
            atob(base64)
                .split('')
                .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
                .join(''),
        );
        return JSON.parse(jsonPayload);
    } catch {
        return {};
    }
}

/**
 * JWT'den AuthUser bilgilerini çıkarır.
 */
function extractUser(accessToken: string, employeeNo: string, fullName: string): AuthUser {
    const claims = parseJwt(accessToken);

    // Backend: ClaimTypes.Role → "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
    const roleClaim =
        claims['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

    const isAdmin = Array.isArray(roleClaim)
        ? roleClaim.includes('Admin')
        : roleClaim === 'Admin';

    return {
        employeeNo,
        fullName,
        isAdmin,
    };
}

export const useAuthStore = create<AuthState>()(
    persist(
        (set) => ({
            accessToken: null,
            refreshToken: null,
            user: null,
            isAuthenticated: false,

            setAuth: (response: AuthResponse) => {
                const user = extractUser(response.accessToken, response.employeeNo, response.fullName);
                set({
                    accessToken: response.accessToken,
                    refreshToken: response.refreshToken,
                    user,
                    isAuthenticated: true,
                });
            },

            logout: () =>
                set({
                    accessToken: null,
                    refreshToken: null,
                    user: null,
                    isAuthenticated: false,
                }),
        }),
        {
            name: 'auth-storage',
            partialize: (state) => ({
                accessToken: state.accessToken,
                refreshToken: state.refreshToken,
                user: state.user,
                isAuthenticated: state.isAuthenticated,
            }),
        },
    ),
);