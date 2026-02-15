# Admin Panel

React + TypeScript + Ant Design ile oluşturulmuş admin paneli.

## Teknolojiler

- **React 19** + **TypeScript 5**
- **Ant Design 5** (UI kütüphanesi)
- **React Router v7** (routing)
- **React Hook Form** (form yönetimi)
- **TanStack React Query v5** (veri çekme / cache)
- **Zustand v5** (state management)
- **Axios** (HTTP client)
- **Vite 6** (build tool)

## Kurulum

```bash
npm install
npm run dev
```

## Demo Giriş Bilgileri

- **E-posta:** admin@admin.com
- **Şifre:** 123456

## Proje Yapısı

```
src/
├── api/              # API katmanı (types + endpoints)
├── mocks/            # Mock data (geçici)
├── store/            # Zustand store'ları
├── config/           # Query client + theme config
├── hooks/            # Custom hooks + React Query hooks
├── components/       # Paylaşılan bileşenler
│   ├── layout/       # AppLayout, ProtectedRoute, menü
│   ├── shared/       # FormDrawer, DeleteModal, PageHeader
│   └── form/         # RHF wrapper'lar (Input, Select, MultiSelect...)
├── pages/            # Sayfa bileşenleri
└── router/           # Route tanımları
```

## API Entegrasyonu

Mock data'dan gerçek API'ye geçmek için:

1. `src/api/endpoints/` altındaki dosyalarda mock import'ları kaldır
2. Yorum satırındaki axios çağrılarını aktif et
3. `.env` dosyasındaki `VITE_API_BASE_URL`'i güncelle

## Özellikler

- ✅ Login (JWT token)
- ✅ Dashboard (istatistik kartları)
- ✅ Kullanıcılar CRUD
- ✅ Ürünler CRUD
- ✅ Kategoriler CRUD
- ✅ Drawer ile create/edit
- ✅ Modal ile silme onayı
- ✅ Searchable Select
- ✅ Multiple Searchable Select
- ✅ Light / Dark tema
- ✅ Responsive sidebar
