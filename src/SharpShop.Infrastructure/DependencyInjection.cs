using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharpShop.Application.Services;
using SharpShop.Domain.Interfaces;
using SharpShop.Infrastructure.Currency;
using SharpShop.Infrastructure.Data;
using SharpShop.Infrastructure.Repositories;

namespace SharpShop.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection"))
        );

        services.AddHttpClient<FrankfurterCurrencyClient>();

        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICurrencyService, FrankfurterCurrencyClient>();

        return services;
    }
}
