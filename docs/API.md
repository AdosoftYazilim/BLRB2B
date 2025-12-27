# BLRB2B - API Dokümantasyonu

## 📡 API Genel Bakış

**Base URL:** `https://api.blrb2b.com/api/v1`

**Authentication:** Bearer Token (JWT)

**Content-Type:** `application/json`

**Response Format:** JSON

## 🔐 Authentication

### Login

```http
POST /api/v1/auth/login
```

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "string"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "user": {
      "userId": 1,
      "email": "user@example.com",
      "firstName": "John",
      "lastName": "Doe",
      "roles": ["User"]
    },
    "expiresAt": "2025-12-27T15:00:00Z"
  }
}
```

### Logout

```http
POST /api/v1/auth/logout
```

**Headers:**
```
Authorization: Bearer {token}
```

### Refresh Token

```http
POST /api/v1/auth/refresh
```

**Request Body:**
```json
{
  "refreshToken": "string"
}
```

---

## 👤 User API

### Get Current User

```http
GET /api/v1/users/me
```

**Response:**
```json
{
  "success": true,
  "data": {
    "userId": 1,
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "phoneNumber": "5551234567",
    "roles": ["User"]
  }
}
```

### Update Profile

```http
PUT /api/v1/users/me
```

**Request Body:**
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "5551234567"
}
```

### Change Password

```http
POST /api/v1/users/change-password
```

**Request Body:**
```json
{
  "currentPassword": "string",
  "newPassword": "string"
}
```

---

## 🏢 Customer API

### List Customers (Admin)

```http
GET /api/v1/customers
```

**Query Parameters:**
```
?page=1
&pageSize=20
&search=searchTerm
&isActive=true
&customerGroupId=1
```

**Response:**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "customerId": 1,
        "customerCode": "TEST001",
        "companyName": "Test Company A",
        "email": "test@test.com",
        "phone": "5551234567",
        "customerGroupName": "VIP",
        "creditLimit": 100000.00,
        "currentBalance": 5000.00,
        "isActive": true,
        "isApproved": true
      }
    ],
    "totalCount": 100,
    "page": 1,
    "pageSize": 20
  }
}
```

### Get Customer by ID

```http
GET /api/v1/customers/{id}
```

### Create Customer (Admin)

```http
POST /api/v1/customers
```

**Request Body:**
```json
{
  "customerCode": "TEST001",
  "companyName": "Test Company A",
  "taxNumber": "1234567890",
  "taxOffice": "Vergi Dairesi",
  "address": "Test Address",
  "city": "Istanbul",
  "country": "Turkey",
  "phone": "5551234567",
  "email": "test@test.com",
  "customerGroupId": 1,
  "creditLimit": 100000.00
}
```

### Update Customer

```http
PUT /api/v1/customers/{id}
```

### Delete Customer

```http
DELETE /api/v1/customers/{id}
```

### Approve Customer

```http
POST /api/v1/customers/{id}/approve
```

---

## 📦 Product API

### List Products

```http
GET /api/v1/products
```

**Query Parameters:**
```
?page=1
&pageSize=20
&search=searchTerm
&categoryId=1
&isActive=true
&isFeatured=false
&sortBy=name
&sortOrder=asc
```

**Response:**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "productId": 1,
        "productCode": "PRD001",
        "productName": "Test Product",
        "categoryName": "Elektronik",
        "listPrice": 1000.00,
        "customerPrice": 900.00,
        "discountPercent": 10,
        "stock": {
          "availableQuantity": 50,
          "isOutOfStock": false,
          "isLowStock": false
        },
        "images": [
          {
            "imageUrl": "https://cdn.example.com/product1.jpg",
            "isMainImage": true
          }
        ]
      }
    ],
    "totalCount": 500,
    "page": 1,
    "pageSize": 20
  }
}
```

### Get Product by ID

```http
GET /api/v1/products/{id}
```

### Get Product Price for Customer

```http
GET /api/v1/products/{id}/price?customerId=1
```

**Response:**
```json
{
  "success": true,
  "data": {
    "productId": 1,
    "listPrice": 1000.00,
    "customerPrice": 900.00,
    "priceCode": "Bayi Fiyatı",
    "discountPercent": 10,
    "currency": "TRY"
  }
}
```

### Create Product (Admin)

```http
POST /api/v1/products
```

**Request Body:**
```json
{
  "productCode": "PRD001",
  "productName": "Test Product",
  "productName_EN": "Test Product EN",
  "description": "Product description",
  "description_EN": "Product description EN",
  "categoryId": 1,
  "brand": "Test Brand",
  "unit": "Adet",
  "barcode": "1234567890123",
  "listPrice": 1000.00,
  "minOrderQuantity": 1,
  "maxOrderQuantity": 100,
  "isActive": true
}
```

### Update Product

```http
PUT /api/v1/products/{id}
```

### Delete Product

```http
DELETE /api/v1/products/{id}
```

### Upload Product Image

```http
POST /api/v1/products/{id}/images
```

**Request:** `multipart/form-data`

---

## 🛒 Cart API

### Get Cart

```http
GET /api/v1/cart
```

**Response:**
```json
{
  "success": true,
  "data": {
    "cartId": 1,
    "items": [
      {
        "cartItemId": 1,
        "product": {
          "productId": 1,
          "productCode": "PRD001",
          "productName": "Test Product"
        },
        "quantity": 2,
        "unitPrice": 900.00,
        "lineTotal": 1800.00
      }
    ],
    "subTotal": 1800.00,
    "discountAmount": 0.00,
    "taxAmount": 360.00,
    "grandTotal": 2160.00
  }
}
```

### Add to Cart

```http
POST /api/v1/cart/items
```

**Request Body:**
```json
{
  "productId": 1,
  "quantity": 2
}
```

### Update Cart Item

```http
PUT /api/v1/cart/items/{itemId}
```

**Request Body:**
```json
{
  "quantity": 5
}
```

### Remove from Cart

```http
DELETE /api/v1/cart/items/{itemId}
```

### Clear Cart

```http
DELETE /api/v1/cart
```

### Validate Cart

```http
POST /api/v1/cart/validate
```

**Response:**
```json
{
  "success": true,
  "data": {
    "isValid": true,
    "warnings": [],
    "errors": []
  }
}
```

---

## 📋 Order API

### List Orders

```http
GET /api/v1/orders
```

**Query Parameters:**
```
?page=1
&pageSize=20
&search=searchTerm
&orderStatus=Pending
&paymentStatus=Unpaid
&customerId=1
&fromDate=2025-01-01
&toDate=2025-12-31
```

**Response:**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "orderId": 1,
        "orderNumber": "SO-2025-000001",
        "orderDate": "2025-01-15T10:30:00Z",
        "customerName": "Test Company A",
        "totalAmount": 2000.00,
        "grandTotal": 2400.00,
        "orderStatus": "Pending",
        "paymentStatus": "Unpaid"
      }
    ],
    "totalCount": 50,
    "page": 1,
    "pageSize": 20
  }
}
```

### Get Order by ID

```http
GET /api/v1/orders/{id}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "orderId": 1,
    "orderNumber": "SO-2025-000001",
    "orderDate": "2025-01-15T10:30:00Z",
    "customer": {
      "customerId": 1,
      "customerCode": "TEST001",
      "companyName": "Test Company A"
    },
    "items": [
      {
        "orderItemId": 1,
        "product": {
          "productId": 1,
          "productCode": "PRD001",
          "productName": "Test Product"
        },
        "quantity": 2,
        "unitPrice": 900.00,
        "discountPercent": 10,
        "lineTotal": 1800.00
      }
    ],
    "totalAmount": 2000.00,
    "discountAmount": 200.00,
    "taxAmount": 360.00,
    "shippingAmount": 40.00,
    "grandTotal": 2200.00,
    "orderStatus": "Pending",
    "paymentStatus": "Unpaid",
    "notes": "Test notes"
  }
}
```

### Create Order

```http
POST /api/v1/orders
```

**Request Body:**
```json
{
  "customerId": 1,
  "requiredDate": "2025-02-01",
  "items": [
    {
      "productId": 1,
      "quantity": 2
    }
  ],
  "notes": "Test order notes"
}
```

### Update Order Status (Admin)

```http
PUT /api/v1/orders/{id}/status
```

**Request Body:**
```json
{
  "orderStatus": "Approved",
  "notes": "Order approved"
}
```

### Approve Order (Admin)

```http
POST /api/v1/orders/{id}/approve
```

### Cancel Order

```http
POST /api/v1/orders/{id}/cancel
```

**Request Body:**
```json
{
  "reason": "Customer request"
}
```

### Reorder (Create from existing)

```http
POST /api/v1/orders/{id}/reorder
```

---

## 💳 Payment API

### Get Order Payments

```http
GET /api/v1/orders/{orderId}/payments
```

### Process Credit Card Payment

```http
POST /api/v1/payments/credit-card
```

**Request Body:**
```json
{
  "orderId": 1,
  "amount": 2200.00,
  "cardNumber": "5555555555555555",
  "cardHolder": "John Doe",
  "expiryMonth": 12,
  "expiryYear": 2026,
  "cvv": "123",
  "installment": 1
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "paymentId": 1,
    "transactionId": "TXN-123456",
    "authorizationCode": "AUTH-789",
    "amount": 2200.00,
    "currency": "TRY",
    "paymentDate": "2025-01-15T10:35:00Z",
    "status": "Success"
  }
}
```

### Record Bank Transfer

```http
POST /api/v1/payments/bank-transfer
```

**Request Body:**
```json
{
  "orderId": 1,
  "amount": 2200.00,
  "bankName": "Garanti BBVA",
  "accountNumber": "TR12 3456 7890 1234 5678 9012 34",
  "transferDate": "2025-01-15T10:35:00Z",
  "notes": "Bank transfer payment"
}
```

### Record Check Payment

```http
POST /api/v1/payments/check
```

**Request Body:**
```json
{
  "orderId": 1,
  "amount": 2200.00,
  "checkNumber": "CK123456",
  "bankName": "Garanti BBVA",
  "checkDate": "2025-02-15T00:00:00Z",
  "dueDate": "2025-02-15T00:00:00Z"
}
```

### Refund Payment

```http
POST /api/v1/payments/{paymentId}/refund
```

---

## 📊 Stock API

### Get Stock by Product

```http
GET /api/v1/stock/products/{productId}
```

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "warehouseId": 1,
      "warehouseName": "Merkez Depo",
      "quantity": 100,
      "reservedQuantity": 10,
      "availableQuantity": 90
    }
  ]
}
```

### Get Low Stock Products

```http
GET /api/v1/stock/low
```

**Query Parameters:**
```
?threshold=10
```

### Get Stock Movements

```http
GET /api/v1/stock/movements
```

**Query Parameters:**
```
?productId=1
&warehouseId=1
&fromDate=2025-01-01
&toDate=2025-12-31
```

### Adjust Stock (Admin)

```http
POST /api/v1/stock/adjust
```

**Request Body:**
```json
{
  "productId": 1,
  "warehouseId": 1,
  "quantity": 10,
  "notes": "Stock adjustment"
}
```

---

## 📈 Report API

### Sales Report

```http
GET /api/v1/reports/sales
```

**Query Parameters:**
```
?fromDate=2025-01-01
&toDate=2025-12-31
&customerId=1
&productId=1
&groupBy=day
&format=pdf
```

**Response:**
```json
{
  "success": true,
  "data": {
    "summary": {
      "totalOrders": 100,
      "totalRevenue": 500000.00,
      "averageOrderValue": 5000.00
    },
    "items": [
      {
        "date": "2025-01-15",
        "orders": 10,
        "revenue": 50000.00
      }
    ],
    "topProducts": [
      {
        "productId": 1,
        "productName": "Product A",
        "quantitySold": 500,
        "revenue": 50000.00
      }
    ],
    "topCustomers": [
      {
        "customerId": 1,
        "companyName": "Customer A",
        "totalOrders": 20,
        "totalSpent": 100000.00
      }
    ]
  }
}
```

### Customer Report

```http
GET /api/v1/reports/customers
```

**Query Parameters:**
```
?fromDate=2025-01-01
&toDate=2025-12-31
&customerGroupId=1
&format=pdf
```

**Response:**
```json
{
  "success": true,
  "data": {
    "summary": {
      "totalCustomers": 50,
      "activeCustomers": 45,
      "totalBalance": 250000.00
    },
    "items": [
      {
        "customerId": 1,
        "customerCode": "TEST001",
        "companyName": "Test Company A",
        "groupName": "VIP",
        "totalOrders": 20,
        "totalSpent": 100000.00,
        "currentBalance": 5000.00,
        "lastOrderDate": "2025-01-15"
      }
    ]
  }
}
```

---

## 🏷️ Category API

### List Categories

```http
GET /api/v1/categories
```

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "categoryId": 1,
      "categoryName": "Elektronik",
      "categoryName_EN": "Electronics",
      "description": "Elektronik ürünler",
      "parentCategoryId": null,
      "imageUrl": "https://cdn.example.com/category1.jpg",
      "isActive": true
    }
  ]
}
```

### Get Category Tree

```http
GET /api/v1/categories/tree
```

---

## 🔔 Notification API

### Get Notifications

```http
GET /api/v1/notifications
```

**Query Parameters:**
```
?isRead=false
&page=1
&pageSize=20
```

**Response:**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "notificationId": 1,
        "title": "Sipariş Onaylandı",
        "message": "Siparişiniz onaylandı",
        "notificationType": "Order",
        "isRead": false,
        "actionUrl": "/orders/1",
        "createdAt": "2025-01-15T10:30:00Z"
      }
    ],
    "unreadCount": 5,
    "totalCount": 20
  }
}
```

### Mark as Read

```http
PUT /api/v1/notifications/{id}/read
```

### Mark All as Read

```http
PUT /api/v1/notifications/read-all
```

---

## 🔄 Sync API (Admin)

### Sync Netsim Customers

```http
POST /api/v1/sync/netsim/customers
```

**Response:**
```json
{
  "success": true,
  "data": {
    "syncedCount": 50,
    "failedCount": 0,
    "duration": "00:00:05.123"
  }
}
```

### Sync Netsim Products

```http
POST /api/v1/sync/netsim/products
```

### Sync Netsim Stock

```http
POST /api/v1/sync/netsim/stock
```

### Sync Order to Netsim

```http
POST /api/v1/sync/netsim/orders/{id}
```

---

## ❌ Error Responses

All error responses follow this format:

```json
{
  "success": false,
  "error": {
    "code": "ERROR_CODE",
    "message": "Human readable error message",
    "details": "Additional error details (optional)"
  }
}
```

### Common Error Codes

| Code | HTTP Status | Description |
|------|-------------|-------------|
| UNAUTHORIZED | 401 | Authentication required or failed |
| FORBIDDEN | 403 | User doesn't have permission |
| NOT_FOUND | 404 | Resource not found |
| VALIDATION_ERROR | 400 | Input validation failed |
| CONFLICT | 409 | Resource conflict (duplicate, etc.) |
| INTERNAL_ERROR | 500 | Server error |

---

## 📝 Pagination

All list endpoints support pagination:

**Response Format:**
```json
{
  "success": true,
  "data": {
    "items": [...],
    "totalCount": 100,
    "page": 1,
    "pageSize": 20,
    "totalPages": 5
  }
}
```

---

## 🔍 Filtering & Sorting

**Common Query Parameters:**

```
?page=1              -- Page number
&pageSize=20         -- Items per page (max: 100)
&search=searchTerm   -- Full-text search
&sortBy=name         -- Sort field
&sortOrder=asc       -- asc or desc
```

---

## 🧪 Rate Limiting

| Tier | Requests | Window |
|------|----------|--------|
| Anonymous | 100 | 1 hour |
| Authenticated | 1000 | 1 hour |
| Admin | 10000 | 1 hour |

**Rate Limit Headers:**
```
X-RateLimit-Limit: 1000
X-RateLimit-Remaining: 999
X-RateLimit-Reset: 1705334400
```

---

## 📡 Webhooks (Future)

Webhooks allow external systems to receive real-time notifications.

**Endpoints:**
- Order created
- Order status changed
- Payment received
- Stock low

**Webhook Payload Example:**
```json
{
  "eventType": "order.created",
  "eventId": "evt_123456",
  "timestamp": "2025-01-15T10:30:00Z",
  "data": {
    "orderId": 1,
    "orderNumber": "SO-2025-000001",
    "customerName": "Test Company A",
    "totalAmount": 2200.00
  }
}
```
