using Microsoft.Extensions.DependencyInjection;
using BLRB2B.Application.Interfaces;
using BLRB2B.Application.Services;
using BLRB2B.Application.Helpers;

namespace BLRB2B.Application.Extensions;

/// <summary>
/// Extension methods for registering application services
/// </summary>
public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register AutoMapper
        services.AddAutoMapper(typeof(MappingProfile));

        // Register Services
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
