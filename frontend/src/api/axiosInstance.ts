import axios from 'axios';
import { useAuthStore } from '@/store/useAuthStore';

const axiosInstance = axios.create({
    baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5000/api',
    headers: { 'Content-Type': 'application/json' },
});

// ── Request interceptor — her isteğe token ekle ─────────────
axiosInstance.interceptors.request.use((config) => {
    const token = useAuthStore.getState().accessToken;
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

// ── Response interceptor — 401'de refresh dene ──────────────
let isRefreshing = false;
let failedQueue: Array<{
    resolve: (token: string) => void;
    reject: (error: unknown) => void;
}> = [];

const processQueue = (error: unknown, token: string | null) => {
    failedQueue.forEach((prom) => {
        if (token) prom.resolve(token);
        else prom.reject(error);
    });
    failedQueue = [];
};

axiosInstance.interceptors.response.use(
    (response) => response,
    async (error) => {
        const originalRequest = error.config;

        // 401 değilse veya login/refresh isteğiyse direkt reject
        if (
            error.response?.status !== 401 ||
            originalRequest._retry ||
            originalRequest.url?.includes('/auth/login') ||
            originalRequest.url?.includes('/auth/refresh')
        ) {
            return Promise.reject(error);
        }

        // Zaten refresh yapılıyorsa kuyruğa ekle
        if (isRefreshing) {
            return new Promise((resolve, reject) => {
                failedQueue.push({
                    resolve: (token: string) => {
                        originalRequest.headers.Authorization = `Bearer ${token}`;
                        resolve(axiosInstance(originalRequest));
                    },
                    reject,
                });
            });
        }

        originalRequest._retry = true;
        isRefreshing = true;

        const { refreshToken, setAuth, logout } = useAuthStore.getState();

        if (!refreshToken) {
            logout();
            window.location.href = '/login';
            return Promise.reject(error);
        }

        try {
            const { data } = await axios.post(
                `${axiosInstance.defaults.baseURL}/auth/refresh`,
                { refreshToken },
                { headers: { 'Content-Type': 'application/json' } },
            );

            setAuth(data);
            processQueue(null, data.accessToken);

            originalRequest.headers.Authorization = `Bearer ${data.accessToken}`;
            return axiosInstance(originalRequest);
        } catch (refreshError) {
            processQueue(refreshError, null);
            logout();
            window.location.href = '/login';
            return Promise.reject(refreshError);
        } finally {
            isRefreshing = false;
        }
    },
);

export default axiosInstance;