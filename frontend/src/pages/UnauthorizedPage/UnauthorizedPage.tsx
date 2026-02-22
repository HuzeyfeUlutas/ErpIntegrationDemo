import { Button, Result } from 'antd';
import { useNavigate } from 'react-router-dom';
import { useLogout } from '@/hooks/useAuth';

export function UnauthorizedPage() {
    const navigate = useNavigate();
    const logout = useLogout();

    return (
        <Result
            status="403"
            title="Yetkiniz Yok"
            subTitle="Bu sayfaya erişim yetkiniz bulunmamaktadır. Yalnızca admin kullanıcılar erişebilir."
            extra={[
                <Button key="logout" type="primary" onClick={logout}>
                    Çıkış Yap
                </Button>,
                <Button key="back" onClick={() => navigate(-1)}>
                    Geri Dön
                </Button>,
            ]}
        />
    );
}