using BLRB2B.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BLRB2B.Infrastructure.Data;

/// <summary>
/// Database seeder for initial data
/// </summary>
public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Eğer veri varsa dön
        if (await context.Categories.AnyAsync())
            return;

        var now = DateTime.Now;

        // 1. Kategoriler
        var categories = new List<Category>
        {
            new()
            {
                Name = "Elektronik",
                Code = "ELEC",
                Description = "Elektronik ürünler",
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                Name = "Bilgisayar",
                Code = "COMP",
                Description = "Bilgisayar ve laptop ürünleri",
                ParentCategoryId = 1, // Elektronik kategorisi altında
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                Name = "Telefon",
                Code = "PHONE",
                Description = "Telefon ve aksesuarları",
                ParentCategoryId = 1, // Elektronik kategorisi altında
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                Name = "Aksesuar",
                Code = "ACC",
                Description = "Bilgisayar ve telefon aksesuarları",
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();

        // Kategorileri yeniden yükle (ID'leri almak için)
        var electronicsCategory = await context.Categories.FirstAsync(c => c.Code == "ELEC");
        var computerCategory = await context.Categories.FirstAsync(c => c.Code == "COMP");
        var phoneCategory = await context.Categories.FirstAsync(c => c.Code == "PHONE");
        var accessoriesCategory = await context.Categories.FirstAsync(c => c.Code == "ACC");

        // 2. Müşteri Grupları
        var customerGroups = new List<CustomerGroup>
        {
            new()
            {
                Name = "Standart",
                DiscountPercent = 0,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                Name = "Bronz",
                DiscountPercent = 5,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                Name = "Gümüş",
                DiscountPercent = 10,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                Name = "Altın",
                DiscountPercent = 15,
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        await context.CustomerGroups.AddRangeAsync(customerGroups);
        await context.SaveChangesAsync();

        var standardGroup = await context.CustomerGroups.FirstAsync(g => g.Name == "Standart");
        var goldGroup = await context.CustomerGroups.FirstAsync(g => g.Name == "Altın");

        // 3. Müşteriler
        var customers = new List<Customer>
        {
            new()
            {
                CompanyName = "ABC Ltd. Şti.",
                ContactName = "Ahmet Yılmaz",
                PhoneNumber = "0212-555-0001",
                Email = "ahmet@abcltd.com",
                Address = "İstanbul, Türkiye",
                TaxNumber = "1234567890",
                TaxOffice = "Vergi Dairesi 1",
                IsActive = true,
                CreditLimit = 100000,
                DiscountRate = 10,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CompanyName = "XYZ Ticaret A.Ş.",
                ContactName = "Mehmet Demir",
                PhoneNumber = "0216-555-0002",
                Email = "mehmet@xyzticaret.com",
                Address = "İstanbul, Türkiye",
                TaxNumber = "0987654321",
                TaxOffice = "Vergi Dairesi 2",
                IsActive = true,
                CreditLimit = 150000,
                DiscountRate = 12,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                CompanyName = "Demo Ltd.",
                ContactName = "Ayşe Kaya",
                PhoneNumber = "0312-555-0003",
                Email = "ayse@demoltd.com",
                Address = "Ankara, Türkiye",
                TaxNumber = "1122334455",
                TaxOffice = "Vergi Dairesi 3",
                IsActive = true,
                CreditLimit = 50000,
                DiscountRate = 5,
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        await context.Customers.AddRangeAsync(customers);
        await context.SaveChangesAsync();

        var abcCustomer = await context.Customers.FirstAsync(c => c.Email == "ahmet@abcltd.com");
        var xyzCustomer = await context.Customers.FirstAsync(c => c.Email == "mehmet@xyzticaret.com");

        // 4. Kullanıcılar
        // Admin password hash for "admin123"
        var adminPasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123");

        // User password hash for "user123"
        var userPasswordHash = BCrypt.Net.BCrypt.HashPassword("user123");

        var users = new List<User>
        {
            new()
            {
                Username = "admin",
                Email = "admin@blrb2b.com",
                PasswordHash = adminPasswordHash,
                FirstName = "Admin",
                LastName = "User",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                Username = "ahmetyilmaz",
                Email = "ahmet@abcltd.com",
                PasswordHash = userPasswordHash,
                FirstName = "Ahmet",
                LastName = "Yılmaz",
                Role = UserRole.User,
                CustomerId = abcCustomer.Id,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                Username = "mehmetdemir",
                Email = "mehmet@xyzticaret.com",
                PasswordHash = userPasswordHash,
                FirstName = "Mehmet",
                LastName = "Demir",
                Role = UserRole.User,
                CustomerId = xyzCustomer.Id,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();

        // 5. Ürünler
        var products = new List<Product>
        {
            new()
            {
                Name = "Laptop Pro 15",
                Description = "Intel Core i7, 16GB RAM, 512GB SSD, 15.6 inç ekran",
                Sku = "LAPTOP-PRO-15",
                Price = 45000,
                StockQuantity = 25,
                Unit = "Adet",
                CategoryId = computerCategory.Id,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                Name = "Kablosuz Mouse",
                Description = "Ergonomik tasarım, 2.4GHz kablosuz, USB bağlantılı",
                Sku = "MOUSE-WL-001",
                Price = 450,
                StockQuantity = 150,
                Unit = "Adet",
                CategoryId = accessoriesCategory.Id,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                Name = "Mekanik Klavye",
                Description = "RGB aydınlatma, Cherry MX switch, Türkçe Q klavye",
                Sku = "KEYB-MEC-001",
                Price = 1000,
                StockQuantity = 80,
                Unit = "Adet",
                CategoryId = accessoriesCategory.Id,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                Name = "USB-C Hub",
                Description = "7'i 1 arada, HDMI, USB 3.0, SD kart okuyucu",
                Sku = "HUB-USBC-001",
                Price = 350,
                StockQuantity = 100,
                Unit = "Adet",
                CategoryId = accessoriesCategory.Id,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                Name = "Monitor 27 inç",
                Description = "IPS panel, 4K çözünürlük, HDR destekli",
                Sku = "MON-27-4K",
                Price = 8500,
                StockQuantity = 30,
                Unit = "Adet",
                CategoryId = computerCategory.Id,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                Name = "Akıllı Telefon Pro",
                Description = "6.7 inç OLED, 256GB, 5G destekli",
                Sku = "PHONE-PRO-256",
                Price = 35000,
                StockQuantity = 50,
                Unit = "Adet",
                CategoryId = phoneCategory.Id,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                Name = "Tablet Air",
                Description = "10.9 inç ekran, 64GB, Wi-Fi",
                Sku = "TAB-AIR-64",
                Price = 12000,
                StockQuantity = 40,
                Unit = "Adet",
                CategoryId = electronicsCategory.Id,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                Name = "Kulaklık Bluetooth",
                Description = "Aktif gürültü engelleme, 30 saat pil ömrü",
                Sku = "HEAD-BT-ANC",
                Price = 2500,
                StockQuantity = 60,
                Unit = "Adet",
                CategoryId = accessoriesCategory.Id,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }
}
