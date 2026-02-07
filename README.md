# MenuApp

MenuApp, işletmelerin kendi dijital menülerini yönetebildiği ve müşterilerin mobil öncelikli bir arayüzde menüyü yalnızca görüntüleyebildiği çok kiracılı (multi-tenant) bir platform olarak tasarlanmıştır.

> Bu repo, MVP kapsamını, ürün kararlarını ve teknik tasarımını içeren detaylı bir proje planıdır. Uygulama geliştirmesi sırasında bu doküman “living document” olarak güncellenecektir.

---

## 1) Vizyon ve Amaç

- **İşletmeler için**: Menü yönetimi, tema ayarları, görsel yönetimi, yayınlama ve QR üretimi tek bir panelde.
- **Müşteriler için**: Mobil-first, hızlı ve sadece görüntüleme modunda (read-only) menü deneyimi.
- **Geliştirici için**: Ölçeklenebilir, cache’li, SSR/SSG ile SEO ve performans odaklı mimari.

---

## 2) Ürün Kapsamı (MVP)

### 2.1 Firma Paneli (Admin)

**Kimlik doğrulama**
- Giriş / kayıt (firma hesabı)
- JWT ile auth

**Menü yönetimi**
- Kategori ekle / sırala / gizle
- Ürün ekle / düzenle
  - isim, açıklama, fiyat
  - görsel
  - etiket (ör. vegan, acılı)
  - alerjen bilgisi
- Ürünü kategoriye bağla
- Ürün sıralaması
- Görünürlük ayarı
  - “Stokta yok” / “pasif”

**Tema ayarları**
- Logo
- Renkler
- Font
- Para birimi
- Dil

**Yayınlama**
- Public link oluşturma
- QR üretimi

---

### 2.2 Müşteri Görünümü (Public)

- Read-only
- Mobil-first tasarım
- Kategori filtreleme / arama
- Ürün detay modal’ı
- (Opsiyonel) “Garson çağır” / “Wi-Fi şifresi” widget’ı

---

## 3) Kritik Ürün Kararları (Portfolyo Değeri Yüksek)

### 3.1 Multi-tenant tasarım
- Her firma = **Tenant (Business)**
- Admin tarafı: JWT + tenant scope
- Public tarafı: slug/subdomain ile tenant seçimi

### 3.2 Link formatları
**Seçenek 1 (kolay):**
```
https://menuapp.com/b/{businessSlug}
```

**Seçenek 2 (profesyonel):**
```
https://{businessSlug}.menuapp.com
```

**Menü bazlı gerekiyorsa:**
```
.../m/{menuSlug}
```

### 3.3 Public link güvenliği
- Varsayılan: `businessSlug` ile erişim
- Opsiyonel: slug + **publicToken** (tahmin edilemez link)
```
/b/cafe-x?t=8f3k2a
```

### 3.4 Görsel yönetimi (kritik)
- Upload: S3/MinIO presigned URL veya MVP için local storage
- Thumbnail üretimi (performans için)
- CDN & caching (v2)

### 3.5 Performans & SEO
- Public sayfa: SSR/SSG (Next.js)
- Redis ile “public menu snapshot” cache

---

## 4) Teknoloji Yığını (Önerilen)

### Backend
- **.NET 8 Web API**
- Clean/Onion Architecture + CQRS (MediatR)
- PostgreSQL
- Redis (cache + rate limit opsiyon)
- Serilog + correlationId + global exception handling
- FluentValidation
- Background jobs: Hangfire / Quartz

### Frontend
- **Admin Panel**: Next.js App Router
- **Public View**: Next.js (SSR/SEO)

---

## 5) Repo Yapısı

```
backend/MenuApp.Api   # Minimal .NET 8 API (in-memory store ile MVP)
admin-panel           # Next.js admin paneli
public-view            # Next.js public view
```

---

## 6) Veri Modeli (MVP Önerisi)

### Tenant (Business)
- Id
- Name
- Slug
- PublicToken (opsiyonel)
- ThemeSettings
- CreatedAt

### Menu
- Id
- BusinessId
- Name
- IsPublished
- CreatedAt

### Category
- Id
- MenuId
- Name
- SortOrder
- IsHidden

### Product
- Id
- MenuId
- CategoryId
- Name
- Description
- Price
- Currency
- ImageUrl
- Tags (vegan, acılı...)
- Allergens
- SortOrder
- Visibility (stokta yok, pasif)

### ThemeSettings
- LogoUrl
- PrimaryColor
- SecondaryColor
- Font
- Currency
- Language

---

## 7) API Taslağı (Örnek)

### Auth
- `POST /api/auth/register`
- `POST /api/auth/login`

### Menu & Category
- `GET /api/admin/menus`
- `POST /api/admin/menus`
- `GET /api/admin/menus/{menuId}`
- `POST /api/admin/categories`
- `PATCH /api/admin/categories/{id}`

### Product
- `POST /api/admin/products`
- `PATCH /api/admin/products/{id}`
- `POST /api/admin/products/{id}/visibility`

### Publish
- `POST /api/admin/menus/{menuId}/publish`

### Public
- `GET /api/public/menus/{businessSlug}`
- `GET /api/public/menus/{businessSlug}?t=publicToken`

---

## 8) Admin UX Akışı (Özet)

1. Firma kayıt olur / giriş yapar
2. Menü oluşturur
3. Kategorileri ekler ve sıralar
4. Ürünleri ekler, kategoriye bağlar
5. Tema ayarlarını yapar
6. Yayınlar → public link + QR alır

---

## 9) Public UX Akışı (Özet)

1. QR veya link ile menüye gider
2. Kategoriler arasında gezer / arama yapar
3. Ürün detayına tıklar → modal
4. (Opsiyon) garson çağır / wi-fi şifresi widget

---

## 10) MVP Sonrası (Backlog)

- Çoklu menü yönetimi (kahvaltı, öğle vs.)
- Çoklu dil desteği
- Kampanya / indirim yönetimi
- Analytics (en çok görüntülenen ürünler)
- QR dinamik güncelleme
- Push bildirim / mesajlaşma

---

## 11) Geliştirme Adımları (Roadmap)

1. **Backend iskeleti** (auth + CRUD)
2. **Admin panel MVP** (menü / kategori / ürün yönetimi)
3. **Public view MVP** (SSR + read-only)
4. **Theme + publish + QR**
5. **Görsel upload + thumbnail**
6. **Cache & performans iyileştirmeleri**

---

## 12) Lokal Çalıştırma

### Backend (.NET API)
```bash
cd backend/MenuApp.Api
# dotnet restore
# dotnet run
```

### Admin Panel (Next.js)
```bash
cd admin-panel
# npm install
# npm run dev
```

### Public View (Next.js)
```bash
cd public-view
# npm install
# npm run dev
```

---

## 13) Katkı ve Lisans

- Katkılar için PR süreci daha sonra eklenecek.
- Lisans bilgisi: TBD

---

## 14) Tasarım Notları

- Admin panel sade, grid tabanlı ve tablo odaklı olacak.
- Public menü yüksek kontrast, büyük tipografi ve görsel odaklı olacak.
- Tema ayarları ile marka kimliği yansıtılacak.

---

## 15) Ek: QR Üretimi

- QR link: `https://menuapp.com/b/{businessSlug}`
- QR üretimi için backend job
- Görselin yeniden boyutlandırılması (thumbnail) ve cachelenmesi

---

## 16) Proje Sahibi

Bu proje, portfolyo ve ürün odaklı bir showcase olarak tasarlanmıştır.
