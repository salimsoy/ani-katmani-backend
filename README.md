# Anı Katmanı 3D — Backend API

3D baskı figürin e-ticaret uygulamasının backend servisi. ASP.NET Core Minimal API, katmanlı mimari ve JWT tabanlı kimlik doğrulama ile geliştirildi.

## Teknolojiler

- **.NET 10** — ASP.NET Core Minimal API
- **Entity Framework Core** — ORM, Code-First migrations
- **SQLite** — veritabanı
- **JWT (JSON Web Token)** — kimlik doğrulama ve yetkilendirme
- **BCrypt** — şifre hash'leme

## Mimari

Katmanlı mimari (Layered Architecture) ile geliştirildi:

- **AniKatmani.API** — HTTP endpoint'leri, request/response yönetimi
- **AniKatmani.Business** — iş mantığı (servisler)
- **AniKatmani.DataAccess** — veritabanı erişimi (DbContext, migrations)
- **AniKatmani.Entities** — veri modelleri (entity'ler)

## Özellikler

- **Kimlik doğrulama:** Kayıt, giriş, JWT token üretimi (7 gün geçerli)
- **Rol tabanlı yetkilendirme:** Admin ve normal kullanıcı ayrımı
- **Ürün yönetimi:** Figürin listeleme, ekleme, düzenleme, silme (admin)
- **Sepet:** Ürün ekleme, adet güncelleme, silme
- **Sipariş:** Sepetten sipariş oluşturma, sipariş geçmişi
- **Sipariş yönetimi:** Admin panelinden sipariş durumu güncelleme

## API Endpoint'leri

### Auth
- `POST /auth/register` — kullanıcı kaydı
- `POST /auth/login` — giriş, JWT döner

### Figürinler
- `GET /figurines` — tüm figürinler
- `GET /figurines/{id}` — tek figürin
- `POST /figurines` — figürin ekle (admin)
- `PUT /figurines/{id}` — figürin güncelle (admin)
- `DELETE /figurines/{id}` — figürin sil (admin)

### Sepet
- `GET /cart` — sepeti getir
- `POST /cart` — sepete ekle
- `PUT /cart/{id}` — adet güncelle
- `DELETE /cart/{id}` — sepetten çıkar

### Siparişler
- `POST /orders` — sipariş oluştur
- `GET /orders` — kullanıcının siparişleri
- `GET /orders/admin` — tüm siparişler (admin)
- `PUT /orders/{id}/status` — sipariş durumu güncelle (admin)

## Kurulum

```bash
# Bağımlılıkları yükle
dotnet restore

# JWT anahtarını user-secrets ile ayarla
cd AniKatmani.API
dotnet user-secrets set "Jwt:Key" "<güçlü-rastgele-anahtar>"

# Veritabanını oluştur
dotnet ef database update --project ../AniKatmani.DataAccess --startup-project .

# Çalıştır
dotnet run
```

## Güvenlik Notları

- Şifreler BCrypt ile hash'lenerek saklanır (salt otomatik)
- JWT anahtarı `user-secrets` içinde tutulur, koda gömülmez
- Admin endpoint'leri rol tabanlı yetkilendirme ile korunur
