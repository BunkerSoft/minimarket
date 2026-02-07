using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MerkaCentro.Domain.Entities;
using MerkaCentro.Domain.Enums;
using MerkaCentro.Domain.ValueObjects;

namespace MerkaCentro.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MerkaCentroDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<MerkaCentroDbContext>>();

        try
        {
            await SeedUsersAsync(context, logger);
            await SeedCategoriesAsync(context, logger);
            await SeedExpenseCategoriesAsync(context, logger);
            await SeedSuppliersAsync(context, logger);
            await SeedProductsAsync(context, logger);

            logger.LogInformation("Datos semilla insertados correctamente");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al insertar datos semilla");
            throw;
        }
    }

    private static async Task SeedUsersAsync(MerkaCentroDbContext context, ILogger logger)
    {
        if (await context.Users.AnyAsync())
            return;

        var users = new[]
        {
            User.Create("admin", HashPassword("Admin123!"), "Administrador del Sistema", UserRole.Admin),
            User.Create("cajero", HashPassword("Cajero123!"), "Cajero Principal", UserRole.Cashier),
        };

        context.Users.AddRange(users);
        await context.SaveChangesAsync();
        logger.LogInformation("Usuarios creados: admin, cajero");
    }

    private static async Task SeedCategoriesAsync(MerkaCentroDbContext context, ILogger logger)
    {
        if (await context.Categories.AnyAsync())
            return;

        var categories = new[]
        {
            Category.Create("Bebidas", "Agua, gaseosas, jugos y bebidas en general"),
            Category.Create("Lacteos", "Leche, yogurt, queso y derivados"),
            Category.Create("Panaderia", "Pan, galletas, tortas y productos de panaderia"),
            Category.Create("Snacks y Golosinas", "Papas fritas, chocolates, caramelos y dulces"),
            Category.Create("Abarrotes", "Arroz, azucar, aceite, fideos y productos basicos"),
            Category.Create("Limpieza", "Detergente, jabon, lejia y productos de limpieza"),
            Category.Create("Cuidado Personal", "Shampoo, pasta dental, jabon y cuidado personal"),
            Category.Create("Frutas y Verduras", "Frutas frescas y verduras"),
            Category.Create("Carnes y Embutidos", "Pollo, carne, jamon y embutidos"),
            Category.Create("Congelados", "Helados, productos congelados y refrigerados"),
        };

        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();
        logger.LogInformation("Categorias creadas: {Count}", categories.Length);
    }

    private static async Task SeedExpenseCategoriesAsync(MerkaCentroDbContext context, ILogger logger)
    {
        if (await context.ExpenseCategories.AnyAsync())
            return;

        var expenseCategories = new[]
        {
            ExpenseCategory.Create("Alquiler", "Pago mensual de alquiler del local"),
            ExpenseCategory.Create("Servicios Basicos", "Luz, agua, internet y telefono"),
            ExpenseCategory.Create("Sueldos", "Pago de sueldos y salarios del personal"),
            ExpenseCategory.Create("Transporte", "Gastos de transporte y delivery"),
            ExpenseCategory.Create("Mantenimiento", "Reparaciones y mantenimiento del local"),
            ExpenseCategory.Create("Impuestos", "Pagos de impuestos y tributos"),
            ExpenseCategory.Create("Otros", "Gastos varios no categorizados"),
        };

        context.ExpenseCategories.AddRange(expenseCategories);
        await context.SaveChangesAsync();
        logger.LogInformation("Categorias de gasto creadas: {Count}", expenseCategories.Length);
    }

    private static async Task SeedSuppliersAsync(MerkaCentroDbContext context, ILogger logger)
    {
        if (await context.Suppliers.AnyAsync())
            return;

        var suppliers = new[]
        {
            Supplier.Create(
                "Distribuidora Lima SAC",
                "Distribuidora Lima S.A.C.",
                address: Address.Create("Av. Argentina 1234", "Cercado de Lima", "Lima")),
            Supplier.Create(
                "Productos del Norte EIRL",
                "Productos del Norte E.I.R.L.",
                address: Address.Create("Jr. Comercio 567", "Trujillo", "La Libertad")),
            Supplier.Create(
                "Alimentos Frescos SAC",
                "Alimentos Frescos S.A.C.",
                address: Address.Create("Calle Los Olivos 890", "Los Olivos", "Lima")),
        };

        context.Suppliers.AddRange(suppliers);
        await context.SaveChangesAsync();
        logger.LogInformation("Proveedores creados: {Count}", suppliers.Length);
    }

    private static async Task SeedProductsAsync(MerkaCentroDbContext context, ILogger logger)
    {
        if (await context.Products.AnyAsync())
            return;

        var categories = await context.Categories.ToDictionaryAsync(c => c.Name, c => c.Id);

        var products = new List<(string Code, string Name, string CatName, decimal PurchasePrice, decimal SalePrice, string Unit, bool AllowFractions)>
        {
            ("BEB001", "Agua Mineral 500ml", "Bebidas", 0.50m, 1.00m, "Unidad", false),
            ("BEB002", "Gaseosa Cola 500ml", "Bebidas", 1.20m, 2.50m, "Unidad", false),
            ("BEB003", "Jugo de Naranja 1L", "Bebidas", 2.00m, 3.50m, "Unidad", false),
            ("BEB004", "Gaseosa Limon 2L", "Bebidas", 2.50m, 5.00m, "Unidad", false),
            ("BEB005", "Agua Mineral 2.5L", "Bebidas", 1.00m, 2.00m, "Unidad", false),
            ("LAC001", "Leche Entera 1L", "Lacteos", 3.00m, 4.50m, "Unidad", false),
            ("LAC002", "Yogurt Natural 1L", "Lacteos", 3.50m, 5.50m, "Unidad", false),
            ("LAC003", "Queso Fresco 500g", "Lacteos", 6.00m, 9.00m, "Unidad", false),
            ("LAC004", "Mantequilla 200g", "Lacteos", 3.00m, 4.50m, "Unidad", false),
            ("PAN001", "Pan Frances (unidad)", "Panaderia", 0.10m, 0.20m, "Unidad", false),
            ("PAN002", "Pan Integral (unidad)", "Panaderia", 0.15m, 0.30m, "Unidad", false),
            ("PAN003", "Galletas Soda (paquete)", "Panaderia", 1.00m, 1.80m, "Paquete", false),
            ("SNK001", "Papas Fritas 150g", "Snacks y Golosinas", 2.00m, 3.50m, "Unidad", false),
            ("SNK002", "Chocolate con Leche 30g", "Snacks y Golosinas", 0.80m, 1.50m, "Unidad", false),
            ("SNK003", "Caramelos Surtidos 100g", "Snacks y Golosinas", 1.00m, 2.00m, "Bolsa", false),
            ("ABR001", "Arroz Extra 1kg", "Abarrotes", 3.50m, 4.80m, "Kilogramo", true),
            ("ABR002", "Azucar Rubia 1kg", "Abarrotes", 2.80m, 3.80m, "Kilogramo", true),
            ("ABR003", "Aceite Vegetal 1L", "Abarrotes", 5.00m, 7.50m, "Unidad", false),
            ("ABR004", "Fideos Spaghetti 500g", "Abarrotes", 1.50m, 2.50m, "Unidad", false),
            ("ABR005", "Atun en Conserva 170g", "Abarrotes", 3.00m, 4.50m, "Unidad", false),
            ("ABR006", "Sal de Mesa 1kg", "Abarrotes", 0.80m, 1.50m, "Kilogramo", false),
            ("LIM001", "Detergente en Polvo 500g", "Limpieza", 3.00m, 5.00m, "Unidad", false),
            ("LIM002", "Lejia 1L", "Limpieza", 2.00m, 3.50m, "Unidad", false),
            ("LIM003", "Jabon en Barra (unidad)", "Limpieza", 1.50m, 2.50m, "Unidad", false),
            ("CUI001", "Shampoo 400ml", "Cuidado Personal", 8.00m, 12.00m, "Unidad", false),
            ("CUI002", "Pasta Dental 100ml", "Cuidado Personal", 3.50m, 5.50m, "Unidad", false),
            ("CUI003", "Papel Higienico x4", "Cuidado Personal", 3.00m, 5.00m, "Paquete", false),
        };

        var now = DateTime.UtcNow;
        int count = 0;

        foreach (var p in products)
        {
            if (!categories.TryGetValue(p.CatName, out var catId))
                continue;

            var productId = Guid.NewGuid();
            var priceHistoryId = Guid.NewGuid();

            await context.Database.ExecuteSqlRawAsync(@"
                INSERT INTO Products (Id, Code, Name, CategoryId, PurchasePrice, PurchaseCurrency, SalePrice, SaleCurrency, MinStock, CurrentStock, Unit, AllowFractions, Status, CreatedAt)
                VALUES ({0}, {1}, {2}, {3}, {4}, 'PEN', {5}, 'PEN', 5, 0, {6}, {7}, 0, {8})",
                productId, p.Code, p.Name, catId, p.PurchasePrice, p.SalePrice, p.Unit, p.AllowFractions, now);

            await context.Database.ExecuteSqlRawAsync(@"
                INSERT INTO ProductPriceHistories (Id, ProductId, PurchasePrice, PurchaseCurrency, SalePrice, SaleCurrency, EffectiveDate, CreatedAt)
                VALUES ({0}, {1}, {2}, 'PEN', {3}, 'PEN', {4}, {4})",
                priceHistoryId, productId, p.PurchasePrice, p.SalePrice, now);

            count++;
        }

        logger.LogInformation("Productos creados: {Count}", count);
    }

    private static string HashPassword(string password)
    {
        const int saltSize = 16;
        const int hashSize = 32;
        const int iterations = 100000;

        var salt = RandomNumberGenerator.GetBytes(saltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            hashSize);

        var result = new byte[saltSize + hashSize];
        Buffer.BlockCopy(salt, 0, result, 0, saltSize);
        Buffer.BlockCopy(hash, 0, result, saltSize, hashSize);

        return Convert.ToBase64String(result);
    }
}
