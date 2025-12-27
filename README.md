# BLRB2B - B2B E-Ticaret Sistemi

Modern, kurumsal B2B e-ticaret platformu. Bayiler için ürün kataloğu, sipariş yönetimi ve stok takibi çözümü.

## 🏗️ Teknik Mimari

| Bileşen | Teknoloji |
|---------|-----------|
| Frontend | Blazor Web App (Server Mode) |
| Backend | .NET 9 |
| Database | SQL Server |
| API | REST |
| Hosting | IIS / Docker |

## 👥 Kullanıcı Yapısı

- **Firma:** Tek firma
- **Kullanıcı Sayısı:** ~100
- **Roller:** Admin, User
- **Diller:** Türkçe (TR), İngilizce (EN)

## 📦 Modüller

### 1. Ürün & Sipariş Yönetimi
- Ürün kataloğu (Kategori, barkod, fiyat, resim, varyasyon)
- Stoktan Netsim senkronizasyonu
- Çoklu fiyatlandırma (Liste, bayi, özel fiyat)
- Sepet ve sipariş oluşturma
- Tek tıkla tekrar sipariş
- Toplu sipariş
- Sipariş onay mekanizması (Admin onayı)
- Sipariş durumları (Bekliyor, Onaylandı, Hazırlanıyor, Teslim Edildi, İptal)
- Minimum sipariş tutarı/adesi kısıtlaması
- Stok limiti kontrolü

### 2. Müşteri (Cari) Yönetimi
- Müşteri kaydı (Admin ekler, müşteri kayıt olur)
- Firma bilgileri (Unvan, vergi no, vergi dairesi, adres, iletişim)
- Müşteri onay süreci (Admin onayı)
- Müşteri grupları/kategorileri (Altın bayi, gümüş bayi, VIP)
- Gruba özel fiyat listesi
- Müşteri bazlı kredi limiti/bakiye takibi
- B2B → Netsim senkronizasyonu

### 3. Stok Yönetimi
- Gerçek zamanlı stok senkronizasyonu
- Rezervasyon mekanizması (Sepetteki ürün stoğu düşürür)
- Stok bitince ürün gizleme / "Tükendi" gösterme
- Kritik stok seviyesi uyarısı (Admin email)
- Çoklu depo yönetimi
- Stok raporları (Durum, Hareketli ürünler, Satış)

### 4. Ödeme Yönetimi
- Sanal POS (Kredi kartı online)
- Havale/EFT
- Vadeli satış (30, 60, 90 gün)
- Çek
- Ödeme durumları (Bekliyor, Ödendi, Kısmi Ödeme, İade)
- Email bildirimleri

### 5. Raporlama
- Satış raporu (PDF) - Günlük/Haftalık/Aylık, En çok satan, En harcayan
- Müşteri raporu (PDF) - Analiz, Bakiye, Ödemeler
- Filtreleme (Tarih, Müşteri, Ürün, Durum)
- Export: PDF

### 6. Admin Paneli
- Ürün yönetimi
- Stok yönetimi
- Sipariş yönetimi
- Müşteri yönetimi
- Raporlar
- Sistem ayarları

## 🔗 Entegrasyonlar

| Entegrasyon | Açıklama |
|-------------|----------|
| **Netsim** | ERP entegrasyonu (Cari ve Stok sync) |
| **Sanal POS** | Online ödeme |
| **Email** | SMTP / SendGrid |

## 🎨 Tasarım

- **Renk:** Mavi tema
- **Dil:** TR / EN (Çoklu dil desteği)
- **Responsive:** Mobil uyumlu

## 🚀 Kurulum

```bash
# Repo klonla
git clone https://github.com/AdosoftYazilim/BLRB2B.git
cd BLRB2B

# Restore
dotnet restore

# Database migration
dotnet ef database update

# Çalıştır
dotnet run
```

## 📋 Gereksinimler

- .NET 9 SDK
- SQL Server 2022+
- Visual Studio 2022 / VS Code

## 📝 Lisans

Copyright © 2025 AdosoftYazilim
