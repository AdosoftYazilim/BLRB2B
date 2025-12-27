# BLRB2B - Teknik Mimari Dokümanı

## 📐 Sistem Mimarisi

```
┌─────────────────────────────────────────────────────────────┐
│                         BLRB2B System                       │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌──────────────────────────────────────────────────────┐  │
│  │           Presentation Layer (Blazor Server)         │  │
│  │  ┌──────────┐  ┌──────────┐  ┌──────────────────┐  │  │
│  │  │  Pages   │  │ Components │  │   Services      │  │  │
│  │  └──────────┘  └──────────┘  └──────────────────┘  │  │
│  └──────────────────────────────────────────────────────┘  │
│                           ▼                                  │
│  ┌──────────────────────────────────────────────────────┐  │
│  │            Business Logic Layer                      │  │
│  │  ┌──────────┐  ┌──────────┐  ┌──────────────────┐  │  │
│  │  │ Services │  │ Validators│  │  Interfaces     │  │  │
│  │  └──────────┘  └──────────┘  └──────────────────┘  │  │
│  └──────────────────────────────────────────────────────┘  │
│                           ▼                                  │
│  ┌──────────────────────────────────────────────────────┐  │
│  │            Data Access Layer (EF Core)               │  │
│  │  ┌──────────┐  ┌──────────┐  ┌──────────────────┐  │  │
│  │  │ Repositories │ │ DbContext │  │   Entities      │  │  │
│  │  └──────────┘  └──────────┘  └──────────────────┘  │  │
│  └──────────────────────────────────────────────────────┘  │
│                           ▼                                  │
│  ┌──────────────────────────────────────────────────────┐  │
│  │              SQL Server Database                     │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                               │
└─────────────────────────────────────────────────────────────┘
```

## 🏗️ Çok Katmanlı Mimari (Clean Architecture)

### 1. Presentation Layer (Blazor Web App - Server)

**Sorumluluklar:**
- UI bileşenleri ve sayfaları
- Kullanıcı etkileşimi
- Routing ve navigasyon
- State management (Blazor server-side)

**Teknolojiler:**
- Blazor Web App (Server Mode)
- MudBlazor / Radzen (UI Component Library)
- FluentValidation (Input validation)

### 2. Business Logic Layer (Application Core)

**Sorumluluklar:**
- İş kuralları ve validasyonlar
- Service implementasyonları
- DTO'lar ve mapping
- Business logic işlemleri

**Teknolojiler:**
- .NET 9 Class Library
- AutoFixture (Test)
- FluentValidation

### 3. Data Access Layer (Infrastructure)

**Sorumluluklar:**
- Veritabanı işlemleri
- Entity Framework Core DbContext
- Repository pattern implementation
- Migration yönetimi

**Teknolojiler:**
- Entity Framework Core 9
- SQL Server Provider
- Dapper (Performans kritik sorgular için)

### 4. Domain Layer (Core)

**Sorumluluklar:**
- Domain entities
- Value objects
- Interfaces
- Domain events

**Teknolojiler:**
- .NET 9 Class Library

## 📁 Proje Yapısı

```
BLRB2B.sln
│
├── src/
│   ├── BLRB2B.Web/                    # Blazor Web App (Server)
│   │   ├── Components/
│   │   │   ├── Layout/
│   │   │   └── Pages/
│   │   ├── Data/
│   │   ├── wwwroot/
│   │   └── Program.cs
│   │
│   ├── BLRB2B.Application/            # Business Logic Layer
│   │   ├── Services/
│   │   ├── DTOs/
│   │   ├── Validators/
│   │   └── Interfaces/
│   │
│   ├── BLRB2B.Domain/                 # Domain Layer
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   ├── Interfaces/
│   │   └── Events/
│   │
│   └── BLRB2B.Infrastructure/         # Data Access Layer
│       ├── Data/
│       │   ├── ApplicationDbContext.cs
│       │   └── Repositories/
│       ├── Migrations/
│       └── Services/
│           └── External/
│               ├── NetsimService.cs
│               ├── PaymentService.cs
│               └── EmailService.cs
│
├── tests/
│   ├── BLRB2B.UnitTests/
│   ├── BLRB2B.IntegrationTests/
│   └── BLRB2B.UI.Tests/
│
└── docs/
    ├── ARCHITECTURE.md
    ├── DATABASE.md
    ├── API.md
    └── CODING_STANDARDS.md
```

## 🔐 Güvenlik Mimarisi

### Authentication & Authorization

```
┌─────────────────────────────────────────────┐
│              Authentication Flow             │
├─────────────────────────────────────────────┤
│                                             │
│  1. User Login → JWT Token                  │
│  2. Token Stored → Session/LocalStorage     │
│  3. Each Request → Token Validated          │
│  4. Role Check → Authorization              │
│                                             │
│  Roles:                                     │
│  - Admin (Full access)                      │
│  - User (Limited access)                    │
│                                             │
└─────────────────────────────────────────────┘
```

**Teknolojiler:**
- ASP.NET Core Identity
- JWT Bearer Tokens
- Role-based Authorization
- Policy-based Authorization

### Güvenlik Önlemleri

| Önlem | Açıklama |
|-------|----------|
| HTTPS | Zorunlu SSL/TLS |
| CSRF Protection | Anti-forgery tokens |
| XSS Protection | Input sanitization, encoding |
| SQL Injection | Parameterized queries (EF Core) |
| Password Hashing | ASP.NET Core Identity (PBKDF2) |
| Rate Limiting | API endpoint'leri için |
| Audit Logging | Tüm kritik işlemler |

## 🌐 Entegrasyon Mimarisi

### Netsim ERP Entegrasyonu

```csharp
// Netsim Service Interface
public interface INetsimService
{
    Task SyncCustomersAsync();
    Task SyncProductsAsync();
    Task SyncStockAsync();
    Task<OrderResult> SendOrderAsync(OrderDto order);
}

// Sync Strategy
public enum SyncDirection
{
    Bidirectional,    // Çift yönlü
    ToNetsim,         // B2B → Netsim
    FromNetsim        // Netsim → B2B
}

// Sync Frequency
public enum SyncFrequency
{
    Realtime,         // Anlık
    Scheduled,        // Planlı (cron)
    Manual            // Manuel tetik
}
```

### Ödeme Entegrasyonu

```csharp
// Payment Service Interface
public interface IPaymentService
{
    Task<PaymentResult> ProcessCreditCardPaymentAsync(PaymentRequest request);
    Task<PaymentResult> ProcessBankTransferAsync(PaymentRequest request);
    Task<PaymentResult> ProcessCheckPaymentAsync(PaymentRequest request);
    Task RefundPaymentAsync(string transactionId, decimal amount);
}
```

## 📡 Communication Pattern

### Service Communication

```csharp
// Repository Pattern
public interface IRepository<T> where T : BaseEntity
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

// Service Pattern
public interface IProductService
{
    Task<ProductDto> GetProductByIdAsync(int id);
    Task<IEnumerable<ProductDto>> GetProductsAsync();
    Task CreateProductAsync(CreateProductDto dto);
    Task UpdateProductAsync(int id, UpdateProductDto dto);
}
```

## 🔄 Caching Strategy

```
┌────────────────────────────────────────┐
│         Caching Layer                  │
├────────────────────────────────────────┤
│                                        │
│  ┌──────────────────────────────────┐ │
│  │  In-Memory Cache (IMemoryCache)  │ │
│  │  - Products                      │ │
│  │  - Categories                    │ │
│  │  - User Sessions                 │ │
│  └──────────────────────────────────┘ │
│                                        │
│  ┌──────────────────────────────────┐ │
│  │  Redis (Optional - Future)       │ │
│  │  - Distributed cache             │ │
│  │  - Session store                 │ │
│  └──────────────────────────────────┘ │
│                                        │
└────────────────────────────────────────┘
```

### Cache Policies

| Veri Tipi | Cache Süresi | Invalidasyon |
|-----------|--------------|--------------|
| Products | 1 saat | Ürün güncelleme |
| Categories | 24 saat | Admin güncelleme |
| User Info | 30 dakika | Logout |
| Stock | 5 dakika | Real-time sync |

## 📊 Logging & Monitoring

```
┌────────────────────────────────────────┐
│         Logging Architecture           │
├────────────────────────────────────────┤
│                                        │
│  Application → Serilog → Seq / Elastic │
│                ↓                       │
│  File (Logs/app-{Date}.log)           │
│                                        │
│  Log Levels:                           │
│  - Debug (Development)                 │
│  - Information (General)               │
│  - Warning (Business logic)            │
│  - Error (Exceptions)                  │
│  - Fatal (System failure)              │
│                                        │
└────────────────────────────────────────┘
```

### Log Yapısı

```csharp
// Structured Logging
Log.Information("Order created: {OrderId}, Customer: {CustomerId}, Amount: {Amount}",
    order.Id, order.CustomerId, order.TotalAmount);

// Error Logging
Log.Error(ex, "Failed to process payment for order {OrderId}", order.Id);
```

## 🚀 Deployment Mimarisi

### Development

```
Developer Machine → Local SQL Server → IIS Express / dotnet run
```

### Production

```
┌─────────────────────────────────────────┐
│           Production Environment        │
├─────────────────────────────────────────┤
│                                         │
│  ┌───────────────────────────────────┐ │
│  │  Docker Container                │ │
│  │  - BLRB2B.Web App                │ │
│  │  - SQL Server (separate container)│ │
│  └───────────────────────────────────┘ │
│           ↓                             │
│  ┌───────────────────────────────────┐ │
│  │  Reverse Proxy (Nginx / IIS)     │ │
│  └───────────────────────────────────┘ │
│           ↓                             │
│  ┌───────────────────────────────────┐ │
│  │  SSL Certificate (HTTPS)         │ │
│  └───────────────────────────────────┘ │
│                                         │
└─────────────────────────────────────────┘
```

### CI/CD Pipeline

```
┌──────────┐    ┌──────────┐    ┌──────────┐    ┌──────────┐
│   Push   │ → │   Build   │ → │   Test   │ → │ Deploy   │
│ to GitHub│    │   (CI)   │    │  (Unit)  │    │  (CD)    │
└──────────┘    └──────────┘    └──────────┘    └──────────┘
```

## 🧪 Testing Strategy

```
┌─────────────────────────────────────────┐
│         Testing Pyramid                 │
├─────────────────────────────────────────┤
│                                         │
│            ▲                             │
│           ╱ ╲                            │
│          ╱   ╲     E2E Tests (10%)      │
│         ╱─────╲    (Selenium/Playwright) │
│        ╱       ╲                         │
│       ╱─────────╲   Integration (20%)   │
│      ╱───────────╲  (API/DB)            │
│     ╱─────────────╲                      │
│    ╱───────────────╲ Unit Tests (70%)   │
│   ╱─────────────────╲(xUnit/NUnit)      │
│                                         │
└─────────────────────────────────────────┘
```

## 📱 Responsive Design

```
┌─────────────────────────────────────────┐
│         Breakpoints                     │
├─────────────────────────────────────────┤
│                                         │
│  Mobile:   < 768px  (Stack layout)     │
│  Tablet:   768px - 1024px              │
│  Desktop:  > 1024px (Grid layout)      │
│                                         │
└─────────────────────────────────────────┘
```

## 🔧 Configuration Management

```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=BLRB2B;Trusted_Connection=True;"
  },
  "JwtSettings": {
    "Secret": "YOUR_SECRET_KEY",
    "ExpiryMinutes": 60
  },
  "NetsimSettings": {
    "ApiKey": "YOUR_API_KEY",
    "BaseUrl": "https://api.netsim.com.tr",
    "SyncIntervalMinutes": 5
  },
  "PaymentSettings": {
    "Provider": "Iyzico",
    "ApiKey": "YOUR_API_KEY",
    "SecretKey": "YOUR_SECRET_KEY"
  },
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "Username": "your@email.com",
    "Password": "YOUR_PASSWORD"
  },
  "CacheSettings": {
    "ProductCacheDurationMinutes": 60,
    "StockCacheDurationMinutes": 5
  }
}
```

## 🎨 Naming Conventions

### C# Code

```csharp
// Classes: PascalCase
public class ProductService { }

// Interfaces: PascalCase with 'I' prefix
public interface IProductService { }

// Methods: PascalCase
public async Task<ProductDto> GetProductByIdAsync(int id) { }

// Properties: PascalCase
public string ProductName { get; set; }

// Local variables: camelCase
var productName = "Test";

// Constants: PascalCase
public const int MaxCartItems = 100;

// Private fields: _camelCase
private readonly ILogger _logger;
```

### Database

```sql
-- Tables: PascalCase, plural
CREATE TABLE Products (
    ProductId INT PRIMARY KEY,
    ProductName NVARCHAR(100)
);

-- Columns: PascalCase
CREATE TABLE Orders (
    OrderId INT PRIMARY KEY,
    OrderDate DATETIME,
    TotalAmount DECIMAL(18,2)
);

-- Foreign Keys: TableNameId
CustomerId, ProductId, OrderId
```

### API Endpoints

```
GET    /api/products          - List all
GET    /api/products/{id}     - Get by id
POST   /api/products          - Create
PUT    /api/products/{id}     - Update
DELETE /api/products/{id}     - Delete
```
