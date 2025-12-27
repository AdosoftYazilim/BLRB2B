# BLRB2B - Veritabanı Tasarım Dokümanı

## 📊 Veritabanı Şeması

```
┌─────────────────────────────────────────────────────────────────────┐
│                        BLRB2B Database                              │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐           │
│  │ Users    │  │Products  │  │ Customers│  │  Orders  │           │
│  └──────────┘  └──────────┘  └──────────┘  └──────────┘           │
│       │              │              │              │               │
│       ▼              ▼              ▼              ▼               │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐           │
│  │ Roles    │  │Categories│  │Warehouses│  │OrderItems│           │
│  └──────────┘  └──────────┘  └──────────┘  └──────────┘           │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

## 📋 Tablo Yapıları

### 1. Users (Kullanıcılar)

```sql
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(256) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(512) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    PhoneNumber NVARCHAR(20),
    IsActive BIT NOT NULL DEFAULT 1,
    EmailConfirmed BIT NOT NULL DEFAULT 0,
    LastLoginDate DATETIME2,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT CK_User_Email CHECK (Email LIKE '%@%')
);

GO

CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Users_IsActive ON Users(IsActive);
```

### 2. Roles (Roller)

```sql
CREATE TABLE Roles (
    RoleId INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(255),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

GO

-- Default Roles
INSERT INTO Roles (RoleName, Description) VALUES
('Admin', 'Full system access'),
('User', 'Limited customer access');
```

### 3. UserRoles (Kullanıcı-Rol İlişkisi)

```sql
CREATE TABLE UserRoles (
    UserRoleId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    RoleId INT NOT NULL,
    AssignedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_UserRoles_Users FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    CONSTRAINT FK_UserRoles_Roles FOREIGN KEY (RoleId) REFERENCES Roles(RoleId) ON DELETE CASCADE,
    CONSTRAINT UQ_UserRoles_User_Role UNIQUE (UserId, RoleId)
);

GO

CREATE INDEX IX_UserRoles_UserId ON UserRoles(UserId);
CREATE INDEX IX_UserRoles_RoleId ON UserRoles(RoleId);
```

### 4. Customers (Müşteriler / Cariler)

```sql
CREATE TABLE Customers (
    CustomerId INT IDENTITY(1,1) PRIMARY KEY,
    CustomerCode NVARCHAR(50) NOT NULL UNIQUE, -- Netsim cari kodu
    CompanyName NVARCHAR(255) NOT NULL,
    TaxNumber NVARCHAR(20),
    TaxOffice NVARCHAR(100),
    Address NVARCHAR(500),
    City NVARCHAR(100),
    Country NVARCHAR(100) NOT NULL DEFAULT 'Turkey',
    Phone NVARCHAR(20),
    Email NVARCHAR(256),
    WebSite NVARCHAR(255),
    CustomerGroupId INT NULL,
    CreditLimit DECIMAL(18,2) NOT NULL DEFAULT 0,
    CurrentBalance DECIMAL(18,2) NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    IsApproved BIT NOT NULL DEFAULT 0,
    ApprovedAt DATETIME2 NULL,
    ApprovedBy INT NULL,
    NetsimSynced BIT NOT NULL DEFAULT 0,
    NetsimSyncDate DATETIME2 NULL,
    Notes NVARCHAR(MAX),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Customers_Groups FOREIGN KEY (CustomerGroupId) REFERENCES CustomerGroups(CustomerGroupId),
    CONSTRAINT CK_Customers_TaxNumber CHECK (TaxNumber LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]')
);

GO

CREATE INDEX IX_Customers_CustomerCode ON Customers(CustomerCode);
CREATE INDEX IX_Customers_CustomerGroupId ON Customers(CustomerGroupId);
CREATE INDEX IX_Customers_IsActive ON Customers(IsActive);
CREATE INDEX IX_Customers_NetsimSynced ON Customers(NetsimSynced);
```

### 5. CustomerGroups (Müşteri Grupları)

```sql
CREATE TABLE CustomerGroups (
    CustomerGroupId INT IDENTITY(1,1) PRIMARY KEY,
    GroupName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255),
    DiscountPercent DECIMAL(5,2) NOT NULL DEFAULT 0,
    Priority INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

GO

-- Default Customer Groups
INSERT INTO CustomerGroups (GroupName, Description, DiscountPercent, Priority) VALUES
('VIP', 'VIP Customers', 15.00, 1),
('Gold', 'Gold Dealers', 10.00, 2),
('Silver', 'Silver Dealers', 5.00, 3),
('Standard', 'Standard Customers', 0.00, 4);
```

### 6. Categories (Kategoriler)

```sql
CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(150) NOT NULL,
    CategoryName_EN NVARCHAR(150),
    Description NVARCHAR(500),
    Description_EN NVARCHAR(500),
    ParentCategoryId INT NULL,
    ImageUrl NVARCHAR(500),
    DisplayOrder INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Categories_Parent FOREIGN KEY (ParentCategoryId) REFERENCES Categories(CategoryGroupId),
    CONSTRAINT CK_Categories_SelfParent CHECK (CategoryId <> ParentCategoryId OR ParentCategoryId IS NULL)
);

GO

CREATE INDEX IX_Categories_ParentCategoryId ON Categories(ParentCategoryId);
CREATE INDEX IX_Categories_IsActive ON Categories(IsActive);
```

### 7. Products (Ürünler)

```sql
CREATE TABLE Products (
    ProductId INT IDENTITY(1,1) PRIMARY KEY,
    ProductCode NVARCHAR(50) NOT NULL UNIQUE, -- Barkod / Ürün kodu
    ProductName NVARCHAR(255) NOT NULL,
    ProductName_EN NVARCHAR(255),
    Description NVARCHAR(MAX),
    Description_EN NVARCHAR(MAX),
    CategoryId INT NOT NULL,
    Brand NVARCHAR(100),
    Unit NVARCHAR(20) NOT NULL DEFAULT 'Adet',
    Barcode NVARCHAR(50),
    ListPrice DECIMAL(18,2) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    IsFeatured BIT NOT NULL DEFAULT 0,
    MinOrderQuantity INT NOT NULL DEFAULT 1,
    MaxOrderQuantity INT NULL,
    Weight DECIMAL(10,3),
    Dimensions NVARCHAR(50),
    NetsimSynced BIT NOT NULL DEFAULT 0,
    NetsimSyncDate DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId),
    CONSTRAINT CK_Products_ListPrice CHECK (ListPrice >= 0)
);

GO

CREATE INDEX IX_Products_ProductCode ON Products(ProductCode);
CREATE INDEX IX_Products_CategoryId ON Products(CategoryId);
CREATE INDEX IX_Products_IsActive ON Products(IsActive);
CREATE INDEX IX_Products_Barcode ON Products(Barcode);
CREATE INDEX IX_Products_NetsimSynced ON Products(NetsimSynced);
```

### 8. ProductImages (Ürün Resimleri)

```sql
CREATE TABLE ProductImages (
    ProductImageId INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL,
    ImageUrl NVARCHAR(500) NOT NULL,
    AltText NVARCHAR(255),
    DisplayOrder INT NOT NULL DEFAULT 0,
    IsMainImage BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_ProductImages_Products FOREIGN KEY (ProductId) REFERENCES Products(ProductId) ON DELETE CASCADE
);

GO

CREATE INDEX IX_ProductImages_ProductId ON ProductImages(ProductId);
```

### 9. ProductPrices (Ürün Fiyatları)

```sql
CREATE TABLE ProductPrices (
    ProductPriceId INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL,
    CustomerGroupId INT NULL, -- NULL = Liste fiyatı
    PriceCode NVARCHAR(50) NOT NULL, -- Liste, Bayi, Özel vb.
    Price DECIMAL(18,2) NOT NULL,
    Currency NVARCHAR(3) NOT NULL DEFAULT 'TRY',
    ValidFrom DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ValidUntil DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_ProductPrices_Products FOREIGN KEY (ProductId) REFERENCES Products(ProductId) ON DELETE CASCADE,
    CONSTRAINT FK_ProductPrices_Groups FOREIGN KEY (CustomerGroupId) REFERENCES CustomerGroups(CustomerGroupId),
    CONSTRAINT CK_ProductPrices_Price CHECK (Price >= 0),
    CONSTRAINT CK_ProductPrices_ValidDate CHECK (ValidUntil IS NULL OR ValidUntil >= ValidFrom)
);

GO

CREATE INDEX IX_ProductPrices_ProductId ON ProductPrices(ProductId);
CREATE INDEX IX_ProductPrices_CustomerGroupId ON ProductPrices(CustomerGroupId);
```

### 10. Warehouses (Depolar)

```sql
CREATE TABLE Warehouses (
    WarehouseId INT IDENTITY(1,1) PRIMARY KEY,
    WarehouseCode NVARCHAR(20) NOT NULL UNIQUE,
    WarehouseName NVARCHAR(150) NOT NULL,
    Address NVARCHAR(500),
    City NVARCHAR(100),
    Phone NVARCHAR(20),
    ManagerName NVARCHAR(100),
    IsActive BIT NOT NULL DEFAULT 1,
    IsDefault BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

GO

CREATE UNIQUE INDEX UX_Warehouses_IsDefault ON Warehouses(IsDefault) WHERE IsDefault = 1;
```

### 11. Stock (Stok)

```sql
CREATE TABLE Stock (
    StockId INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL,
    WarehouseId INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 0,
    ReservedQuantity INT NOT NULL DEFAULT 0,
    AvailableQuantity AS (Quantity - ReservedQuantity) PERSISTED,
    LastStockUpdate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    NetsimSynced BIT NOT NULL DEFAULT 0,
    NetsimSyncDate DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Stock_Products FOREIGN KEY (ProductId) REFERENCES Products(ProductId),
    CONSTRAINT FK_Stock_Warehouses FOREIGN KEY (WarehouseId) REFERENCES Warehouses(WarehouseId),
    CONSTRAINT CK_Stock_Quantity CHECK (Quantity >= 0),
    CONSTRAINT CK_Stock_Reserved CHECK (ReservedQuantity >= 0),
    CONSTRAINT UQ_Stock_Product_Warehouse UNIQUE (ProductId, WarehouseId)
);

GO

CREATE INDEX IX_Stock_ProductId ON Stock(ProductId);
CREATE INDEX IX_Stock_WarehouseId ON Stock(WarehouseId);
CREATE INDEX IX_Stock_AvailableQuantity ON Stock(AvailableQuantity);
```

### 12. StockMovements (Stok Hareketleri)

```sql
CREATE TABLE StockMovements (
    StockMovementId INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL,
    WarehouseId INT NOT NULL,
    MovementType NVARCHAR(20) NOT NULL, -- In, Out, Transfer, Adjustment
    Quantity INT NOT NULL,
    ReferenceType NVARCHAR(50), -- Order, Return, Manual
    ReferenceId INT,
    Notes NVARCHAR(500),
    CreatedBy INT,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_StockMovements_Products FOREIGN KEY (ProductId) REFERENCES Products(ProductId),
    CONSTRAINT FK_StockMovements_Warehouses FOREIGN KEY (WarehouseId) REFERENCES Warehouses(WarehouseId),
    CONSTRAINT CK_StockMovements_Type CHECK (MovementType IN ('In', 'Out', 'Transfer', 'Adjustment'))
);

GO

CREATE INDEX IX_StockMovements_ProductId ON StockMovements(ProductId);
CREATE INDEX IX_StockMovements_CreatedAt ON StockMovements(CreatedAt);
```

### 13. Orders (Siparişler)

```sql
CREATE TABLE Orders (
    OrderId INT IDENTITY(1,1) PRIMARY KEY,
    OrderNumber NVARCHAR(50) NOT NULL UNIQUE,
    CustomerId INT NOT NULL,
    UserId INT NOT NULL, -- Siparişi oluşturan kullanıcı
    OrderDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    RequiredDate DATETIME2,
    TotalAmount DECIMAL(18,2) NOT NULL,
    DiscountAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    TaxAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    ShippingAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    GrandTotal DECIMAL(18,2) NOT NULL,
    OrderStatus NVARCHAR(20) NOT NULL DEFAULT 'Pending', -- Pending, Approved, Processing, Completed, Cancelled
    PaymentStatus NVARCHAR(20) NOT NULL DEFAULT 'Unpaid', -- Unpaid, Partial, Paid, Refunded
    PaymentMethod NVARCHAR(50), -- CreditCard, BankTransfer, Check
    PaymentDate DATETIME2 NULL,
    Notes NVARCHAR(MAX),
    InternalNotes NVARCHAR(MAX),
    NetsimSynced BIT NOT NULL DEFAULT 0,
    NetsimSyncDate DATETIME2 NULL,
    NetsimOrderRef NVARCHAR(50),
    ApprovedBy INT NULL,
    ApprovedAt DATETIME2 NULL,
    CompletedAt DATETIME2 NULL,
    CancelledAt DATETIME2 NULL,
    CancelReason NVARCHAR(500),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Orders_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId),
    CONSTRAINT FK_Orders_Users FOREIGN KEY (UserId) REFERENCES Users(UserId),
    CONSTRAINT CK_Orders_Status CHECK (OrderStatus IN ('Pending', 'Approved', 'Processing', 'Completed', 'Cancelled')),
    CONSTRAINT CK_Orders_PaymentStatus CHECK (PaymentStatus IN ('Unpaid', 'Partial', 'Paid', 'Refunded')),
    CONSTRAINT CK_Orders_Total CHECK (TotalAmount >= 0)
);

GO

CREATE INDEX IX_Orders_CustomerId ON Orders(CustomerId);
CREATE INDEX IX_Orders_UserId ON Orders(UserId);
CREATE INDEX IX_Orders_OrderDate ON Orders(OrderDate);
CREATE INDEX IX_Orders_OrderStatus ON Orders(OrderStatus);
CREATE INDEX IX_Orders_PaymentStatus ON Orders(PaymentStatus);
CREATE INDEX IX_Orders_NetsimSynced ON Orders(NetsimSynced);
```

### 14. OrderItems (Sipariş Kalemleri)

```sql
CREATE TABLE OrderItems (
    OrderItemId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    ProductCode NVARCHAR(50) NOT NULL,
    ProductName NVARCHAR(255) NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    DiscountPercent DECIMAL(5,2) NOT NULL DEFAULT 0,
    DiscountAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    TaxPercent DECIMAL(5,2) NOT NULL DEFAULT 20,
    TaxAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    LineTotal DECIMAL(18,2) NOT NULL,
    Notes NVARCHAR(500),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES Orders(OrderId) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItems_Products FOREIGN KEY (ProductId) REFERENCES Products(ProductId),
    CONSTRAINT CK_OrderItems_Quantity CHECK (Quantity > 0),
    CONSTRAINT CK_OrderItems_UnitPrice CHECK (UnitPrice >= 0)
);

GO

CREATE INDEX IX_OrderItems_OrderId ON OrderItems(OrderId);
CREATE INDEX IX_OrderItems_ProductId ON OrderItems(ProductId);
```

### 15. OrderStatusHistory (Sipariş Durum Geçmişi)

```sql
CREATE TABLE OrderStatusHistory (
    HistoryId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    OldStatus NVARCHAR(20),
    NewStatus NVARCHAR(20) NOT NULL,
    ChangedBy INT,
    Notes NVARCHAR(500),
    ChangedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_OrderStatusHistory_Orders FOREIGN KEY (OrderId) REFERENCES Orders(OrderId) ON DELETE CASCADE
);

GO

CREATE INDEX IX_OrderStatusHistory_OrderId ON OrderStatusHistory(OrderId);
CREATE INDEX IX_OrderStatusHistory_ChangedAt ON OrderStatusHistory(ChangedAt);
```

### 16. Payments (Ödemeler)

```sql
CREATE TABLE Payments (
    PaymentId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    PaymentDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    PaymentMethod NVARCHAR(50) NOT NULL, -- CreditCard, BankTransfer, Check
    Amount DECIMAL(18,2) NOT NULL,
    Currency NVARCHAR(3) NOT NULL DEFAULT 'TRY',
    TransactionId NVARCHAR(255),
    AuthorizationCode NVARCHAR(255),
    BankName NVARCHAR(100),
    CheckNumber NVARCHAR(50),
    CheckDate DATETIME2,
    DueDate DATETIME2,
    Notes NVARCHAR(500),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Payments_Orders FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),
    CONSTRAINT CK_Payments_Amount CHECK (Amount > 0)
);

GO

CREATE INDEX IX_Payments_OrderId ON Payments(OrderId);
CREATE INDEX IX_Payments_PaymentDate ON Payments(PaymentDate);
```

### 17. Carts (Sepetler - Geçici Veri)

```sql
CREATE TABLE Carts (
    CartId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ExpiresAt DATETIME2 NOT NULL DEFAULT DATEADD(DAY, 7, GETUTCDATE()),
    CONSTRAINT FK_Carts_Users FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

GO

CREATE INDEX IX_Carts_UserId ON Carts(UserId);
```

### 18. CartItems (Sepet Kalemleri)

```sql
CREATE TABLE CartItems (
    CartItemId INT IDENTITY(1,1) PRIMARY KEY,
    CartId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    AddedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_CartItems_Carts FOREIGN KEY (CartId) REFERENCES Carts(OrderId) ON DELETE CASCADE,
    CONSTRAINT FK_CartItems_Products FOREIGN KEY (ProductId) REFERENCES Products(ProductId),
    CONSTRAINT CK_CartItems_Quantity CHECK (Quantity > 0),
    CONSTRAINT UQ_CartItems_Cart_Product UNIQUE (CartId, ProductId)
);

GO

CREATE INDEX IX_CartItems_CartId ON CartItems(CartId);
```

### 19. Notifications (Bildirimler)

```sql
CREATE TABLE Notifications (
    NotificationId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Title NVARCHAR(255) NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    NotificationType NVARCHAR(50) NOT NULL, -- Order, Payment, Stock, System
    IsRead BIT NOT NULL DEFAULT 0,
    ReadAt DATETIME2 NULL,
    ActionUrl NVARCHAR(500),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Notifications_Users FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);

GO

CREATE INDEX IX_Notifications_UserId ON Notifications(UserId);
CREATE INDEX IX_Notifications_IsRead ON Notifications(IsRead);
CREATE INDEX IX_Notifications_CreatedAt ON Notifications(CreatedAt);
```

### 20. AuditLogs (Denetim Kayıtları)

```sql
CREATE TABLE AuditLogs (
    AuditLogId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NULL,
    ActionType NVARCHAR(50) NOT NULL, -- Create, Update, Delete, Login, Logout
    EntityName NVARCHAR(100) NOT NULL, -- Product, Order, Customer vb.
    EntityId INT,
    OldValues NVARCHAR(MAX), -- JSON
    NewValues NVARCHAR(MAX), -- JSON
    IpAddress NVARCHAR(50),
    UserAgent NVARCHAR(500),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

GO

CREATE INDEX IX_AuditLogs_UserId ON AuditLogs(UserId);
CREATE INDEX IX_AuditLogs_ActionType ON AuditLogs(ActionType);
CREATE INDEX IX_AuditLogs_EntityName ON AuditLogs(EntityName);
CREATE INDEX IX_AuditLogs_CreatedAt ON AuditLogs(CreatedAt);
```

## 🔗 İlişkiler Diyagramı (ERD)

```
Users (1) ───< (N) UserRoles >── (1) Roles
  │
  │ (1)
  │
  └──< (N) Orders
                │
                │ (1)
                │
                └──< (N) OrderItems >── (1) Products
                                         │
                                         │ (N)
                                         │
                Categories (1) ─────────┘
                                         │
                                         │ (1)
                                         │
                    ProductPrices >─────┘

Customers (1) ───< (N) Orders
     │
     │ (N)
     │
CustomerGroups >──< (N) ProductPrices

Products (1) ───< (N) Stock >── (1) Warehouses
  │
  │ (N)
  │
StockMovements

Orders (1) ───< (N) Payments
Orders (1) ───< (N) OrderStatusHistory
```

## 📊 Views (Görünümler)

### v_ProductStockSummary

```sql
CREATE VIEW v_ProductStockSummary AS
SELECT
    p.ProductId,
    p.ProductCode,
    p.ProductName,
    p.CategoryId,
    c.CategoryName,
    SUM(s.Quantity) AS TotalQuantity,
    SUM(s.ReservedQuantity) AS TotalReserved,
    SUM(s.Quantity - s.ReservedQuantity) AS TotalAvailable,
    CASE WHEN SUM(s.Quantity - s.ReservedQuantity) <= 0 THEN 1 ELSE 0 END AS IsOutOfStock,
    CASE WHEN SUM(s.Quantity - s.ReservedQuantity) <= p.MinOrderQuantity THEN 1 ELSE 0 END AS IsLowStock
FROM Products p
INNER JOIN Stock s ON p.ProductId = s.ProductId
INNER JOIN Categories c ON p.CategoryId = c.CategoryId
WHERE p.IsActive = 1
GROUP BY p.ProductId, p.ProductCode, p.ProductName, p.CategoryId, c.CategoryName, p.MinOrderQuantity;
```

### v_CustomerOrderSummary

```sql
CREATE VIEW v_CustomerOrderSummary AS
SELECT
    c.CustomerId,
    c.CustomerCode,
    c.CompanyName,
    COUNT(o.OrderId) AS TotalOrders,
    SUM(CASE WHEN o.OrderStatus = 'Pending' THEN 1 ELSE 0 END) AS PendingOrders,
    SUM(CASE WHEN o.OrderStatus = 'Completed' THEN 1 ELSE 0 END) AS CompletedOrders,
    SUM(o.GrandTotal) AS TotalOrderAmount,
    SUM(CASE WHEN o.PaymentStatus = 'Unpaid' THEN o.GrandTotal ELSE 0 END) AS OutstandingBalance
FROM Customers c
LEFT JOIN Orders o ON c.CustomerId = o.CustomerId
WHERE c.IsActive = 1
GROUP BY c.CustomerId, c.CustomerCode, c.CompanyName;
```

## 🔄 Stored Procedures

### sp_SyncNetsimCustomers

```sql
CREATE PROCEDURE sp_SyncNetsimCustomers
    @LastSyncDate DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Netsim API'den carileri çeker ve veritabanına günceller
    -- Bu SP uygulama katmanında çağrılacak
    -- NetsimService tarafından kullanılır

    SELECT TOP 0 * FROM Customers; -- Placeholder
END;
```

### sp_GetProductPriceForCustomer

```sql
CREATE PROCEDURE sp_GetProductPriceForCustomer
    @ProductId INT,
    @CustomerId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CustomerGroupId INT = NULL;

    SELECT @CustomerGroupId = CustomerGroupId
    FROM Customers
    WHERE CustomerId = @CustomerId;

    SELECT TOP 1
        pp.Price,
        pp.Currency,
        pp.PriceCode
    FROM ProductPrices pp
    WHERE pp.ProductId = @ProductId
      AND (pp.CustomerGroupId = @CustomerGroupId OR pp.CustomerGroupId IS NULL)
      AND (pp.ValidUntil IS NULL OR pp.ValidUntil > GETUTCDATE())
    ORDER BY
        CASE WHEN pp.CustomerGroupId IS NOT NULL THEN 0 ELSE 1 END,
        pp.ValidFrom DESC;
END;
```

## 🔒 Index Optimization

```sql
-- Covering Index for Order Listing
CREATE INDEX IX_Orders_OrderListing ON Orders(OrderStatus, OrderDate)
INCLUDE (CustomerGroupId, TotalAmount, GrandTotal);

-- Covering Index for Product Search
CREATE INDEX IX_Products_Search ON Products(ProductCode, ProductName, CategoryId)
WHERE IsActive = 1;

-- Covering Index for Stock Monitoring
CREATE INDEX IX_Stock_LowStock ON Stock(ProductId, AvailableQuantity)
WHERE AvailableQuantity <= 100;
```

## 🧩 Full-Text Search (Optional)

```sql
-- Product Full-Text Search
CREATE FULLTEXT CATALOG ftCatalog AS DEFAULT;

CREATE FULLTEXT INDEX ON Products(ProductName, Description)
KEY INDEX PK_Products;

-- Search Query
SELECT * FROM Products
WHERE CONTAINS((ProductName, Description), 'aranan kelime');
```

## 📦 Partitioning (Large Scale - Future)

```sql
-- Order Partitioning by Year (Optional for large scale)
CREATE PARTITION FUNCTION pf_OrderByYear(DATETIME2)
AS RANGE RIGHT FOR VALUES (
    '2025-01-01', '2026-01-01', '2027-01-01'
);

CREATE PARTITION SCHEME ps_OrderByYear
AS PARTITION pf_OrderByYear
ALL TO ([PRIMARY]);
```

## 🗂️ Database Backup Strategy

```sql
-- Full Backup (Daily)
BACKUP DATABASE BLRB2B TO DISK = 'C:\Backup\BLRB2B_Full.bak'
WITH COMPRESSION, STATS = 10;

-- Differential Backup (Hourly)
BACKUP DATABASE BLRB2B TO DISK = 'C:\Backup\BLRB2B_Diff.bak'
WITH DIFFERENTIAL, COMPRESSION, STATS = 10;

-- Transaction Log Backup (Every 15 min)
BACKUP LOG BLRB2B TO DISK = 'C:\Backup\BLRB2B_Log.trn'
WITH COMPRESSION, STATS = 10;
```

## 🧪 Sample Test Data

```sql
-- Test Customer
INSERT INTO Customers (CustomerCode, CompanyName, TaxNumber, TaxOffice, Email, Phone, CustomerGroupId, CreditLimit, IsActive, IsApproved)
VALUES ('TEST001', 'Test Company A', '1234567890', 'Vergi Dairesi', 'test@test.com', '5551234567', 1, 100000, 1, 1);

-- Test Category
INSERT INTO Categories (CategoryName, CategoryName_EN, Description, IsActive)
VALUES ('Elektronik', 'Electronics', 'Elektronik ürünler', 1);

-- Test Product
INSERT INTO Products (ProductCode, ProductName, ProductName_EN, CategoryId, Unit, ListPrice, IsActive)
VALUES ('PRD001', 'Test Product', 'Test Product', 1, 'Adet', 1000.00, 1);
```
